using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class CastParser
{


    private static readonly Regex _castRegex;

    private static readonly Regex _castBlockStartRegex;

    private static readonly Regex _castLineRegex;

    static CastParser()
    {
        _castRegex = new Regex("<td class=\"primary_photo\">.*?<td><a href=\"/name/(?'PersonLink'[a-z0-9]+)/.*?>(?'PersonName'.+?)</a>          </td><td class=\"ellipsis\">(\\.\\.\\.)?</td><td class=\"character\">(<div>)?(?'Role'.*?)(</div>)?</td>", RegexOptions.Compiled);

        _castBlockStartRegex = new Regex("<table (.*?)class=\"cast_list\"(.*?)>", RegexOptions.Compiled | RegexOptions.Multiline);

        _castLineRegex = new Regex("<tr (.*?)class=\"(odd|even)\"(.*?)>.*?</tr>", RegexOptions.Compiled | RegexOptions.Multiline);
    }


    internal static void ParseCast(string webSite, ref List<CastMatch> castMatches)
    {
        castMatches = null;

        ParseCastOldStyle(webSite, ref castMatches);

        if (castMatches == null || castMatches.Count == 0)
        {
            JsonCastParser.Parse(webSite, ref castMatches);
        }

        castMatches ??= new();
    }

    private static void ParseCastOldStyle(string webSite, ref List<CastMatch> castMatches)
    {
        using var sr = new StringReader(webSite);

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            if (_castBlockStartRegex.Match(line).Success)
            {
                var block = new StringBuilder();

                block.Append(line.Trim());

                while (!IMDbParser.BlockEndRegex.Match(line).Success && sr.Peek() != -1)
                {
                    line = LineReader.ReadLine(sr);

                    block.Append(line.Trim());
                }

                var lineMatches = _castLineRegex.Matches(block.ToString());

                if (lineMatches.Count > 0)
                {
                    foreach (Match lineMatch in lineMatches)
                    {
                        if (lineMatch.Success)
                        {
                            CastParser.ProcessCastLine(lineMatch.Value, ref castMatches);
                        }
                    }
                }
            }
        }
    }




    private static void ProcessCastLine(string linePart, ref List<CastMatch> matches)
    {
        Debug.Assert(IMDbParser.IsInitialized, "IMDbParser not initialized!");

        var matchColl = _castRegex.Matches(linePart);

        if (matchColl.Count > 0)
        {
            matches ??= new();

            foreach (Match match in matchColl)
            {
                var link = match.Groups["PersonLink"].Value;

                var name = match.Groups["PersonName"].Value;

                var roleGroup = match.Groups[ColumnNames.Role];

                var role = roleGroup.Value;

                matches.Add(new(link, name, role, roleGroup.Success));
            }
        }
    }

    internal static void ProcessCastLine(List<CastInfo> castList, List<CastMatch> matches, DefaultValues defaultValues, SetProgress setProgress)
    {
        var maxCount = matches.Count;

        for (var castIndex = 0; castIndex < maxCount;)
        {
            var maxTasks = ((castIndex + IMDbParser.MaxTasks - 1) < maxCount)
                ? IMDbParser.MaxTasks
                : (maxCount - castIndex);

            var tasks = new List<Task<List<CastInfo>>>(maxTasks);

            for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, castIndex++)
            {
                var match = matches[castIndex];

                var task = Task.Run(() => CastLineProcessor.Process(match, defaultValues));

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                foreach (var castMember in task.Result)
                {
                    castList.Add(castMember);
                }
            }

            for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++)
            {
                setProgress();
            }
        }
    }
}