using System;
using System.Drawing;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.ToolBox.Extensions;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations
{
    internal sealed class Logger : ILogger
    {
        private readonly ILog Log;

        public Logger(ILog log)
        {
            Log = log;
        }

        #region ILogger

        public void LogChange(PersonInfo oldPerson
            , PersonInfo newPerson)
        {
            var names = Log.CreateMultiplePersonOutput(ObjectExtensions.Enumerate(oldPerson, newPerson));

            if (AreIdentical(oldPerson, newPerson))
            {
                var log = $"These people are considered identical by IMDb. Any call to the first person will lead to the second person. But since the data is identical by DVD Profiler's standards, nothing needs to be done.{Environment.NewLine}{names}";

                Log.AppendParagraph(log, Color.LightBlue);
            }
            else
            {
                var log = $"These people are considered identical by IMDb. Any call to the first person will lead to the second person. Please adapt your DVD Profiler data accordingly.{Environment.NewLine}{names}";

                Log.AppendParagraph(log, Color.LightYellow);
            }
        }

        public void LogChange(PersonInfo oldPerson
            , string newLink)
        {
            var name = Log.CreatePersonOutput(oldPerson);

            newLink = Log.CreatePersonLinkHtml(newLink);

            var log = $"This person has gotten a new link which did not yet exist in your cache: {newLink}. Nothing needs to be done.{Environment.NewLine}{name}";

            Log.AppendParagraph(log, Color.LightBlue);
        }

        public void LogException(Exception exception
            , PersonInfo person)
        {
            var name = Log.CreatePersonOutput(person);

            var log = $"This person could not be resolved due to the follwing error.{Environment.NewLine}{name}{Environment.NewLine}{exception.Message}";

            Log.AppendParagraph(log, Color.LightCoral);
        }

        #endregion

        private bool AreIdentical(PersonInfo oldPerson
            , PersonInfo newPerson)
        {
            if (NamesAreIdentical(oldPerson, newPerson))
            {
                if ((string.IsNullOrEmpty(oldPerson.BirthYear) == false) || (string.IsNullOrEmpty(newPerson.BirthYear) == false))
                {
                    return (oldPerson.BirthYear == newPerson.BirthYear);
                }
                else
                {
                    return (oldPerson.FakeBirthYear == newPerson.FakeBirthYear);
                }
            }

            return (false);
        }

        private bool NamesAreIdentical(PersonInfo oldPerson
            , PersonInfo newPerson)
            => ((oldPerson.FirstName == newPerson.FirstName) && (oldPerson.LastName == newPerson.LastName) && (oldPerson.MiddleName == newPerson.MiddleName));
    }
}
