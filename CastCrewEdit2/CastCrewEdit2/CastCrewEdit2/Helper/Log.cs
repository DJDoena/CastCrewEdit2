using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public sealed class Log
    {
        private StringBuilder LogBuilder;

        private readonly Object Lock;

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

        public Log()
        {
            LogBuilder = new StringBuilder();

            Lock = new Object();
        }

        public void AppendParagraph(String text)
        {
            lock (Lock)
            {
                LogBuilder.AppendLine("<p style=\"font-family:Courier New, Courier, monospace;\">");
                LogBuilder.AppendLine(text.Replace(Environment.NewLine, "<br/>" + Environment.NewLine));
                LogBuilder.AppendLine("</p>");
            }
        }

        public override String ToString()
        {
            lock (Lock)
            {
                return (LogBuilder.ToString());
            }
        }

        public void Show(WebBrowser webBrowser)
        {
            String file = Path.Combine(Path.GetTempPath(), "cce2log.html");

            using (StreamWriter sw = new StreamWriter(file, false, Encoding.UTF8))
            {
                sw.Write(ToString());
            }

            webBrowser.Navigate(file);
        }
    }
}