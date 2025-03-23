using System;
using System.IO;
using System.Linq;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class LineReader
{
    internal static string ReadLine(StringReader sr)
    {
        var line = sr.ReadLine();

        var openTagCount = GetOpenTagCount(line);

        var closeTagCount = GetCloseTagCount(line);

        while (openTagCount > closeTagCount && sr.Peek() != -1)
        {
            line += sr.ReadLine();

            openTagCount = GetOpenTagCount(line);

            closeTagCount = GetCloseTagCount(line);

            if (line.Contains("</script>"))
            {
                break;
            }
        }

        return line;
    }

    private static int GetOpenTagCount(string line)
        => line.Count(c => c == '<') - (line.Split(new[] { "< " }, StringSplitOptions.None).Length - 1);

    private static int GetCloseTagCount(string line)
        => line.Count(c => c == '>');
}
