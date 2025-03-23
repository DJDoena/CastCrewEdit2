using System.Collections.Generic;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class BirthYearParser
{
    private static readonly object _getBirthYearLock;

    internal static Dictionary<PersonInfo, string> BirthYearCache
       => IMDbParser.SessionData.BirthYearCache;

    internal static Dictionary<string, ushort> ForcedFakeBirthYears { get; private set; }

    static BirthYearParser()
    {
        _getBirthYearLock = new object();
    }

    internal static void ProcessForcedFakeBirthYears(List<string> forcedFakeBirthYears)
    {
        ForcedFakeBirthYears = new Dictionary<string, ushort>(forcedFakeBirthYears.Count);

        foreach (var forcedFakeBirthYear in forcedFakeBirthYears)
        {
            var split = forcedFakeBirthYear.Split(';');

            if (split.Length == 2)
            {
                if (!NameParser.IsKnownName(split[0]) && !string.IsNullOrEmpty(split[1]))
                {
                    if (ushort.TryParse(split[1], out var birthYear))
                    {
                        ForcedFakeBirthYears.Add(split[0], birthYear);
                    }
                }
            }
        }
    }

    internal static void GetBirthYear(PersonInfo person)
    {
        lock (_getBirthYearLock)
        {
            if (BirthYearCache.ContainsKey(person))
            {
                person.BirthYear = BirthYearCache[person];

                return;
            }
        }

        person.BirthYear = BirthYearGetter.Get(person.PersonLink);

        person.BirthYearWasRetrieved = true;

        lock (_getBirthYearLock)
        {
            BirthYearCache[person] = person.BirthYear;
        }
    }
}