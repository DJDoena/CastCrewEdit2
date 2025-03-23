using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class NameParser
{
    private const int MaxNamePartLength = 35;

    private static List<string> _knownFirstnamePrefixes;

    private static List<string> _knownLastnamePrefixes;

    private static List<string> _knownLastnameSuffixes;

    private static Dictionary<string, Name> _knownNames;

    public static void Initialize(IWin32Window windowHandle, string path)
    {
        IMDbParser.InitList(path + @"KnownFirstnamePrefixes.txt", out _knownFirstnamePrefixes, windowHandle, true);
        IMDbParser.InitList(path + @"KnownLastnamePrefixes.txt", out _knownLastnamePrefixes, windowHandle, true);
        IMDbParser.InitList(path + @"KnownLastnameSuffixes.txt", out _knownLastnameSuffixes, windowHandle, true);

        IMDbParser.InitList(path + @"KnownNames.txt", out var knownNames, windowHandle, false);
        ProcessKnownNames(knownNames);
    }

    public static void InitList(string fileName, FileNameType fileNameType, IWin32Window windowHandle)
    {
        switch (fileNameType)
        {
            case FileNameType.FirstnamePrefixes:
                {
                    IMDbParser.InitList(fileName, out _knownFirstnamePrefixes, windowHandle, true);

                    break;
                }
            case FileNameType.LastnamePrefixes:
                {
                    IMDbParser.InitList(fileName, out _knownLastnamePrefixes, windowHandle, true);

                    break;
                }
            case FileNameType.LastnameSuffixes:
                {
                    IMDbParser.InitList(fileName, out _knownLastnameSuffixes, windowHandle, true);

                    break;
                }
            case FileNameType.KnownNames:
                {
                    IMDbParser.InitList(fileName, out var knownNames, windowHandle, false);

                    ProcessKnownNames(knownNames);

                    break;
                }
        }
    }

    internal static bool IsKnownName(string name) => _knownNames.ContainsKey(name);

    internal static Name Parse(string fullName, bool standardizeJuniorSenior)
    {
        fullName = HttpUtility.HtmlDecode(fullName).Trim();

        if (CheckForUnchangedName(fullName, out var finalName))
        {
            finalName.OriginalName = fullName;

            return finalName;
        }

        finalName = new Name()
        {
            OriginalName = fullName,
        };

        if (Program.DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName)
        {
            if (fullName.Contains("."))
            {
                fullName = BreakInitialsApart(fullName);
                //Now it might have happened that we split "M.D." into "M. D."
                fullName = FindAndRepairBrokenInitials(fullName, _knownLastnameSuffixes);
                fullName = FindAndRepairBrokenInitials(fullName, _knownLastnamePrefixes);
                fullName = FindAndRepairBrokenInitials(fullName, _knownFirstnamePrefixes);
            }
        }

        fullName = CheckForQuotes(fullName, '\'', 0);
        fullName = CheckForQuotes(fullName, '"', 0);

        var nameParts = fullName.Split(' ');

        if (nameParts.Length > 0)
        {
            nameParts[0] = nameParts[0].Replace("#SpacePlaceHolder#", " ");
        }

        if (nameParts.Length == 1)
        {
            finalName.FirstName = new StringBuilder(nameParts[0]);

            return finalName;
        }

        FindStartOfNameParts(nameParts, out var beginOfMiddleName, out var beginOfLastName);

        if (_knownFirstnamePrefixes.Contains(nameParts[0].ToLower()))
        {
            FixStartOfNameParts(nameParts, ref beginOfMiddleName, ref beginOfLastName);
        }

        if (beginOfMiddleName == beginOfLastName)
        {
            beginOfMiddleName = -1;
        }

        if (beginOfMiddleName > 0)
        {
            BuildFirstName(nameParts, beginOfMiddleName, finalName);

            BuildMiddleName(nameParts, beginOfMiddleName, beginOfLastName, finalName);
        }
        else
        {
            BuildFirstName(nameParts, beginOfLastName, finalName);
        }

        BuildLastName(nameParts, beginOfLastName, finalName, standardizeJuniorSenior);

        return finalName;
    }

    private static bool CheckForUnchangedName(string fullName, out Name finalName)
    {
        if (_knownNames.TryGetValue(fullName, out finalName))
        {
            return true;
        }

        var lowered = fullName.ToLower();

        if (lowered.StartsWith("the ") || ((fullName[0] >= '0') && (fullName[0] <= '9')))
        {
            //For bands/performers, e.g. "The Who", "50 Cent"
            finalName = GetStageName(fullName);

            return true;
        }
        else if (lowered.Contains(" the ") || lowered.Contains(" and ") || lowered.Contains(" & "))
        {
            finalName = GetStageName(fullName);

            return true;
        }

        finalName = null;

        return false;
    }

    private static Name GetStageName(string fullName)
    {
        Name result;
        if (fullName.Length <= MaxNamePartLength)
        {
            result = new Name()
            {
                FirstName = new StringBuilder(fullName),
            };
        }
        else if (fullName.Length <= (MaxNamePartLength * 2))
        {
            result = SplitStageNameIntoFirstAndLast(fullName);
        }
        else if (fullName.Length <= (MaxNamePartLength * 3))
        {
            result = SplitStageNameIntoFirstMiddleAndLast(fullName);
        }
        else
        {
            return GetStageName(fullName.Substring(0, MaxNamePartLength * 3));
        }

        return result;
    }

    private static Name SplitStageNameIntoFirstAndLast(string fullName)
    {
        var parts = fullName.Split(' ');

        var firstName = new StringBuilder(parts[0]);

        int currentIndex;
        for (currentIndex = 1; currentIndex < parts.Length; currentIndex++)
        {
            if ((firstName.Length + parts[currentIndex].Length + 1) <= MaxNamePartLength)
            {
                firstName.Append(" " + parts[currentIndex]);
            }
            else
            {
                break;
            }
        }

        var lastName = new StringBuilder();

        if (currentIndex < parts.Length)
        {
            lastName.Append(parts[currentIndex]);

            for (currentIndex = currentIndex + 1; currentIndex < parts.Length; currentIndex++)
            {
                lastName.Append(" " + parts[currentIndex]);
            }
        }

        Name result;
        if (lastName.Length < MaxNamePartLength)
        {
            result = new Name()
            {
                FirstName = firstName,
                LastName = lastName,
            };
        }
        else
        {
            result = SplitStageNameIntoFirstMiddleAndLast(fullName);
        }

        return result;
    }

    private static Name SplitStageNameIntoFirstMiddleAndLast(string fullName)
    {
        var parts = fullName.Split(' ');

        var firstName = new StringBuilder(parts[0]);

        int currentIndex;
        for (currentIndex = 1; currentIndex < parts.Length; currentIndex++)
        {
            if ((firstName.Length + parts[currentIndex].Length + 1) <= MaxNamePartLength)
            {
                firstName.Append(" " + parts[currentIndex]);
            }
            else
            {
                break;
            }
        }

        var middleName = new StringBuilder();

        if (currentIndex < parts.Length)
        {
            middleName.Append(parts[currentIndex]);

            for (currentIndex = currentIndex + 1; currentIndex < parts.Length; currentIndex++)
            {
                middleName.Append(" " + parts[currentIndex]);
            }
        }

        var lastName = new StringBuilder();

        if (currentIndex < parts.Length)
        {
            lastName.Append(parts[currentIndex]);

            for (currentIndex = currentIndex + 1; currentIndex < parts.Length; currentIndex++)
            {
                lastName.Append(" " + parts[currentIndex]);
            }
        }

        if (lastName.Length > MaxNamePartLength)
        {
            lastName = new StringBuilder(lastName.ToString().Substring(0, MaxNamePartLength));
        }

        return new Name()
        {
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
        };
    }

    private static string FindAndRepairBrokenInitials(string fullName, List<string> list)
    {
        foreach (var entry in list)
        {
            if (entry.Contains("."))
            {
                var newEntry = BreakInitialsApart(entry);

                if (newEntry != entry)
                {
                    var indexOf = fullName.IndexOf(newEntry, StringComparison.InvariantCultureIgnoreCase);

                    if (indexOf != -1)
                    {
                        var toBeReplacedEntry = fullName.Substring(indexOf, newEntry.Length);

                        //remove spaces again
                        var replaceEntry = toBeReplacedEntry.Replace(" ", "");

                        fullName = fullName.Replace(toBeReplacedEntry, replaceEntry);
                    }
                }
            }
        }

        return fullName;
    }

    private static string BreakInitialsApart(string name)
    {
        var nameParts = name.Split('.');

        var dotSplitter = new StringBuilder();

        for (var partIndex = 0; partIndex < nameParts.Length - 1; partIndex++)
        {
            dotSplitter.Append(nameParts[partIndex].Trim() + ". ");
        }

        dotSplitter.Append(nameParts[nameParts.Length - 1].Trim());

        name = dotSplitter.ToString().Trim();

        return name;
    }

    private static string CheckForQuotes(string fullName, char unsplittable, int rootIndexOf)
    {
        if (rootIndexOf < fullName.Length)
        {
            var indexOf = fullName.IndexOf(unsplittable, rootIndexOf);

            if (indexOf != -1 && indexOf != fullName.Length - 1)
            {
                var indexOf2 = fullName.IndexOf(unsplittable, indexOf + 1);

                if (indexOf2 != -1)
                {
                    var newName = new StringBuilder();

                    if (indexOf != 0)
                    {
                        newName.Append(fullName.Substring(0, indexOf));
                    }

                    var section = fullName.Substring(indexOf, indexOf2 - indexOf + 1);

                    newName.Append(section.Replace(" ", "#SpacePlaceHolder#"));

                    if (indexOf2 != fullName.Length - 1)
                    {
                        newName.Append(fullName.Substring(indexOf2 + 1, fullName.Length - indexOf2 - 1));
                    }

                    fullName = newName.ToString();

                    indexOf2 = fullName.IndexOf(unsplittable, indexOf + 1);

                    fullName = CheckForQuotes(fullName, unsplittable, indexOf2 + 1);

                    return fullName;
                }
            }
        }

        return fullName;
    }

    private static void ProcessKnownNames(List<string> knownNames)
    {
        _knownNames = new Dictionary<string, Name>(knownNames.Count);

        foreach (var knownName in knownNames)
        {
            var split = knownName.Split(';');

            if (split.Length == 4)
            {
                if (!IsKnownName(split[0]) && !string.IsNullOrEmpty(split[1]))
                {
                    var name = new Name()
                    {
                        FirstName = new StringBuilder(split[1]),
                        MiddleName = new StringBuilder(split[2]),
                        LastName = new StringBuilder(split[3]),
                    };

                    _knownNames.Add(split[0], name);
                }
            }
        }
    }

    private static void FindStartOfNameParts(string[] nameParts, out int beginOfMiddleName, out int beginOfLastName)
    {
        beginOfMiddleName = -1;

        beginOfLastName = -1;

        var canBeSuffix = true;

        var canBePrefix = false;

        for (var namePartIndex = nameParts.Length - 1; namePartIndex >= 1; namePartIndex--)
        {
            nameParts[namePartIndex] = nameParts[namePartIndex].Replace("#SpacePlaceHolder#", " ");

            if (canBeSuffix)
            {
                beginOfLastName = namePartIndex;

                if (!_knownLastnameSuffixes.Contains(nameParts[namePartIndex].ToLower()))
                {
                    canBeSuffix = false;

                    canBePrefix = true;
                }

                continue;
            }

            if (canBePrefix)
            {
                if (_knownLastnamePrefixes.Contains(nameParts[namePartIndex].ToLower()))
                {
                    beginOfLastName = namePartIndex;

                    continue;
                }
            }

            if (namePartIndex > 0 && beginOfLastName > 1)
            {
                beginOfMiddleName = 1;
            }
        }
    }

    private static void FixStartOfNameParts(string[] nameParts, ref int beginOfMiddleName, ref int beginOfLastName)
    {
        for (var namePartIndex = 1; namePartIndex < nameParts.Length; namePartIndex++)
        {
            if (beginOfMiddleName == namePartIndex)
            {
                beginOfMiddleName++;
            }

            if (beginOfLastName == namePartIndex)
            {
                beginOfLastName++;
            }

            if (_knownFirstnamePrefixes.Contains(nameParts[namePartIndex].ToLower()))
            {
                continue;
            }
            else
            {
                break;
            }
        }
    }

    private static void BuildFirstName(string[] nameParts, int beginOfNextNamePart, Name finalName)
    {
        for (var namePartIndex = 0; namePartIndex < beginOfNextNamePart; namePartIndex++)
        {
            finalName.FirstName.Append(" " + nameParts[namePartIndex]);
        }

        finalName.FirstName = new StringBuilder(finalName.FirstName.ToString().Trim());
    }

    private static void BuildMiddleName(string[] nameParts, int beginOfMiddleName, int beginOfLastName, Name finalName)
    {
        for (var namePartIndex = beginOfMiddleName; namePartIndex < beginOfLastName; namePartIndex++)
        {
            finalName.MiddleName.Append(" " + nameParts[namePartIndex]);
        }

        finalName.MiddleName = new StringBuilder(finalName.MiddleName.ToString().Trim());
    }

    private static void BuildLastName(string[] nameParts, int beginOfLastName, Name finalName, bool standardizeJuniorSenior)
    {
        for (var namePartIndex = beginOfLastName; namePartIndex < nameParts.Length; namePartIndex++)
        {
            finalName.LastName.Append(" " + nameParts[namePartIndex]);
        }

        if (standardizeJuniorSenior)
        {
            finalName.LastName = new StringBuilder(StandardizeJuniorSenior(finalName.LastName.ToString().Trim()));
        }

        finalName.LastName = new StringBuilder(finalName.LastName.ToString().Trim());
    }

    private static string StandardizeJuniorSenior(string lastName)
    {
        if (!TryReplaceSuffix(ref lastName, ", jr.", new[] { "jr", "jr.", "jnr", "jnr.", "junior" }))
        {
            TryReplaceSuffix(ref lastName, ", sr.", new[] { "sr", "sr.", "snr", "snr.", "senior" });
        }

        return lastName;
    }

    private static bool TryReplaceSuffix(ref string lastName, string replacement, IEnumerable<string> suffixes)
    {
        var hasReplaced = false;

        foreach (var suffix in suffixes)
        {
            hasReplaced = TryReplaceLastNameSuffix(ref lastName, suffix);

            if (hasReplaced)
            {
                break;
            }
        }

        if (hasReplaced)
        {
            lastName += replacement;
        }

        return hasReplaced;
    }

    private static bool TryReplaceLastNameSuffix(ref string lastName, string suffix)
    {
        if (lastName.EndsWith($", {suffix}", StringComparison.OrdinalIgnoreCase))
        {
            lastName = lastName.Substring(0, lastName.Length - (suffix.Length + 2));

            return true;
        }
        else if (lastName.EndsWith($" {suffix}", StringComparison.OrdinalIgnoreCase))
        {
            lastName = lastName.Substring(0, lastName.Length - (suffix.Length + 1));

            return true;
        }
        else
        {
            return false;
        }
    }
}