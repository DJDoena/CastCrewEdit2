using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations
{
    internal delegate bool TryGetValue(string key, out PersonInfo value);

    internal sealed class PersonsProcessor : IPersonsProcessor
    {
        private const int MaxTasks = 5;

        private readonly IProcessData ProcessData;

        private readonly ILog Log;

        private readonly IWebServices WebServices;

        private readonly CancellationToken CancellationToken;

        private readonly object RemovalLock;

        private readonly object UpdateLock;

        private Dictionary<string, PersonInfo> PersonInfos { get; set; }

        private int Step { get; set; }

        private int ProgressMax { get; set; }

        private HashSet<string> Removals
            => (ProcessData.Removals);

        private Dictionary<string, string> Updates
            => (ProcessData.Updates);

        private HashSet<string> ProcessedPersons
            => (ProcessData.ProcessedPersons);

        public PersonsProcessor(IProcessData processData
            , ILog log
            , IWebServices webServices
            , CancellationToken cancellationToken)
        {
            ProcessData = processData;
            Log = log;
            WebServices = webServices;
            CancellationToken = cancellationToken;

            RemovalLock = new object();
            UpdateLock = new object();
        }

        #region IPersonsProcessor

        public event EventHandler<EventArgs<int>> ProgressMaxChanged;

        public event EventHandler<EventArgs<int>> ProgressValueChanged;

        public PersonInfos Process(PersonInfos persons)
        {
            Initialize(persons);

            Process();

            RemoveOldDuplicates();

            UpdateNewLinks();

            var personInfos = GetOutput();

            return (personInfos);
        }

        #endregion

        #region Initialization

        private void Initialize(PersonInfos persons)
        {
            PersonInfos = GetDictionary(persons);

            ProgressMax = PersonInfos.Count;

            FireProgressMaxChanged();

            FireProgressValueChanged(0);
        }

        private Dictionary<string, PersonInfo> GetDictionary(PersonInfos list)
        {
            var dict = new Dictionary<string, PersonInfo>(list.PersonInfoList.Length);

            var sorted = new List<PersonInfo>(list.PersonInfoList);

            sorted.Sort((left, right) => right.LastModified.CompareTo(left.LastModified));

            foreach (var pi in sorted)
            {
                dict.Add(pi.PersonLink, pi);
            }

            return (dict);
        }

        private int GetStep()
        {
            var maxSteps = (ProgressMax > 10000) ? 1000 : 100;

            var step = 1;

            if (ProgressMax > maxSteps)
            {
                step = ProgressMax / maxSteps;

                if ((ProgressMax % maxSteps) != 0)
                {
                    step++;
                }
            }

            return (step);
        }

        #endregion

        #region Process

        private void Process()
        {
            var personInfos = PersonInfos.Values.ToList();

            var progress = 0;

            for (var personIndex = 0; personIndex < personInfos.Count;)
            {
                var previousProgress = progress;

                try
                {
                    progress += RunTasks(personInfos, ref personIndex);
                }
                catch (AggregateException aggrEx)
                {
                    var ex = aggrEx.InnerExceptions.First();

                    throw (new Exception(aggrEx.Message, ex));
                }

                for (var i = previousProgress; i < progress; i++)
                {
                    FireProgressValueChanged(i);
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private int RunTasks(List<PersonInfo> personInfos
            , ref int personIndex)
        {
            var maxCount = personInfos.Count;

            var maxTasks = ((personIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - personIndex);

            var tasks = new List<Task>(MaxTasks);

            for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, personIndex++)
            {
                TryRunTask(personInfos[personIndex], tasks);
            }

            Task.WaitAll(tasks.ToArray());

            return (tasks.Count);
        }

        private void TryRunTask(PersonInfo personInfo
            , List<Task> tasks)
        {
            if (ProcessedPersons.Contains(personInfo.PersonLink) == false)
            {
                var task = ProcessPerson(personInfo);

                tasks.Add(task);

                ProcessedPersons.Add(personInfo.PersonLink);
            }
            else
            {
                ProgressMax--;

                FireProgressMaxChanged();
            }
        }

        private Task ProcessPerson(PersonInfo person)
            => (Task.Run(() => ProcessPersonInTask(person)));

        private void ProcessPersonInTask(PersonInfo person)
        {
            IPersonProcessor processor = new PersonProcessor(PersonInfos.TryGetValue, AddRemoval, AddUpdate, Log, WebServices);

            processor.Process(person);
        }

        #endregion

        private void RemoveOldDuplicates()
        {
            foreach (var key in Removals)
            {
                PersonInfos.Remove(key);
            }
        }

        private void UpdateNewLinks()
        {
            foreach (var kvp in Updates)
            {
                TryUpdateNewLink(kvp);
            }
        }

        private void TryUpdateNewLink(KeyValuePair<string, string> update)
        {
            PersonInfo personInfo;
            if (PersonInfos.TryGetValue(update.Key, out personInfo))
            {
                UpdateNewLink(update, personInfo);
            }
        }

        private void UpdateNewLink(KeyValuePair<string, string> update
            , PersonInfo personInfo)
        {
            PersonInfos.Remove(update.Key);

            if (PersonInfos.ContainsKey(update.Value) == false) //multiple old entries could map to one new entry
            {
                personInfo.PersonLink = update.Value;

                PersonInfos.Add(update.Value, personInfo);
            }
        }

        private PersonInfos GetOutput()
        {
            var personInfos = new PersonInfos();

            var list = PersonInfos.Values.ToList();

            list.Sort(PersonInfo.CompareForSorting);

            personInfos.PersonInfoList = list.ToArray();

            return (personInfos);
        }

        private void FireProgressValueChanged(int value)
        {
            if ((value % Step) == 0)
            {
                ProgressValueChanged?.Invoke(this, new EventArgs<int>(value));
            }
        }

        private void FireProgressMaxChanged()
        {
            Step = GetStep();

            ProgressMaxChanged?.Invoke(this, new EventArgs<int>(ProgressMax));
        }

        #region Delegates

        private void AddRemoval(string link)
        {
            lock (RemovalLock)
            {
                Removals.Add(link);
            }
        }

        private string AddUpdate(string oldLink
            , string newLink)
        {
            lock (UpdateLock)
            {
                var otherOldLink = Updates.Where(item => item.Value == newLink).FirstOrDefault().Key;

                if (string.IsNullOrEmpty(otherOldLink))
                {
                    Updates.Add(oldLink, newLink);
                }

                return (otherOldLink);
            }
        }

        #endregion
    }
}