using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class CrewParser
{
    private static readonly Regex _crewBlockStartRegex;

    private static readonly Regex _creditTypeRegex;

    private static readonly Regex _crewRegex;

    internal static List<string> IgnoreCustomInIMDbCreditType { get; set; }

    internal static List<string> IgnoreIMDbCreditTypeInOther { get; set; }

    static CrewParser()
    {
        _crewBlockStartRegex = new Regex("<h4 (.*?)class=\"dataHeaderWithBorder\"(.*?)>", RegexOptions.Compiled | RegexOptions.Multiline);

        _creditTypeRegex = new Regex("<h4 (.*?)class=\"dataHeaderWithBorder\"(.*?)>(?'CreditType'.+?)(<span.+?)?(&nbsp;)?</h4>", RegexOptions.Compiled | RegexOptions.Multiline);

        _crewRegex = new Regex("<td (.*?)class=\"name\"(.*?)><a (.*?)href=\"/name/(?'PersonLink'[a-z0-9]+)/(.*?)>(?'PersonName'.+?)</a>.*?</td>.*?(\\.\\.\\.)?.*?((<td colspan=\"(2|3)\">)|(<td (.*?)class=\"credit\"(.*?)>))(?'Credit'.*?)</td>", RegexOptions.Compiled | RegexOptions.Multiline);
    }

    internal static void ParseCrew(string webSite, ref List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches)
    {
        crewMatches = null;

        ParseCrewOldStyle(webSite, ref crewMatches);

        if (crewMatches == null || crewMatches.Count == 0)
        {
            JsonCrewParser.Parse(webSite, ref crewMatches);
        }
    }

    private static void ParseCrewOldStyle(string webSite, ref List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches)
    {
        using var sr = new StringReader(webSite.ToString());

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            if (_crewBlockStartRegex.Match(line).Success && !line.Contains("id=\"cast\""))
            {
                var block = new StringBuilder();

                block.Append(line.Trim());

                while (!IMDbParser.BlockEndRegex.Match(line).Success && sr.Peek() != -1)
                {
                    line = LineReader.ReadLine(sr);
                    block.Append(line.Trim());
                }

                ProcessCrewLine(block.ToString(), ref crewMatches);
            }
        }
    }

    private static void ProcessCrewLine(string blockMatch, ref List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> matches)
    {
        Debug.Assert(IMDbParser.IsInitialized, "IMDbParser not initialized!");

        var creditTypeMatch = _creditTypeRegex.Match(blockMatch);

        var matchList = new List<CrewMatch>();

        var matchColl = _crewRegex.Matches(blockMatch);

        if (matchColl.Count > 0)
        {
            foreach (Match match in matchColl)
            {
                if (match.Success)
                {
                    var link = match.Groups["PersonLink"].Value;

                    var name = match.Groups["PersonName"].Value;

                    var creditGroup = match.Groups["Credit"];

                    var credit = creditGroup.Value;

                    var creditSuccess = creditGroup.Success;

                    matchList.Add(new(link, name, credit, creditSuccess));
                }
            }
        }

        matches ??= new();

        matches.Add(new KeyValuePair<CreditTypeMatch, List<CrewMatch>>(new(creditTypeMatch.Groups["CreditType"].Value), matchList));
    }

    internal static void ProcessCrewLine(List<CrewInfo> crewList, List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> matches, DefaultValues defaultValues, SetProgress setProgress)
    {
        var maxCount = matches.Count;

        for (var creditTypeIndex = 0; creditTypeIndex < maxCount;)
        {
            var maxTasks = ((creditTypeIndex + IMDbParser.MaxTasks - 1) < maxCount)
                ? IMDbParser.MaxTasks
                : (maxCount - creditTypeIndex);

            var tasks = new List<Task<CrewResult>>();

            for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, creditTypeIndex++)
            {
                var kvp = matches[creditTypeIndex];

                var task = Task.Run(() => ProcessCrewLine(kvp.Key, kvp.Value, defaultValues));

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                crewList.AddRange(task.Result.CrewMembers);

                for (var matchIndex = 0; matchIndex < task.Result.MatchCount; matchIndex++)
                {
                    setProgress();
                }
            }
        }
    }

    private static CrewResult ProcessCrewLine(CreditTypeMatch creditTypeMatch, List<CrewMatch> crewMatches, DefaultValues defaultValues)
    {
        var result = new List<CrewInfo>();

        foreach (var crewMatch in crewMatches)
        {
            if (crewMatch.CreditSuccess)
            {
                var credit = crewMatch.Credit;

                credit = credit.Replace("</a>", string.Empty);
                credit = credit.Replace("<br>", string.Empty);
                credit = credit.Trim();

                var originalCredit = HttpUtility.HtmlDecode(credit);

                if (credit.EndsWith(" and"))
                {
                    credit = credit.Substring(0, credit.Length - 4);
                    credit = credit.Trim();
                }

                if (credit.EndsWith(" &amp;"))
                {
                    credit = credit.Substring(0, credit.Length - 6);
                    credit = credit.Trim();
                }

                if (credit.EndsWith(" &"))
                {
                    credit = credit.Substring(0, credit.Length - 2);
                    credit = credit.Trim();
                }

                credit = HttpUtility.HtmlDecode(credit);
                credit = credit.Trim();

                var newMatch = IMDbParser.UncreditedRegex.Match(credit);

                if (newMatch.Success)
                {
                    continue;
                }

                var split = credit.Split('/');

                for (var crewIndex = 0; crewIndex < split.Length; crewIndex++)
                {
                    credit = split[crewIndex].Trim();

                    var crewInfo = CrewLineProcessor.Process(defaultValues, creditTypeMatch, crewMatch, credit, originalCredit);

                    if (crewInfo != null)
                    {
                        result.Add(crewInfo);
                    }
                }
            }
        }

        return new CrewResult(result, crewMatches.Count);
    }
}