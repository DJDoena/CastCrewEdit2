using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.JsonFragmentParser;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class JsonCastParser
{
    private const string CastStartNewStyle = "\"id\":\"amzn1.imdb.concept.name_credit_group.7caf7d16-5db9-4f4f-8864-d4c6e711c686\",";

    internal static void Parse(string webSite, ref List<CastMatch> castMatches)
    {
        DebugHelper.FormatJson(webSite);

        using var sr = new StringReader(webSite);

        string cast = null;

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            var indexOf = line.IndexOf(CastStartNewStyle);

            if (indexOf != -1)
            {
                cast = line.Substring(indexOf);

                break;
            }
        }

        if (cast != null)
        {
            castMatches = ParseSegment(cast);
        }
    }

    private static List<CastMatch> ParseSegment(string segment)
    {
        const string StartSegment = "{";

        var indexOfStart = segment.IndexOf(StartSegment);

        if (indexOfStart == -1)
        {
            return null;
        }

        var targetSegment = segment.Substring(indexOfStart);

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

        var matches = new List<CastMatch>();

        foreach (var node in nodes.Where(n => n is not null))
        {
            TryAddCastEntry(matches, node);
        }

        return matches;
    }

    private static void TryAddCastEntry(List<CastMatch> matches, JsonNode node)
    {
        var idNode = node["id"];

        var link = idNode?.ValueAsString;

        if (string.IsNullOrEmpty(link))
        {
            return;
        }

        var nameNode = node["rowTitle"];

        var name = nameNode?.ValueAsString;

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        var characterNodes = node["characters"];

        var roleBuilder = new StringBuilder();

        if (characterNodes?.Any() == true)
        {
            roleBuilder.Append(string.Join(" / ", characterNodes.Where(n => n is not null).Select(n => n.ValueAsString)));
        }

        var attributeText = node["attributes"]?.ValueAsString;

        if (!string.IsNullOrWhiteSpace(attributeText))
        {
            Debug.WriteLine($"Attribute {name}: {attributeText}");

            if (roleBuilder.Length > 0)
            {
                roleBuilder.Append(" ");
            }

            roleBuilder.Append(attributeText);
        }

        matches.Add(new(link, name, roleBuilder.ToString(), true));
    }
}