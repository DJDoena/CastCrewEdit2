﻿using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class CrewLineProcessor
{
    internal static CrewInfo Process(DefaultValues defaultValues, CreditTypeMatch creditTypeMatch, CrewMatch crewMatch, string credit, string originalCredit)
    {
        var name = NameParser.Parse(crewMatch.Name, defaultValues.StandardizeJuniorSenior);

        var crewMember = new CrewInfo()
        {
            FirstName = name.FirstName.ToString(),
            MiddleName = name.MiddleName.ToString(),
            LastName = name.LastName.ToString(),
            OriginalCredit = originalCredit,
        };

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

            var creditParts = credit
                .Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(cp => !cp.StartsWith("segment"))
                .Where(cp => !string.IsNullOrWhiteSpace(cp));

            credit = string.Join(", ", creditParts);

            credit = credit.Trim();
        }

        var typeMatch = false;

        var subtypeMatch = false;

        if (IMDbParser.TransformationData.CreditTypeList != null)
        {
            var useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, credit, out subtypeMatch, out typeMatch);

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
                crewMember.CustomRole = FormatCredit(credit, defaultValues.CapitalizeCustomRole);
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
                    crewMember.CustomRole = creditTypeMatch.CreditType + ": " + FormatCredit(credit, defaultValues.CapitalizeCustomRole);
                }
                else
                {
                    crewMember.CustomRole = FormatCredit(credit, defaultValues.CapitalizeCustomRole);
                }
            }
        }

        var personLink = crewMatch.Link;

        PersonLinkParser.CheckPersonLinkForRedirect(defaultValues, Program.CrewCache, crewMember, personLink);

        return crewMember;
    }

    private static string FormatCredit(string credit, bool capitalizeCustomRole)
    {
        if (capitalizeCustomRole && !string.IsNullOrEmpty(credit))
        {
            credit = CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(credit);
        }

        return credit;
    }

    private static bool FormatCreditType(DefaultValues defaultValues, CreditTypeMatch creditTypeMatch, CrewInfo crewMember, string originalCredit, out bool subtypeMatch, out bool typeMatch)
    {
        var useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, originalCredit, originalCredit, out subtypeMatch, out typeMatch);

        if (!subtypeMatch && originalCredit.Contains(":"))
        {
            var shortCredit = originalCredit.Split(':')[0].TrimEnd();

            useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, originalCredit, shortCredit, out subtypeMatch, out typeMatch);
        }

        return useCredit;
    }

    private static bool FormatCreditType(DefaultValues defaultValues, CreditTypeMatch creditTypeMatch, CrewInfo crewMember, string originalCredit, string shortCredit, out bool subtypeMatch, out bool typeMatch)
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
                            if (creditSubtype.IMDbCreditSubtype.Value.Equals(shortCredit, StringComparison.InvariantCultureIgnoreCase))
                            {
                                SetCreditSubtype(defaultValues, crewMember, originalCredit, shortCredit, creditSubtype);

                                subtypeMatch = true;

                                break;
                            }
                            else if (creditSubtype.IMDbCreditSubtype.StartsWithSpecified
                                && creditSubtype.IMDbCreditSubtype.StartsWith
                                && shortCredit.StartsWith(creditSubtype.IMDbCreditSubtype.Value + " ", StringComparison.InvariantCultureIgnoreCase))
                            {
                                SetCreditSubtype(defaultValues, crewMember, originalCredit, shortCredit, creditSubtype);

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

    private static void SetCreditSubtype(DefaultValues defaultValues, CrewInfo crewMember, string originalCredit, string shortCredit, CreditSubtype creditSubtype)
    {
        crewMember.CreditSubtype = creditSubtype.DVDProfilerCreditSubtype;

        if (defaultValues.RetainOriginalCredit
            && !creditSubtype.DVDProfilerCreditSubtype.Equals(shortCredit, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!string.IsNullOrEmpty(originalCredit))
            {
                crewMember.CustomRole = FormatCredit(originalCredit, defaultValues.CapitalizeCustomRole);
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