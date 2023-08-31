namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using AbstractionLayer.IOServices;
    using CastCrewEdit2;

    public sealed class Log : ILog
    {
        private const string PersonUrl = @"http://www.imdb.com/name/";

        private readonly StringBuilder LogBuilder;

        private readonly IIOServices IOServices;

        private readonly object Lock;

        public Log(IIOServices ioServices)
        {
            IOServices = ioServices;

            LogBuilder = new StringBuilder();

            Lock = new object();
        }

        #region ILog

        public int Length
        {
            get
            {
                lock (Lock)
                {
                    return (LogBuilder.Length);
                }
            }
        }

        public void AppendParagraph(string text
            , Color color)
        {
            var rgb = GetRgb(color);

            lock (Lock)
            {
                LogBuilder.AppendLine($"<p style=\"font-family:Courier New, Courier, monospace; background-color: {rgb};\">");

                text = text.Replace(Environment.NewLine, "<br/>" + Environment.NewLine);

                LogBuilder.AppendLine(text);
                LogBuilder.AppendLine("</p>");
            }
        }

        public void Clear()
        {
            lock (Lock)
            {
                LogBuilder.Clear();
            }
        }

        public string CreateMultiplePersonOutput(IEnumerable<PersonInfo> persons)
        {
            var nameList = new StringBuilder();

            foreach (var item in persons)
            {
                var name = CreatePersonOutput(item, persons);

                nameList.Append(name);
            }

            return (nameList.ToString());
        }

        public string CreatePersonOutput(PersonInfo person)
            => (CreatePersonOutput(person, null));

        public string CreatePersonLinkHtml(string personLink)
            => ($"<a href=\"{PersonUrl}{personLink}/\" target=\"_blank\">{personLink}</a>");

        public void Save(string fileName)
        {
            using (var fs = IOServices.GetFileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var sr = new StreamWriter(fs, Encoding.UTF8))
                {
                    var text = ToString();

                    sr.Write(text);
                }
            }
        }

        public void FromString(string log)
        {
            lock (Lock)
            {
                LogBuilder.Clear();

                LogBuilder.Append(log);
            }
        }

        #endregion

        private string CreatePersonOutput(PersonInfo person
            , IEnumerable<PersonInfo> others)
        {
            var name = new StringBuilder();

            name.Append(CreatePersonLinkHtml(person));
            name.Append(": ");
            name.AppendLine(person.FormatActorNameWithBirthYearWithMarkersAsHtml(true, true, others));

            return (name.ToString());
        }

        private string CreatePersonLinkHtml(PersonInfo person)
            => (CreatePersonLinkHtml(person.PersonLink));

        public override string ToString()
        {
            lock (Lock)
            {
                return (LogBuilder.ToString());
            }
        }

        private static string GetRgb(Color color)
            => ($"#{color.R:X2}{color.G:X2}{color.B:X2}");
    }
}