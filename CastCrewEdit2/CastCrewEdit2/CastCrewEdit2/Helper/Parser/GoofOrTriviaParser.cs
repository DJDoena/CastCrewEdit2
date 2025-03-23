using System.Text;
using DoenaSoft.JsonFragmentParser;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class GoofOrTriviaParser
{
    internal delegate void AddGoofOrTriviaDelegate(string url, string value, StringBuilder result);

    internal static StringBuilder TryAddGoofOrTriviaEntries(string url, JsonRootNode rootNode, AddGoofOrTriviaDelegate addGoofOrTrivia)
    {
        var result = new StringBuilder();

        result.AppendLine("<div style=\"display:none\">");

        foreach (var categoryNode in rootNode)
        {
            TryAddGoofOrTriviaEntries(url, categoryNode["section"], result, addGoofOrTrivia);

            TryAddGoofOrTriviaEntries(url, categoryNode["spoilerSection"], result, addGoofOrTrivia);
        }

        result.AppendLine("</div>");

        return result;
    }

    private static void TryAddGoofOrTriviaEntries(string url, JsonNode sectionNode, StringBuilder results, AddGoofOrTriviaDelegate addGoofOrTrivia)
    {
        if (sectionNode == null)
        {
            return;
        }

        var itemsNode = sectionNode["items"];

        if (itemsNode == null)
        {
            return;
        }

        foreach (var itemNode in itemsNode)
        {
            var value = itemNode["cardHtml"]?.ValueAsString;

            if (!string.IsNullOrEmpty(value))
            {
                addGoofOrTrivia(url, value, results);
            }
        }
    }
}
