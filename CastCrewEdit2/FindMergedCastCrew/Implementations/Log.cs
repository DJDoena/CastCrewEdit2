using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Implementations
{
    public sealed class Log : ILog
    {
        private const String PersonUrl = @"http://www.imdb.com/name/";

        private readonly StringBuilder LogBuilder;

        private readonly IIOServices IOServices;

        private readonly Object Lock;

        public Log(IIOServices ioServices)
        {
            IOServices = ioServices;

            LogBuilder = new StringBuilder();

            Lock = new Object();
        }

        #region ILog

        public Int32 Length
        {
            get
            {
                lock (Lock)
                {
                    return (LogBuilder.Length);
                }
            }
        }

        public void AppendParagraph(String text
            , Color color)
        {
            String rgb = GetRgb(color);

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

        public String CreateMultiplePersonOutput(IEnumerable<PersonInfo> persons)
        {
            StringBuilder nameList = new StringBuilder();

            foreach (PersonInfo item in persons)
            {
                String name = CreatePersonOutput(item, persons);

                nameList.Append(name);
            }

            return (nameList.ToString());
        }

        public String CreatePersonOutput(PersonInfo person)
            => (CreatePersonOutput(person, null));

        public String CreatePersonLinkHtml(String personLink)
            => ($"<a href=\"{PersonUrl}{personLink}/\" target=\"_blank\">{personLink}</a>");

        public void Save(String fileName)
        {
            using (Stream fs = IOServices.GetFileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sr = IOServices.GetStreamWriter(fs))
                {
                    String text = ToString();

                    sr.Write(text);
                }
            }
        }

        public void FromString(String log)
        {
            lock (Lock)
            {
                LogBuilder.Clear();

                LogBuilder.Append(log);
            }
        }

        #endregion

        private String CreatePersonOutput(PersonInfo person
            , IEnumerable<PersonInfo> others)
        {
            StringBuilder name = new StringBuilder();

            name.Append(CreatePersonLinkHtml(person));
            name.Append(": ");
            name.AppendLine(person.FormatActorNameWithBirthYearWithMarkersAsHtml(true, true, others));

            return (name.ToString());
        }

        private String CreatePersonLinkHtml(PersonInfo person)
            => (CreatePersonLinkHtml(person.PersonLink));

        public override String ToString()
        {
            lock (Lock)
            {
                return (LogBuilder.ToString());
            }
        }

        private static String GetRgb(Color color)
            => ($"#{color.R:X2}{color.G:X2}{color.B:X2}");
    }
}