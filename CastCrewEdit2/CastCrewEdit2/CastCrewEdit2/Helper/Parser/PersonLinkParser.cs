using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class PersonLinkParser
{
    internal const string PersonUrl = IMDbParser.BaseUrl + @"/name/";

    private static readonly Regex _personUrlRegex;

    private static readonly object _updatedPersonLock;

    private static readonly Dictionary<string, string> _updatedPersonLinks;

    static PersonLinkParser()
    {
        _updatedPersonLock = new object();

        _personUrlRegex = new Regex(IMDbParser.DomainPrefix + "name/(?'PersonLink'nm[0-9]+)/.*$", RegexOptions.Compiled);

        _updatedPersonLinks = new Dictionary<string, string>();

    }

    internal static string GetUpdatedPersonLink(string personLink)
    {
        lock (_updatedPersonLock)
        {
            if (_updatedPersonLinks.TryGetValue(personLink, out var newPersonLink))
            {
                return newPersonLink;
            }
        }

        using var response = IMDbParser.GetWebResponse(PersonUrl + personLink + "/");

        var responseUri = response.ResponseUri.AbsoluteUri;

        var match = _personUrlRegex.Match(responseUri);

        if (match.Success)
        {
            var newPersonLink = match.Groups["PersonLink"].Value;

            lock (_updatedPersonLock)
            {
                _updatedPersonLinks[personLink] = newPersonLink;
            }

            return newPersonLink;
        }

        return personLink;
    }

    internal static void CheckPersonLinkForRedirect(DefaultValues defaultValues, Dictionary<string, PersonInfo> personCache, PersonInfo castOrCrew, string personLink)
    {
        castOrCrew.PersonLink = personLink;

        if (defaultValues.CheckPersonLinkForRedirect)
        {
            var now = DateTime.UtcNow;

            if (personCache.TryGetValue(personLink, out var person))
            {
                if (person.LastLinkCheck < now.AddDays(-28)) //4 weeks
                {
                    castOrCrew.PersonLink = GetUpdatedPersonLink(personLink);
                    castOrCrew.LastLinkCheck = now;

                    person.LastLinkCheck = now;
                }
            }
            else
            {
                castOrCrew.PersonLink = GetUpdatedPersonLink(personLink);
                castOrCrew.LastLinkCheck = now;
            }
        }
    }
}
