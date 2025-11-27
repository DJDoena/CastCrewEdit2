using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class CrewLineProcessor
{
    internal static CrewInfo Process(DefaultValues defaultValues
        , CreditTypeMatch creditTypeMatch
        , CrewMatch crewMatch
        , string credit
        , string originalCredit)
    {
        var name = NameParser.Parse(crewMatch.Name, defaultValues.StandardizeJuniorSenior);

        var crewMember = new CrewInfo()
        {
            FirstName = name.FirstName.ToString(),
            MiddleName = name.MiddleName.ToString(),
            LastName = name.LastName.ToString(),
            OriginalCredit = originalCredit,
        };

        IEnumerable<string> creditParts;
        if (!string.IsNullOrEmpty(credit))
        {
            crewMember.CreditedAs = string.Empty;

            var newMatch = IMDbParser.CreditedAsRegex.Match(credit);

            if (newMatch.Success)
            {
                if (defaultValues.RetainCrewCreditedAs)
                {
                    crewMember.CreditedAs = newMatch.Groups["CreditedAs"].ToString();
                }

                credit = credit.Replace("(as " + newMatch.Groups["CreditedAs"].ToString() + ")", string.Empty);
                credit = credit.Trim();
            }
            else
            {
                if (name.OriginalName != name.PlainName && defaultValues.RetainCastCreditedAs)
                {
                    crewMember.CreditedAs = name.OriginalName;
                }
            }

            creditParts = [.. credit
                .Split(['(', ')'], StringSplitOptions.RemoveEmptyEntries)
                .Where(cp => !cp.StartsWith("segment"))
                .Where(cp => !string.IsNullOrWhiteSpace(cp))
                .Select(CrewParser.CleanupCredit)];
        }
        else
        {
            creditParts = [];
        }

        var typeMatch = false;

        var subtypeMatch = false;

        if (IMDbParser.TransformationData.CreditTypeList != null)
        {
            var useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, creditParts, out subtypeMatch, out typeMatch);

            if (!useCredit)
            {
                return null;
            }
        }

        if (typeMatch)
        {
            if (!subtypeMatch)
            {
                if (!defaultValues.IncludeCustomCredits)
                {
                    return null;
                }

                if (CrewParser.IgnoreCustomInIMDbCreditType.Contains(creditTypeMatch.CreditType))
                {
                    return null;
                }

                crewMember.CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
                crewMember.CustomRole = FormatCredit(creditParts, defaultValues.CapitalizeCustomRole);
            }
        }
        else
        {
            if (!defaultValues.CreditTypeOther)
            {
                return null;
            }

            if (CrewParser.IgnoreIMDbCreditTypeInOther.Contains(creditTypeMatch.CreditType))
            {
                return null;
            }

            crewMember.CreditType = CreditTypesDataGridViewHelper.CreditTypes.Other;
            crewMember.CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;

            if (string.IsNullOrEmpty(credit))
            {
                crewMember.CustomRole = creditTypeMatch.CreditType;
            }
            else
            {
                if (defaultValues.IncludePrefixOnOtherCredits)
                {
                    crewMember.CustomRole = creditTypeMatch.CreditType + ": " + FormatCredit(creditParts, defaultValues.CapitalizeCustomRole);
                }
                else
                {
                    crewMember.CustomRole = FormatCredit(creditParts, defaultValues.CapitalizeCustomRole);
                }
            }
        }

        var personLink = crewMatch.Link;

        PersonLinkParser.CheckPersonLinkForRedirect(defaultValues, Program.CrewCache, crewMember, personLink);

        return crewMember;
    }

    private static string FormatCredit(IEnumerable<string> creditParts, bool capitalizeCustomRole)
    {
        string credit;
        if (capitalizeCustomRole && creditParts.Any(credit => !string.IsNullOrWhiteSpace(credit)))
        {
            var formattedCreditParts = creditParts
                .Where(credit => !string.IsNullOrWhiteSpace(credit))
                .Select(credit => CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(credit));

            credit = string.Join(", ", formattedCreditParts);
        }
        else
        {
            credit = string.Empty;
        }

        return credit;
    }

    private static bool FormatCreditType(DefaultValues defaultValues
        , CreditTypeMatch creditTypeMatch
        , CrewInfo crewMember
        , IEnumerable<string> originalCreditParts
        , out bool subtypeMatch, out bool typeMatch)
    {
        var useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, originalCreditParts, originalCreditParts, out subtypeMatch, out typeMatch);

        if (!subtypeMatch)
        {
            foreach (var originalCredit in originalCreditParts)
            {
                if (originalCredit.Contains(":"))
                {
                    var shortCredit = originalCredit.Split(':')[0].TrimEnd();

                    useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, [originalCredit], [shortCredit], out subtypeMatch, out typeMatch);

                    if (subtypeMatch)
                    {
                        break;
                    }
                }
            }
        }

        return useCredit;
    }

    private static bool FormatCreditType(DefaultValues defaultValues
        , CreditTypeMatch creditTypeMatch
        , CrewInfo crewMember
        , IEnumerable<string> originalCreditParts
        , IEnumerable<string> shortCreditParts
        , out bool subtypeMatch, out bool typeMatch)
    {
        var useCredit = true;

        typeMatch = false;

        subtypeMatch = false;

        foreach (var creditType in IMDbParser.TransformationData.CreditTypeList)
        {
            if (creditType.IMDbCreditType.Equals(creditTypeMatch.CreditType, StringComparison.InvariantCultureIgnoreCase))
            {
                switch (creditType.DVDProfilerCreditType)
                {
                    case CreditTypesDataGridViewHelper.CreditTypes.Direction:
                        {
                            useCredit = defaultValues.CreditTypeDirection;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.Writing:
                        {
                            useCredit = defaultValues.CreditTypeWriting;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.Production:
                        {
                            useCredit = defaultValues.CreditTypeProduction;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.Cinematography:
                        {
                            useCredit = defaultValues.CreditTypeCinematography;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.FilmEditing:
                        {
                            useCredit = defaultValues.CreditTypeFilmEditing;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.Music:
                        {
                            useCredit = defaultValues.CreditTypeMusic;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.Sound:
                        {
                            useCredit = defaultValues.CreditTypeSound;

                            break;
                        }
                    case CreditTypesDataGridViewHelper.CreditTypes.Art:
                        {
                            useCredit = defaultValues.CreditTypeArt;

                            break;
                        }
                }

                if (!useCredit)
                {
                    break;
                }

                crewMember.CreditType = creditType.DVDProfilerCreditType;

                typeMatch = true;

                if (creditType.CreditSubtypeList != null)
                {
                    foreach (var creditSubtype in creditType.CreditSubtypeList)
                    {
                        if (creditSubtype.IMDbCreditSubtype != null)
                        {
                            if (shortCreditParts.Any(shortCredit => creditSubtype.IMDbCreditSubtype.Value.Equals(shortCredit, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                SetCreditSubtype(defaultValues, crewMember, originalCreditParts, shortCreditParts, creditSubtype);

                                subtypeMatch = true;

                                break;
                            }
                            else if (shortCreditParts.Any(shortCredit => StartsWith(creditSubtype.IMDbCreditSubtype.StartsWithSpecified && creditSubtype.IMDbCreditSubtype.StartsWith, shortCredit, creditSubtype.IMDbCreditSubtype.Value)))
                            {
                                SetCreditSubtype(defaultValues, crewMember, originalCreditParts, shortCreditParts, creditSubtype);

                                subtypeMatch = true;

                                break;
                            }
                            else if (!shortCreditParts.Any() && string.IsNullOrEmpty(creditSubtype.IMDbCreditSubtype.Value))
                            {
                                SetCreditSubtype(defaultValues, crewMember, originalCreditParts, shortCreditParts, creditSubtype);

                                subtypeMatch = true;

                                break;
                            }
                        }
                    }

                    if (subtypeMatch)
                    {
                        break;
                    }
                }
            }
        }

        return useCredit;
    }

    internal static bool StartsWith(bool startsWithAllowed
        , string searchText
        , string compareText)
        => startsWithAllowed
            && (searchText.StartsWith(compareText + " ", StringComparison.InvariantCultureIgnoreCase)
            || searchText.StartsWith(compareText + "s", StringComparison.InvariantCultureIgnoreCase));

    private static void SetCreditSubtype(DefaultValues defaultValues
        , CrewInfo crewMember
        , IEnumerable<string> originalCreditParts
        , IEnumerable<string> shortCreditParts
        , CreditSubtype creditSubtype)
    {
        crewMember.CreditSubtype = creditSubtype.DVDProfilerCreditSubtype;

        if (defaultValues.RetainOriginalCredit
            && !shortCreditParts.Any(shortCredit => creditSubtype.DVDProfilerCreditSubtype.Equals(shortCredit, StringComparison.InvariantCultureIgnoreCase)))
        {
            if (originalCreditParts.Any(originalCredit => !string.IsNullOrEmpty(originalCredit)))
            {
                crewMember.CustomRole = FormatCredit(originalCreditParts, defaultValues.CapitalizeCustomRole);
            }
            else if (!string.IsNullOrEmpty(creditSubtype.DVDProfilerCustomRole))
            {
                crewMember.CustomRole = creditSubtype.DVDProfilerCustomRole;
            }
        }
        else if (!string.IsNullOrEmpty(creditSubtype.DVDProfilerCustomRole))
        {
            crewMember.CustomRole = creditSubtype.DVDProfilerCustomRole;
        }
    }
}