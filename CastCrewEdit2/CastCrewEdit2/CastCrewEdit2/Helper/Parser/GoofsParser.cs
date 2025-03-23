using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DoenaSoft.JsonFragmentParser;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class GoofsParser
{
    private const string GoofsStartNewStyle = "\"categories\":[{\"id\"";

    private static readonly Regex _goofsStartOldStyleRegex;

    private static readonly Regex _goofsLiOldStyleRegex;

    private static readonly Regex _goofSpoilerldStyleRegex;

    static GoofsParser()
    {
        _goofsStartOldStyleRegex = new Regex("<h4 class=\"li_group\">", RegexOptions.Compiled);

        _goofsLiOldStyleRegex = new Regex("<div class=\"sodatext\">(?'Goof'.+?)</div>", RegexOptions.Singleline);

        _goofSpoilerldStyleRegex = new Regex("<h4 class=\"inline\">.+?</h4>(?'Goof'.*)", RegexOptions.Singleline | RegexOptions.Compiled);
    }

    internal static StringBuilder ParseGoofs(string goofsUrl)
    {
        var webSite = IMDbParser.GetWebSite(goofsUrl);

        var goofsResult = ParseGoofsNewStyle(goofsUrl, webSite);

        if (goofsResult == null || goofsResult.Length == 0)
        {
            goofsResult = ParseGoofsOldStyle(goofsUrl, webSite);
        }

        return goofsResult;
    }

    private static StringBuilder ParseGoofsNewStyle(string goofsUrl, string webSite)
    {
        using var sr = new StringReader(webSite);

        string goofs = null;

        while (sr.Peek() != -1)
        {
            var line = sr.ReadLine();

            var indexOf = line.IndexOf(GoofsStartNewStyle);

            if (indexOf != -1)
            {
                goofs = line.Substring(indexOf);

                break;
            }
        }

        if (goofs != null)
        {
            var goofsResult = ParseGoofsNewStyleBySegement(goofsUrl, goofs);

            return goofsResult;
        }

        return null;
    }

    private static StringBuilder ParseGoofsOldStyle(string goofsUrl, string webSite)
    {
        using var sr = new StringReader(webSite);

        var goofsFound = false;

        var goofs = new StringBuilder();

        while (sr.Peek() != -1)
        {
            var line = sr.ReadLine();

            if (!goofsFound)
            {
                var beginMatch = _goofsStartOldStyleRegex.Match(line);

                if (beginMatch.Success)
                {
                    goofsFound = true;

                    continue;
                }
            }

            if (goofsFound)
            {
                goofs.AppendLine(line);
            }
        }

        if (goofs.Length > 0)
        {
            var goofsResult = ParseGoofsOldStyleBySegment(goofs, goofsUrl);

            return goofsResult;
        }

        return null;
    }

    private static StringBuilder ParseGoofsOldStyleBySegment(StringBuilder goofs, string goofsUrl)
    {
        var matches = _goofsLiOldStyleRegex.Matches(goofs.ToString());

        var goofsResult = new StringBuilder();

        goofsResult.AppendLine("<div style=\"display:none\">");

        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                if (match.Groups["Goof"].Success)
                {
                    var goofResult = match.Groups["Goof"].Value.Trim();

                    var spoilerMatch = _goofSpoilerldStyleRegex.Match(goofResult);

                    if (spoilerMatch.Success)
                    {
                        goofResult = spoilerMatch.Groups["Goof"].Value;
                    }

                    AddGoofResult(goofsUrl, goofResult, goofsResult);
                }
            }
        }

        goofsResult.AppendLine("</div>");

        return goofsResult;
    }

    private static StringBuilder ParseGoofsNewStyleBySegement(string goofsUrl, string segment)
    {
        const string StartSegment = "[";

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

        JsonRootNode node;
        try
        {
            var cleaned = (new EndOfJsonParser()).GetJson(targetSegment);

            node = JsonTreeBuilder.Build(cleaned);
        }
        catch
        {
            return null;
        }

        var result = GoofOrTriviaParser.TryAddGoofOrTriviaEntries(goofsUrl, node, AddGoofResult);

        return result;
    }

    private static void AddGoofResult(string url, string value, StringBuilder result)
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Replace("&nbsp;", " ");
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

            result.AppendLine("<goof=" + value + " />");
            result.AppendLine();
        }
    }
}