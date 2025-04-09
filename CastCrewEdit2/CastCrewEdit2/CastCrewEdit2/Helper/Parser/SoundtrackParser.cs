using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.JsonFragmentParser;
using Windows.Graphics.Printing;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class SoundtrackParser
{
    internal const string SoundtrackStartNewStyle = "\"section\":{\"items\":[{\"id\"";

    private static readonly Regex _soundtrackLiOldStyleRegex;

    private static readonly Regex _soundtrackTitleOldStyleRegex;

    private static readonly Regex _soundtrackPersonRegex;

    internal static Regex SoundtrackStartOldStyleRegex { get; }

    static SoundtrackParser()
    {
        SoundtrackStartOldStyleRegex = new Regex("<div id=\"soundtracks_content\"", RegexOptions.Compiled);

        _soundtrackLiOldStyleRegex = new Regex("<div id=\"(?'Soundtrack'.+?)</div>", RegexOptions.Singleline | RegexOptions.Compiled);

        _soundtrackTitleOldStyleRegex = new Regex("\"(?'Title'.+?)\"", RegexOptions.Compiled);

        _soundtrackPersonRegex = new Regex("<a(.+?)href=\"/name/(?'PersonLink'[a-z0-9]+)(.+?)\">(?'PersonName'.+?)</a>", RegexOptions.Compiled);
    }

    internal static void ParseSoundtrack(string titleLink, ref Dictionary<string, List<SoundtrackMatch>> soundtrackEntries)
    {
        soundtrackEntries = null;

        var soundtrackUrl = $"{IMDbParser.TitleUrl}{titleLink}/soundtrack/";

        var webSite = WebSiteReader.GetWebSite(soundtrackUrl, true);

        ParseSoundtrackNewStyle(webSite, ref soundtrackEntries);

        if (soundtrackEntries == null || soundtrackEntries.Count == 0)
        {
            ParseSoundtrackOldStyle(webSite, ref soundtrackEntries);
        }

        soundtrackEntries ??= new();
    }

    private static void ParseSoundtrackNewStyle(string webSite, ref Dictionary<string, List<SoundtrackMatch>> soundtrackEntries)
    {
        using var sr = new StringReader(webSite);

        string soundtrack = null;

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            var indexOf = line.IndexOf(SoundtrackStartNewStyle);

            if (indexOf != -1)
            {
                soundtrack = line.Substring(indexOf);

                break;
            }
        }

        if (soundtrack != null)
        {
            soundtrackEntries = ParseSoundtrackNewStyle(soundtrack);
        }
    }

    private static void ParseSoundtrackOldStyle(string webSite, ref Dictionary<string, List<SoundtrackMatch>> soundtrackEntries)
    {
        using var sr = new StringReader(webSite);

        var soundtrackFound = false;

        var soundtrack = new StringBuilder();

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            if (!soundtrackFound)
            {
                var beginMatch = SoundtrackStartOldStyleRegex.Match(line);

                if (beginMatch.Success)
                {
                    soundtrackFound = true;

                    continue;
                }
            }

            if (soundtrackFound)
            {
                soundtrack.AppendLine(line);
            }
        }

        if (soundtrack.Length > 0)
        {
            soundtrackEntries = ParseSoundtrackOldStyle(soundtrack);
        }
    }

    internal static void ProcessSoundtrackLine(List<CrewInfo> crewList, Dictionary<string, List<SoundtrackMatch>> soundtrackMatches, DefaultValues defaultValues, SetProgress setProgress)
    {
        if (defaultValues.CreditTypeSoundtrack)
        {
            var maxCount = soundtrackMatches.Count;

            var songs = soundtrackMatches.Keys.ToList();

            for (var songIndex = 0; songIndex < songs.Count;)
            {
                var maxTasks = ((songIndex + IMDbParser.MaxTasks - 1) < maxCount)
                    ? IMDbParser.MaxTasks
                    : (maxCount - songIndex);

                var tasks = new List<Task<CrewResult>>(maxTasks);

                for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, songIndex++)
                {
                    var song = songs[songIndex];

                    var songMatches = soundtrackMatches[song];

                    var task = Task.Run(() => SountrackLineProcessor.Process(song, songMatches, defaultValues));

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
    }

    private static Dictionary<string, List<SoundtrackMatch>> ParseSoundtrackOldStyle(StringBuilder soundtrack)
    {
        var matches = _soundtrackLiOldStyleRegex.Matches(soundtrack.ToString());

        var liMatches = new Dictionary<string, List<SoundtrackMatch>>(matches.Count);

        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                var titleSection = match.Groups["Soundtrack"].Value;

                var indexOf = titleSection.IndexOf("<br");

                if (indexOf != -1)
                {
                    titleSection = titleSection.Substring(0, indexOf);
                }

                indexOf = titleSection.IndexOf("\">");

                if (indexOf != -1)
                {
                    titleSection = titleSection.Substring(indexOf + 2);
                }

                var titleMatch = _soundtrackTitleOldStyleRegex.Match(titleSection);

                string key;
                if (titleMatch.Success)
                {
                    key = titleMatch.Groups["Title"].Value;
                    key = HttpUtility.HtmlDecode(key);
                    key = key.Trim();
                }
                else
                {
                    key = titleSection;
                    key = HttpUtility.HtmlDecode(key);
                    key = key.Trim();
                }

                var lines = match.Groups["Soundtrack"].Value.Split(new[] { "<br>", "<br />", "<br/>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var personMatches = _soundtrackPersonRegex.Matches(line);

                    if (personMatches.Count > 0)
                    {
                        if (!liMatches.TryGetValue(key, out var list))
                        {
                            list = new List<SoundtrackMatch>(personMatches.Count);

                            liMatches.Add(key, list);
                        }

                        foreach (Match personMatch in personMatches)
                        {
                            if (TryGetSoundtrackSubtypeMatch(line, out var subType))
                            {
                                list.Add(new SoundtrackMatch(subType, true, personMatch));
                            }
                            else if (TryGetCustomSoundtrackJob(line, out var job))
                            {
                                list.Add(new SoundtrackMatch(job, false, personMatch));
                            }
                            else
                            {
                                list.Add(new SoundtrackMatch(null, false, personMatch));
                            }
                        }
                    }
                }
            }
        }

        return liMatches;
    }

    private static Dictionary<string, List<SoundtrackMatch>> ParseSoundtrackNewStyle(string segment)
    {
        const string StartSegment = "{";

        var indexOfStart = segment.IndexOf(StartSegment);

        if (indexOfStart == -1)
        {
            return null;
        }

        var targetSegment = segment.Substring(indexOfStart);

        const string EndSegment = ",\"requestContext\":";

        var indexOfEnd = targetSegment.IndexOf(EndSegment);

        if (indexOfEnd != -1)
        {
            targetSegment = targetSegment.Substring(0, indexOfEnd);
        }

        JsonNode nodes;
        try
        {
            var cleaned = (new EndOfJsonParser()).GetJson(targetSegment);

            var rootNode = JsonTreeBuilder.Build(cleaned);

            nodes = rootNode["items"];
        }
        catch
        {
            return null;
        }

        var matches = new Dictionary<string, List<SoundtrackMatch>>();

        foreach (var node in nodes)
        {
            TryAddSoundtrackEntries(matches, node);
        }

        return matches;
    }

    private static void TryAddSoundtrackEntries(Dictionary<string, List<SoundtrackMatch>> matches, JsonNode node)
    {
        var titleNode = node["rowTitle"];

        var title = titleNode?.ValueAsString;

        if (string.IsNullOrEmpty(title))
        {
            return;
        }

        var listContentNodes = node["listContent"];

        if (listContentNodes != null)
        {
            foreach (var detailNodes in listContentNodes)
            {
                foreach (var detailNode in detailNodes)
                {
                    TryAddSoundtrackEntry(matches, title, detailNode.ValueAsString);
                }
            }
        }
    }

    private static void TryAddSoundtrackEntry(Dictionary<string, List<SoundtrackMatch>> matches, string title, string detail)
    {
        if (string.IsNullOrEmpty(detail))
        {
            return;
        }

        var personMatches = _soundtrackPersonRegex.Matches(detail);

        if (personMatches.Count > 0)
        {
            if (!matches.TryGetValue(title, out var titleMatches))
            {
                titleMatches = new List<SoundtrackMatch>();

                matches.Add(title, titleMatches);
            }

            foreach (Match personMatch in personMatches)
            {
                if (TryGetSoundtrackSubtypeMatch(detail, out var subType))
                {
                    titleMatches.Add(new SoundtrackMatch(subType, true, personMatch));
                }
                else if (TryGetCustomSoundtrackJob(detail, out var job))
                {
                    titleMatches.Add(new SoundtrackMatch(job, false, personMatch));
                }
                else
                {
                    titleMatches.Add(new SoundtrackMatch(null, false, personMatch));
                }
            }
        }
    }

    private static bool TryGetSoundtrackSubtypeMatch(string line, out string subType)
    {
        var musicSubTypes = IMDbParser.TransformationData.CreditTypeList
            .Where(ct => ct.DVDProfilerCreditType == CreditTypesDataGridViewHelper.CreditTypes.Music)
            .SelectMany(ct => ct.CreditSubtypeList)
            .Where(cst => !string.IsNullOrEmpty(cst.IMDbCreditSubtype.Value));

        foreach (var musicSubtype in musicSubTypes)
        {
            if (StartsWith(line, musicSubtype.IMDbCreditSubtype.Value))
            {
                subType = musicSubtype.DVDProfilerCreditSubtype;

                return true;
            }
        }

        subType = null;

        return false;
    }

    private static bool StartsWith(string line, string search)
        => line.TrimStart().StartsWith(search, StringComparison.InvariantCultureIgnoreCase);

    private static bool TryGetCustomSoundtrackJob(string line, out string job)
    {
        line = line.TrimStart();

        var indexOf = line.IndexOf(" by ", StringComparison.InvariantCultureIgnoreCase);

        if (indexOf != -1)
        {
            job = line.Substring(0, indexOf + 3);

            return true;
        }
        else
        {
            job = null;

            return false;
        }
    }
}
