using System;
using System.Text;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Output.Implementations
{
    internal sealed class OutputViewModel : IOutputViewModel
    {
        public OutputViewModel(ILog log)
        {
            Source = GetSource(log);
        }

        public String Source { get; private set; }

        private String GetSource(ILog log)
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta http-equiv='Content-Type' content='text/html;charset=UTF-8' />");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine(log.ToString());
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return (html.ToString());
        }
    }
}