using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.JsonFragmentParser;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal sealed class JsonCrewParser
{
    private const string CrewNewStyle = "\"creditCategories\":[{\"category\"";

    private readonly List<CreditTypeTuple> _transformationData;

    private Dictionary<CreditTypeMatch, List<CrewMatch>> _matches;

    private JsonCrewParser()
    {
        _transformationData = (IMDbParser.TransformationData?.CreditTypeList ?? Enumerable.Empty<CreditType>())
            .Where(ct => ct?.CreditSubtypeList?.Any() == true)
            .SelectMany(ct => ct.CreditSubtypeList
                .Where(cst => cst != null)
                .Select(cst => new CreditTypeTuple(ct, cst)))
            .ToList();
    }

    internal static void Parse(string webSite, ref List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches)
    {
        using var sr = new StringReader(webSite);

        string crew = null;

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            var indexOf = line.IndexOf(CrewNewStyle);

            if (indexOf != -1)
            {
                crew = line.Substring(indexOf);

                break;
            }
        }

        if (crew != null)
        {
            crewMatches = (new JsonCrewParser()).ParseSegment(crew);
        }
    }

    private List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> ParseSegment(string segment)
    {
        const string StartSegment = "[";

        var indexOfStart = segment.IndexOf(StartSegment);

        if (indexOfStart == -1)
        {
            return null;
        }

        var targetSegment = segment.Substring(indexOfStart);

        const string EndSegment = ",\"fullCredits\":";

        var indexOfEnd = targetSegment.IndexOf(EndSegment);

        if (indexOfEnd != -1)
        {
            targetSegment = targetSegment.Substring(0, indexOfEnd);
        }

        JsonRootNode nodes;
        try
        {
            var cleaned = (new EndOfJsonParser()).GetJson(targetSegment);

            nodes = JsonTreeBuilder.Build(cleaned);
        }
        catch
        {
            return null;
        }

        if (!_transformationData.Any())
        {
            return null;
        }

        _matches = new();

        foreach (var node in nodes.Where(n => n is not null))
        {
            var creditNodes = node?["credits"]?["edges"];

            if (creditNodes == null || !creditNodes.Any())
            {
                continue;
            }

            var groupCreditType = node["category"]?["id"]?.ValueAsString?.Replace("_", " ");

            switch (groupCreditType)
            {
                case "cast":
                case "miscellaneous":
                    {
                        continue;
                    }
            }

            this.ProcessNode(creditNodes, groupCreditType);
        }

        return _matches.ToList();
    }

    private void ProcessNode(JsonNode creditNodes
        , string groupCreditType)
    {
        foreach (var intermediateNode in creditNodes.Where(n => n is not null))
        {
            var creditNode = intermediateNode["node"];

            if (creditNode == null)
            {
                continue;
            }

            var nameNode = creditNode["name"];

            if (nameNode == null)
            {
                continue;
            }

            var personCreditTypNode = creditNode["category"];

            var creditSubTypeNode = creditNode["jobDetails"];

            this.ProcessCrewNode(groupCreditType, personCreditTypNode, creditSubTypeNode, nameNode);
        }
    }

    private void ProcessCrewNode(string groupCreditType
        , JsonNode personCreditTypNode
        , JsonNode creditSubTypeNode
        , JsonNode nameNode)
    {
        var personCreditType = personCreditTypNode?["id"]?.ValueAsString.Replace("_", " ");

        var link = nameNode["id"]?.ValueAsString;

        if (string.IsNullOrEmpty(link))
        {
            return;
        }

        var name = nameNode["nameText"]?["text"]?.ValueAsString;

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (creditSubTypeNode?.Any() == true)
        {
            foreach (var jobDetailsNode in creditSubTypeNode.Where(n => n is not null))
            {
                this.ProcessJobNode(groupCreditType, personCreditType, link, name, jobDetailsNode);
            }
        }
        else
        {
            this.TryAddCredit(null, groupCreditType, personCreditType, link, name, null);
        }
    }

    private void ProcessJobNode(string groupCreditType
        , string personCreditType
        , string link
        , string name
        , JsonNode jobDetailsNode)
    {
        var attributeNodes = jobDetailsNode["attributes"];

        var jobNode = jobDetailsNode["job"];

        string jobText = null;

        if (jobNode != null)
        {
            jobText = jobNode["text"]?.ValueAsString;

            if (!string.IsNullOrWhiteSpace(jobText))
            {
                Debug.WriteLine($"Crew Job {name} {jobText}");
            }
        }

        this.TryAddCredit(jobText, groupCreditType, personCreditType, link, name, attributeNodes);
    }

    private void TryAddCredit(string jobText
        , string groupCreditType
        , string personCreditType
        , string link
        , string name
        , JsonNode attributeNodes)
    {
        ProcessAttributes(name, attributeNodes, out var attributes);

        var actualCredit = this.GetActualCredit(jobText, personCreditType, groupCreditType, attributes);

        if (!this.GetImdbCreditType(jobText, personCreditType, groupCreditType, attributes, out var imdbCreditType))
        {
            imdbCreditType = actualCredit;
        }

        var creditTypeMatch = new CreditTypeMatch(imdbCreditType);

        if (!_matches.TryGetValue(creditTypeMatch, out var crewMatches))
        {
            crewMatches = new();

            _matches.Add(creditTypeMatch, crewMatches);
        }

        crewMatches.Add(new(link, name, actualCredit, !string.IsNullOrWhiteSpace(actualCredit)));
    }

    private static void ProcessAttributes(string name
        , JsonNode attributeRootNode
        , out List<string> attributes)
    {
        attributes = new();

        if (attributeRootNode is null)
        {
            return;
        }

        ProcessAttributes(name, attributeRootNode, attributes);
    }

    private static void ProcessAttributes(string name
        , JsonNode attributeRootNode
        , List<string> attributes)
    {
        if (!string.IsNullOrWhiteSpace(attributeRootNode?.ValueAsString))
        {
            TryAddAttributeText(name, attributes, attributeRootNode.ValueAsString);
        }
        else if (attributeRootNode?.Any() == true)
        {
            foreach (var attributeNode in attributeRootNode.Where(n => n is not null))
            {
                TryAddAttributeText(name, attributes, attributeNode["text"]?.ValueAsString);
            }
        }
    }

    private static void TryAddAttributeText(string name
        , List<string> attributes
        , string attributeText)
    {
        attributeText = attributeText?.Trim();

        if (!string.IsNullOrWhiteSpace(attributeText))
        {
            attributeText = attributeText.TrimStart('(').TrimEnd(')');

            attributeText = $"({attributeText})";

            Debug.WriteLine($"Attribute {name}: {attributeText}");

            attributes.Add(attributeText);
        }
    }

    private CreditType GetImdbJob(string jobText)
    {
        if (string.IsNullOrWhiteSpace(jobText))
        {
            return null;
        }

        jobText = jobText.TrimStart();

        var indexOfColon = jobText.IndexOf(":");

        if (indexOfColon >= 0)
        {
            jobText = jobText.Substring(0, indexOfColon).TrimEnd();
        }

        var indexOfBracket = jobText.IndexOf("(");

        if (indexOfBracket >= 0)
        {
            jobText = jobText.Substring(0, indexOfBracket).TrimEnd();
        }

        var imdbJob = _transformationData.FirstOrDefault(tuple => this.MatchesCreditSubType(tuple, jobText))?.Type;

        imdbJob ??= _transformationData.FirstOrDefault(tuple => this.MatchesCreditType(tuple, jobText))?.Type;

        imdbJob ??= this.GetFromHardMapping(jobText);

        return imdbJob;
    }

    private CreditType FindByCreditType(string searchText)
        => _transformationData.FirstOrDefault(tuple => tuple.Type.IMDbCreditType == searchText)?.Type;

    private bool MatchesCreditSubType(CreditTypeTuple tuple
        , string searchText)
        => Equals(searchText, tuple.SubType.IMDbCreditSubtype?.Value)
            || Equals(searchText, tuple.SubType.DVDProfilerCreditSubtype)
            || Equals(searchText, tuple.SubType.DVDProfilerCustomRole);

    private bool MatchesCreditType(CreditTypeTuple tuple
        , string searchText)
        => Equals(searchText, tuple.Type.IMDbCreditType)
            || Equals(searchText, tuple.Type.DVDProfilerCreditType);

    private CreditType GetFromHardMapping(string jobText)
    {
        CreditType imdbJob;
        switch (jobText?.ToLowerInvariant())
        {
            default:
                {
                    imdbJob = this.FindByCreditType("Other");

                    break;
                }
        }

        return imdbJob;
    }


    private static bool Equals(string searchText
        , string compareText)
    {
        Trim(ref searchText);

        Trim(ref compareText);

        if (string.Equals(searchText, compareText, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }
        else
        {
            Clean(ref searchText);

            Clean(ref compareText);

            if (string.Equals(searchText, compareText, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;

        static void Trim(ref string text)
        {
            text = text?.Trim() ?? string.Empty;
        }

        static void Clean(ref string text)
        {
            text = text.Replace(" ", string.Empty).Replace("-", string.Empty);
        }
    }

    private bool GetImdbCreditType(string jobText
        , string personCreditType
        , string groupCreditType
        , List<string> attributes
        , out string imdbCreditType)
    {
        imdbCreditType = null;

        if (!string.IsNullOrEmpty(jobText))
        {
            imdbCreditType = this.TryFindImdbCreditType(jobText);
        }

        if (string.IsNullOrEmpty(imdbCreditType) && attributes?.Any() == true)
        {
            foreach (var attribute in attributes)
            {
                imdbCreditType = this.TryFindImdbCreditType(attribute);

                if (!string.IsNullOrEmpty(imdbCreditType))
                {
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(imdbCreditType))
        {
            imdbCreditType = this.TryFindImdbCreditType(personCreditType);
        }

        if (string.IsNullOrEmpty(imdbCreditType))
        {
            imdbCreditType = this.TryFindImdbCreditType(groupCreditType);
        }

        return !string.IsNullOrEmpty(imdbCreditType);
    }

    private string TryFindImdbCreditType(string jobText)
    {
        var imdbJob = this.GetImdbJob(jobText);

        return imdbJob?.IMDbCreditType;
    }

    private string GetActualCredit(string jobText
        , string personCreditType
        , string groupCreditType
        , List<string> attributes)
    {
        string actualCredit;

        if (attributes?.Any() == true)
        {
            actualCredit = string.Join(", ", attributes);
        }
        else if (!string.IsNullOrWhiteSpace(jobText))
        {
            actualCredit = jobText;
        }
        else if (!string.IsNullOrWhiteSpace(personCreditType))
        {
            actualCredit = personCreditType;
        }
        else
        {
            actualCredit = groupCreditType ?? string.Empty;
        }

        return actualCredit;
    }

    [DebuggerDisplay("{Type.IMDbCreditType}: {SubType.IMDbCreditSubtype.Value}")]
    private sealed class CreditTypeTuple
    {
        public CreditType Type { get; }

        public CreditSubtype SubType { get; }

        public CreditTypeTuple(CreditType type, CreditSubtype subType)
        {
            this.Type = type;
            this.SubType = subType;
        }
    }
}
