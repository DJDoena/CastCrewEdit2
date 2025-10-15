using System.IO;
using System.Text;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class DebugHelper
{
    internal static void FormatJson(string webSite)
    {
        using var sr = new StringReader(webSite);

        var debugBuilder = new StringBuilder();

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            SplitAtOpen(line, debugBuilder);
        }

        var result = debugBuilder.ToString();

        //System.Diagnostics.Debug.WriteLine(result);
    }

    private static void SplitAtOpen(string line, StringBuilder debugBuilder)
    {
        var splits = line.Split('{');

        SplitAtClose(splits[0], debugBuilder);

        for (var splitIndex = 1; splitIndex < splits.Length; splitIndex++)
        {
            debugBuilder.AppendLine("{");

            SplitAtClose(splits[splitIndex], debugBuilder);
        }
    }

    private static void SplitAtClose(string line, StringBuilder debugBuilder)
    {
        var splits = line.Split('}');

        for (var splitIndex = 0; splitIndex < splits.Length - 1; splitIndex++)
        {
            debugBuilder.AppendLine(splits[splitIndex]);
            debugBuilder.AppendLine("}");
        }

        debugBuilder.AppendLine(splits[splits.Length - 1]);
    }
}
