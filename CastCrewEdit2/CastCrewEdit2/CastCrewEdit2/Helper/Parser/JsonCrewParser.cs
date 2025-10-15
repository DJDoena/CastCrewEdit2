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
    private const string CrewNewStyle = "{\"id\":\"amzn1.imdb.concept.name_credit_category";

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
        DebugHelper.FormatJson(webSite);

        using var sr = new StringReader(webSite);

        var jsonCrewParser = new JsonCrewParser();

        while (sr.Peek() != -1)
        {
            var line = LineReader.ReadLine(sr);

            var previousIndexOf = 0;
            int indexOf;
            do
            {
                indexOf = line.IndexOf(CrewNewStyle, previousIndexOf);

                if (indexOf != -1)
                {
                    var crew = line.Substring(indexOf);

                    var currentCrewMatches = jsonCrewParser.ParseSegment(crew);

                    if (currentCrewMatches?.Any() == true)
                    {
                        crewMatches ??= [];

                        crewMatches.AddRange(currentCrewMatches);
                    }

                    previousIndexOf = indexOf + CrewNewStyle.Length;
                }
            } while (indexOf != -1);
        }
    }

    private List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> ParseSegment(string segment)
    {
        JsonRootNode nodes;
        try
        {
            var cleaned = (new EndOfJsonParser()).GetJson(segment);

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

        var groupCreditTypeNode = nodes["name"];

        var groupCreditType = groupCreditTypeNode?.ValueAsString;

        if (string.IsNullOrEmpty(groupCreditType) || groupCreditType.ToLower() == "cast")
        {
            return null;
        }

        var crewNodes = nodes["section"]?["items"];

        if (crewNodes == null || !crewNodes.Any())
        {
            return null;
        }

        _matches = [];

        foreach (var crewNode in crewNodes.Where(n => n is not null))
        {
            var isCastNode = crewNode["isCast"];

            if (isCastNode?.ValueAsString?.ToLower() == "true")
            {
                continue;
            }

            var creditSubTypeNode = crewNode["attributes"];

            this.ProcessCrewNode(groupCreditType, creditSubTypeNode, crewNode);
        }

        return [.. _matches];
    }

    private void ProcessCrewNode(string groupCreditType
        , JsonNode creditSubTypeNode
        , JsonNode nameNode)
    {
        var link = nameNode["id"]?.ValueAsString;

        if (string.IsNullOrEmpty(link))
        {
            return;
        }

        var name = nameNode["rowTitle"].ValueAsString;

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        this.TryAddCredit(null, groupCreditType, null, link, name, creditSubTypeNode);
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
            crewMatches = [];

            _matches.Add(creditTypeMatch, crewMatches);
        }

        crewMatches.Add(new(link, name, actualCredit, !string.IsNullOrWhiteSpace(actualCredit)));
    }

    private static void ProcessAttributes(string name
        , JsonNode attributeRootNode
        , out List<string> attributes)
    {
        attributes = [];

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

        var indexOfOpenBracket = jobText.IndexOf("(");

        if (indexOfOpenBracket == 0 && jobText.Length >= 4)
        {
            var nextChars = jobText.Substring(indexOfOpenBracket + 1, 3);

            if (nextChars.ToLower() != "as ")
            {
                var indexOfCloseBracket = jobText.IndexOf(")", indexOfOpenBracket + 1);

                if (indexOfCloseBracket >= 0)
                {
                    jobText = jobText.Substring(1, indexOfCloseBracket - 1).Trim();
                }
            }
        }

        if (string.IsNullOrWhiteSpace(jobText))
        {
            return null;
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
        => Equals(searchText, tuple.SubType.IMDbCreditSubtype?.Value, tuple.SubType.IMDbCreditSubtype.StartsWithSpecified && tuple.SubType.IMDbCreditSubtype.StartsWith)
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
        , string compareText
        , bool startsWith = false)
    {
        Trim(ref searchText);

        Trim(ref compareText);

        if (string.Equals(searchText, compareText, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }
        else if (CrewLineProcessor.StartsWith(startsWith, searchText, compareText))
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
            else if (startsWith && searchText.StartsWith(compareText, StringComparison.InvariantCultureIgnoreCase))
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