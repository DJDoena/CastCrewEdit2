using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DoenaSoft.JsonFragmentParser;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class TriviaParser
{
    private const string TriviaStartNewStyle = "\"categories\":[{\"id\"";

    private static readonly Regex _hrefRegex;

    private static readonly Regex _triviaStartRegex;

    private static readonly Regex _triviaLiRegex;

    static TriviaParser()
    {
        _triviaStartRegex = new Regex("class=\"soda (even|odd) sodavote\".*?>", RegexOptions.Compiled);

        _triviaLiRegex = new Regex("<div class=\"sodatext\">(?'Trivia'.+?)</div>", RegexOptions.Singleline | RegexOptions.Compiled);

        _hrefRegex = new Regex("<a(.+?)href=\"(?'Link'.+?)\"(.*?)>", RegexOptions.Compiled);
    }

    internal static StringBuilder ParseTrivia(string triviaUrl)
    {
        var webSite = WebSiteReader.GetWebSite(triviaUrl);

        var triviaResult = ParseTriviaNewStyle(triviaUrl, webSite);

        if (triviaResult == null || triviaResult.Length == 0)
        {
            triviaResult = ParseTriviaOldStyle(triviaUrl, webSite);
        }

        return triviaResult;
    }

    private static StringBuilder ParseTriviaNewStyle(string triviaUrl, string webSite)
    {
        using var sr = new StringReader(webSite);

        string trivia = null;

        while (sr.Peek() != -1)
        {
            var line = sr.ReadLine();

            var indexOf = line.IndexOf(TriviaStartNewStyle);

            if (indexOf != -1)
            {
                trivia = line.Substring(indexOf);

                break;
            }
        }

        if (trivia != null)
        {
            var triviasResult = ParseTriviaNewStyleBySegement(triviaUrl, trivia);

            return triviasResult;
        }

        return null;
    }

    private static StringBuilder ParseTriviaOldStyle(string triviaUrl, string webSite)
    {
        using var sr = new StringReader(webSite);

        var triviaFound = false;

        var trivia = new StringBuilder();

        while (sr.Peek() != -1)
        {
            var line = sr.ReadLine();

            if (!triviaFound)
            {
                var beginMatch = _triviaStartRegex.Match(line);

                if (beginMatch.Success)
                {
                    triviaFound = true;

                    continue;
                }
            }

            if (triviaFound)
            {
                trivia.AppendLine(line);
            }
        }

        if (trivia.Length > 0)
        {
            var triviaResult = ParseTrivia(trivia, triviaUrl);

            return triviaResult;
        }

        return null;
    }

    private static StringBuilder ParseTrivia(StringBuilder triviaResults, string triviaUrl)
    {
        var matches = _triviaLiRegex.Matches(triviaResults.ToString());

        triviaResults = new StringBuilder();

        triviaResults.AppendLine("<div style=\"display:none\">");

        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                if (match.Groups["Trivia"].Success)
                {
                    var triviaResult = match.Groups["Trivia"].Value.Trim();

                    AddTriviaResult(triviaUrl, triviaResult, triviaResults);
                }
            }
        }

        triviaResults.AppendLine("</div>");

        return triviaResults;
    }

    private static StringBuilder ParseTriviaNewStyleBySegement(string triviaUrl, string segment)
    {
        const string StartSegment = "[";

        var indexOfStart = segment.IndexOf(StartSegment);

        if (indexOfStart == -1)
        {
            return null;
        }

        var trivias = segment.Substring(indexOfStart);

        const string EndSegment = ",\"requestContext\":";

        var indexOfEnd = trivias.IndexOf(EndSegment);

        if (indexOfEnd != -1)
        {
            trivias = trivias.Substring(0, indexOfEnd);
        }

        JsonRootNode triviaNode;
        try
        {
            var cleanedTrivia = (new EndOfJsonParser()).GetJson(trivias);

            triviaNode = JsonTreeBuilder.Build(cleanedTrivia);
        }
        catch
        {
            return null;
        }

        var result = GoofOrTriviaParser.TryAddGoofOrTriviaEntries(triviaUrl, triviaNode, AddTriviaResult);

        return result;

    }

    internal static void AddTriviaResult(string url, string value, StringBuilder result)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var hrefMatches = _hrefRegex.Matches(value);

            foreach (Match match in hrefMatches)
            {
                value = value.Replace(match.Value, $"<a href=\"{match.Groups["Link"]}\">");
            }


            value = value.Replace("href=\"#", "href=\"" + url + "#");
            value = value.Replace("href=\"/", "href=\"" + IMDbParser.BaseUrl + "/");
            value = value.Replace("href=\"?", "href=\"" + IMDbParser.BaseUrl + "?");
            value = value.Replace(" />", ">");
            value = value.Replace("/>", ">");
            value = value.Trim();

            while (value.EndsWith("<br>"))
            {
                value = value.Substring(0, value.Length - 4).TrimEnd();
            }

            result.AppendLine("<trivia=" + value + " />");
            result.AppendLine();
        }
    }
}
