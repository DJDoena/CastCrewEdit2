namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public sealed class Log
    {
        private readonly StringBuilder _logBuilder;

        private readonly object _lock;

        public int Length
        {
            get
            {
                lock (_lock)
                {
                    return _logBuilder.Length;
                }
            }
        }

        public Log()
        {
            _logBuilder = new StringBuilder();

            _lock = new object();
        }

        public void AppendParagraph(string text)
        {
            lock (_lock)
            {
                _logBuilder.AppendLine("<p style=\"font-family:Courier New, Courier, monospace;\">");
                _logBuilder.AppendLine(text.Replace(Environment.NewLine, "<br/>" + Environment.NewLine));
                _logBuilder.AppendLine("</p>");
            }
        }

        public override string ToString()
        {
            lock (_lock)
            {
                return _logBuilder.ToString();
            }
        }

        public void Show(WebBrowser webBrowser)
        {
            var file = Path.Combine(Path.GetTempPath(), "cce2log.html");

            using (var sw = new StreamWriter(file, false, Encoding.UTF8))
            {
                sw.Write(this.ToString());
            }

            webBrowser.Navigate(file);
        }
    }
}