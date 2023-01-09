using System;
using System.Text.RegularExpressions;
using System.Threading;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations
{
    internal sealed class PersonProcessor : IPersonProcessor
    {
        private const String PersonUrl = @"http://www.imdb.com/name/";

        private readonly TryGetValue TryGetValue;

        private readonly Action<String> AddRemoval;

        private readonly Func<String, String, String> AddUpdate;

        private readonly ILogger Logger;

        private readonly IWebServices WebServices;

        private static readonly Regex PersonUrlRegex;

        static PersonProcessor()
        {
            PersonUrlRegex = new Regex(@"http://((akas.)*|(www.)*|(us.)*|(german.)*)imdb.(com|de)/name/(?'NameLink'nm[0-9]+)/.*$", RegexOptions.Compiled);
        }

        public PersonProcessor(TryGetValue tryGetValue
            , Action<String> addRemoval
            , Func<String, String, String> addUpdate
            , ILog log
            , IWebServices webServices)
        {
            TryGetValue = tryGetValue;
            AddRemoval = addRemoval;
            AddUpdate = addUpdate;
            WebServices = webServices;

            Logger = new Logger(log);
        }

        public void Process(PersonInfo pi)
        {
            Int32 counter = 0;

            ProcessPerson(pi, counter);
        }

        private void ProcessPerson(PersonInfo person
            , Int32 counter)
        {
            try
            {
                TryProcessPerson(person);
            }
            catch (Exception ex)
            {
                HandleWebException(ex, person, counter);
            }
        }

        #region HandleWebException

        private void HandleWebException(Exception exception
            , PersonInfo person
            , Int32 counter)
        {
            if (ImdbIsBlocking(exception))
            {
                HandleDeniedException(person, counter);
            }
            else
            {
                HandleException(exception, person, counter);
            }
        }

        private static Boolean ImdbIsBlocking(Exception exception)
            => ((IsWebException(exception))
                && ((IsBadGateway(exception)) || ((IsServiceUnavailable(exception)))));

        private static Boolean IsServiceUnavailable(Exception exception)
            => (exception.Message.Contains("503"));

        private static Boolean IsBadGateway(Exception exception)
            => (exception.Message.Contains("502"));

        private static Boolean IsWebException(Exception exception)
            => (exception is System.Net.WebException);

        private void HandleDeniedException(PersonInfo person
            , Int32 counter)
        {
            Thread.Sleep(5000);

            ProcessPerson(person, counter);
        }

        private void HandleException(Exception exception
            , PersonInfo person
            , Int32 counter)
        {
            counter++;

            if ((counter >= 5) || (PageNotFound(exception)))
            {
                Logger.LogException(exception, person);
            }
            else
            {
                Thread.Sleep(20000);

                ProcessPerson(person, counter);
            }
        }

        private static Boolean PageNotFound(Exception exception)
            => ((IsWebException(exception)) && (exception.Message.Contains("404") || exception.Message.Contains("308")));

        #endregion

        private void TryProcessPerson(PersonInfo person)
        {
            IWebRequest request = WebServices.CreateWebRequest(PersonUrl + person.PersonLink);

            using (IWebResponse response = request.GetResponse())
            {
                ProcessResponse(response, person);
            }
        }

        private void ProcessResponse(IWebResponse response
            , PersonInfo oldPerson)
        {
            String responseUri = response.ResponseUri;

            Match match = PersonUrlRegex.Match(responseUri);

            if (match.Success)
            {
                String newLink = match.Groups["NameLink"].Value;

                TryProcessLink(oldPerson, newLink);
            }
        }

        private void TryProcessLink(PersonInfo oldPerson
            , String newLink)
        {
            if (LinkHasChanged(oldPerson.PersonLink, newLink))
            {
                ProcessLink(oldPerson, newLink);
            }
        }

        private void ProcessLink(PersonInfo oldPerson
            , String newLink)
        {
            PersonInfo newPerson;
            if (TryGetValue(newLink, out newPerson))
            {
                AddRemovalAndLogChange(oldPerson, newPerson);
            }
            else
            {
                TryAddUpdate(oldPerson, newLink);
            }
        }

        private void AddRemovalAndLogChange(PersonInfo oldPerson, PersonInfo newPerson)
        {
            AddRemoval(oldPerson.PersonLink);

            Logger.LogChange(oldPerson, newPerson);
        }

        private void TryAddUpdate(PersonInfo oldPerson
            , String newLink)
        {
            String otherOldLink = AddUpdate(oldPerson.PersonLink, newLink);

            if (String.IsNullOrEmpty(otherOldLink))
            {
                Logger.LogChange(oldPerson, newLink);
            }
            else
            {
                PersonInfo otherOldPerson;
                TryGetValue(otherOldLink, out otherOldPerson);

                PersonInfo newPerson = new PersonInfo(otherOldPerson);

                newPerson.PersonLink = newLink;

                AddRemovalAndLogChange(oldPerson, newPerson);
            }
        }

        private Boolean LinkHasChanged(String oldLink
            , String newLink)
            => (newLink != oldLink);
    }
}