using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations
{
    internal delegate Boolean TryGetValue(String key, out PersonInfo value);

    internal sealed class PersonsProcessor : IPersonsProcessor
    {
        const Int32 MaxTasks = 5;

        private readonly IProcessData ProcessData;

        private readonly ILog Log;

        private readonly IWebServices WebServices;

        private readonly CancellationToken CancellationToken;

        private readonly Object RemovalLock;

        private readonly Object UpdateLock;

        private Dictionary<String, PersonInfo> PersonInfos { get; set; }

        private Int32 Step { get; set; }

        private Int32 ProgressMax { get; set; }

        private HashSet<String> Removals
            => (ProcessData.Removals);

        private Dictionary<String, String> Updates
            => (ProcessData.Updates);

        private HashSet<String> ProcessedPersons
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

            RemovalLock = new Object();
            UpdateLock = new Object();
        }

        #region IPersonsProcessor

        public event EventHandler<EventArgs<Int32>> ProgressMaxChanged;

        public event EventHandler<EventArgs<Int32>> ProgressValueChanged;

        public PersonInfos Process(PersonInfos persons)
        {
            Initialize(persons);

            Process();

            RemoveOldDuplicates();

            UpdateNewLinks();

            PersonInfos personInfos = GetOutput();

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

        private Dictionary<String, PersonInfo> GetDictionary(PersonInfos list)
        {
            Dictionary<String, PersonInfo> dict = new Dictionary<String, PersonInfo>(list.PersonInfoList.Length);

            List<PersonInfo> sorted = new List<PersonInfo>(list.PersonInfoList);

            sorted.Sort((left, right) => right.LastModified.CompareTo(left.LastModified));

            foreach (PersonInfo pi in sorted)
            {
                dict.Add(pi.PersonLink, pi);
            }

            return (dict);
        }

        private Int32 GetStep()
        {
            Int32 maxSteps = (ProgressMax > 10000) ? 1000 : 100;

            Int32 step = 1;

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
            List<PersonInfo> personInfos = PersonInfos.Values.ToList();

            Int32 progress = 0;

            for (Int32 personIndex = 0; personIndex < personInfos.Count;)
            {
                Int32 previousProgress = progress;

                try
                {
                    progress += RunTasks(personInfos, ref personIndex);
                }
                catch (AggregateException aggrEx)
                {
                    Exception ex = aggrEx.InnerExceptions.First();

                    throw (new Exception(aggrEx.Message, ex));
                }

                for (Int32 i = previousProgress; i < progress; i++)
                {
                    FireProgressValueChanged(i);
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private Int32 RunTasks(List<PersonInfo> personInfos
            , ref Int32 personIndex)
        {
            Int32 maxCount = personInfos.Count;

            Int32 maxTasks = ((personIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - personIndex);

            List<Task> tasks = new List<Task>(MaxTasks);

            for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++, personIndex++)
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
                Task task = ProcessPerson(personInfo);

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
            foreach (String key in Removals)
            {
                PersonInfos.Remove(key);
            }
        }

        private void UpdateNewLinks()
        {
            foreach (KeyValuePair<String, String> kvp in Updates)
            {
                TryUpdateNewLink(kvp);
            }
        }

        private void TryUpdateNewLink(KeyValuePair<String, String> update)
        {
            PersonInfo personInfo;
            if (PersonInfos.TryGetValue(update.Key, out personInfo))
            {
                UpdateNewLink(update, personInfo);
            }
        }

        private void UpdateNewLink(KeyValuePair<String, String> update
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
            PersonInfos personInfos = new PersonInfos();

            List<PersonInfo> list = PersonInfos.Values.ToList();

            list.Sort(PersonInfo.CompareForSorting);

            personInfos.PersonInfoList = list.ToArray();

            return (personInfos);
        }

        private void FireProgressValueChanged(Int32 value)
        {
            if ((value % Step) == 0)
            {
                ProgressValueChanged?.Invoke(this, new EventArgs<Int32>(value));
            }
        }

        private void FireProgressMaxChanged()
        {
            Step = GetStep();

            ProgressMaxChanged?.Invoke(this, new EventArgs<Int32>(ProgressMax));
        }

        #region Delegates

        private void AddRemoval(String link)
        {
            lock (RemovalLock)
            {
                Removals.Add(link);
            }
        }

        private String AddUpdate(String oldLink
            , String newLink)
        {
            lock (UpdateLock)
            {
                String otherOldLink = Updates.Where(item => item.Value == newLink).FirstOrDefault().Key;

                if (String.IsNullOrEmpty(otherOldLink))
                {
                    Updates.Add(oldLink, newLink);
                }

                return (otherOldLink);
            }
        }

        #endregion
    }
}