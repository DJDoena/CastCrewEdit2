﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    internal static class NameParser
    {
        private static List<string> KnownFirstnamePrefixes;
        private static List<string> KnownLastnamePrefixes;
        private static List<string> KnownLastnameSuffixes;
        private static Dictionary<string, Name> KnownNames;

        public static void Initialize(IWin32Window windowHandle, string path)
        {
            IMDbParser.InitList(path + @"KnownFirstnamePrefixes.txt", ref KnownFirstnamePrefixes, windowHandle, true);
            IMDbParser.InitList(path + @"KnownLastnamePrefixes.txt", ref KnownLastnamePrefixes, windowHandle, true);
            IMDbParser.InitList(path + @"KnownLastnameSuffixes.txt", ref KnownLastnameSuffixes, windowHandle, true);

            List<string> temp = null;

            IMDbParser.InitList(path + @"KnownNames.txt", ref temp, windowHandle, false);

            ProcessKnownNames(temp);
        }

        public static void InitList(string fileName, FileNameType fileNameType, IWin32Window windowHandle)
        {
            switch (fileNameType)
            {
                case (FileNameType.FirstnamePrefixes):
                    {
                        IMDbParser.InitList(fileName, ref KnownFirstnamePrefixes, windowHandle, true);

                        break;
                    }
                case (FileNameType.LastnamePrefixes):
                    {
                        IMDbParser.InitList(fileName, ref KnownLastnamePrefixes, windowHandle, true);

                        break;
                    }
                case (FileNameType.LastnameSuffixes):
                    {
                        IMDbParser.InitList(fileName, ref KnownLastnameSuffixes, windowHandle, true);

                        break;
                    }
                case (FileNameType.KnownNames):
                    {
                        List<string> knownNames = null;

                        IMDbParser.InitList(fileName, ref knownNames, windowHandle, false);

                        ProcessKnownNames(knownNames);

                        break;
                    }
            }
        }

        internal static bool IsKnownName(string name)
            => KnownNames.ContainsKey(name);

        internal static Name Parse(string fullName)
        {
            fullName = HttpUtility.HtmlDecode(fullName).Trim();

            if (CheckForUnchangedName(fullName, out var finalName))
            {
                return finalName;
            }

            finalName = new Name();

            if (Program.Settings.DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName)
            {
                if (fullName.Contains("."))
                {
                    fullName = BreakInitialsApart(fullName);
                    //Now it might have happened that we split "M.D." into "M. D."
                    fullName = FindAndRepairBrokenInitials(fullName, KnownLastnameSuffixes);
                    fullName = FindAndRepairBrokenInitials(fullName, KnownLastnamePrefixes);
                    fullName = FindAndRepairBrokenInitials(fullName, KnownFirstnamePrefixes);
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

            if (KnownFirstnamePrefixes.Contains(nameParts[0].ToLower()))
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

            BuildLastName(nameParts, beginOfLastName, finalName);

            return finalName;
        }

        private static bool CheckForUnchangedName(string fullName, out Name finalName)
        {
            if (KnownNames.TryGetValue(fullName, out finalName))
            {
                return true;
            }

            var lowered = fullName.ToLower();

            if (lowered.StartsWith("the ") || ((fullName[0] >= '0') && (fullName[0] <= '9')))
            {//For bands/performers, e.g. "The Who", "50 Cent"

                finalName = new Name()
                {
                    FirstName = new StringBuilder(fullName),
                };

                return true;
            }
            else if (lowered.Contains(" the ") || lowered.Contains(" and ") || lowered.Contains(" & "))
            {
                finalName = new Name()
                {
                    FirstName = new StringBuilder(fullName),
                };

                return true;
            }

            finalName = null;

            return false;
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

            for (Int32 i = 0; i < nameParts.Length - 1; i++)
            {
                dotSplitter.Append(nameParts[i].Trim() + ". ");
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

                if ((indexOf != -1) && (indexOf != fullName.Length - 1))
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
            KnownNames = new Dictionary<string, Name>(knownNames.Count);

            foreach (var knownName in knownNames)
            {
                var split = knownName.Split(';');

                if (split.Length == 4)
                {
                    if ((IsKnownName(split[0]) == false) && (string.IsNullOrEmpty(split[1]) == false))
                    {
                        var name = new Name()
                        {
                            FirstName = new StringBuilder(split[1]),
                            MiddleName = new StringBuilder(split[2]),
                            LastName = new StringBuilder(split[3]),
                        };

                        KnownNames.Add(split[0], name);
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

                    if (KnownLastnameSuffixes.Contains(nameParts[namePartIndex].ToLower()) == false)
                    {
                        canBeSuffix = false;
                        canBePrefix = true;
                    }

                    continue;
                }

                if (canBePrefix)
                {
                    if (KnownLastnamePrefixes.Contains(nameParts[namePartIndex].ToLower()))
                    {
                        beginOfLastName = namePartIndex;

                        continue;
                    }
                }

                if ((namePartIndex > 0) && (beginOfLastName > 1))
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

                if (KnownFirstnamePrefixes.Contains(nameParts[namePartIndex].ToLower()))
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

        private static void BuildLastName(string[] nameParts, int beginOfLastName, Name finalName)
        {
            for (var namePartIndex = beginOfLastName; namePartIndex < nameParts.Length; namePartIndex++)
            {
                finalName.LastName.Append(" " + nameParts[namePartIndex]);
            }

            finalName.LastName = new StringBuilder(finalName.LastName.ToString().Trim());
        }
    }
}