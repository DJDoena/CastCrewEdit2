using System.Text;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Output.Implementations
{
    internal sealed class OutputViewModel : IOutputViewModel
    {
        public OutputViewModel(ILog log)
        {
            Source = GetSource(log);
        }

        public string Source { get; private set; }

        private string GetSource(ILog log)
        {
            var html = new StringBuilder();

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