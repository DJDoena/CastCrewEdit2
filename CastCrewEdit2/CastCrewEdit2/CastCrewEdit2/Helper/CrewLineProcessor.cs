using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    internal static class CrewLineProcessor
    {
        internal static CrewInfo Process(DefaultValues defaultValues
            , Match creditTypeMatch
            , Match crewMatch
            , String credit
            , String originalCredit)
        {
            CrewInfo crewMember = new CrewInfo();

            Name name = IMDbParser.ParsePersonName(crewMatch.Groups["PersonName"].Value.ToString());

            crewMember.FirstName = name.FirstName.ToString();
            crewMember.MiddleName = name.MiddleName.ToString();
            crewMember.LastName = name.LastName.ToString();

            crewMember.OriginalCredit = originalCredit;

            if (String.IsNullOrEmpty(credit) == false)
            {
                crewMember.CreditedAs = String.Empty;

                Match newMatch = IMDbParser.CreditedAsRegex.Match(credit);

                if (newMatch.Success)
                {
                    if (defaultValues.RetainCrewCreditedAs)
                    {
                        crewMember.CreditedAs = newMatch.Groups["CreditedAs"].ToString();
                    }

                    credit = credit.Replace("(as " + newMatch.Groups["CreditedAs"].ToString() + ")", String.Empty);
                    credit = credit.Trim();
                }

                if ((credit.StartsWith("(")) && (credit.EndsWith(")")))
                {
                    credit = credit.Trim('(', ')');
                }

                credit = credit.Trim();
            }

            Boolean typeMatch = false;

            Boolean subtypeMatch = false;

            if (IMDbParser.TransformationData.CreditTypeList != null)
            {
                Boolean useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, credit, out subtypeMatch, out typeMatch);

                if (useCredit == false)
                {
                    return (null);
                }
            }

            if (typeMatch)
            {
                if (subtypeMatch == false)
                {
                    if (defaultValues.IncludeCustomCredits == false)
                    {
                        return (null);
                    }

                    if (IMDbParser.IgnoreCustomInIMDbCreditType.Contains(creditTypeMatch.Groups["CreditType"].Value.ToString()))
                    {
                        return (null);
                    }

                    crewMember.CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
                    crewMember.CustomRole = FormatCredit(credit, defaultValues.CapitalizeCustomRole);
                }
            }
            else
            {
                if (defaultValues.CreditTypeOther == false)
                {
                    return (null);
                }

                if (IMDbParser.IgnoreIMDbCreditTypeInOther.Contains(creditTypeMatch.Groups["CreditType"].Value.ToString()))
                {
                    return (null);
                }

                crewMember.CreditType = CreditTypesDataGridViewHelper.CreditTypes.Other;
                crewMember.CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;

                if (String.IsNullOrEmpty(credit))
                {
                    crewMember.CustomRole = creditTypeMatch.Groups["CreditType"].Value.ToString();
                }
                else
                {
                    if (defaultValues.IncludePrefixOnOtherCredits)
                    {
                        crewMember.CustomRole = creditTypeMatch.Groups["CreditType"].Value.ToString() + ": "
                            + FormatCredit(credit, defaultValues.CapitalizeCustomRole);
                    }
                    else
                    {
                        crewMember.CustomRole = FormatCredit(credit, defaultValues.CapitalizeCustomRole);
                    }
                }
            }

            String personLink = crewMatch.Groups["PersonLink"].Value;

            crewMember.PersonLink = defaultValues.CheckPersonLinkForRedirect ? IMDbParser.GetUpdatedPersonLink(personLink) : personLink;

            return (crewMember);
        }

        private static String FormatCredit(String credit, Boolean capitalizeCustomRole)
        {
            if ((capitalizeCustomRole) && (String.IsNullOrEmpty(credit) == false))
            {
                credit = CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(credit);
            }

            return (credit);
        }

        private static Boolean FormatCreditType(DefaultValues defaultValues
            , Match creditTypeMatch
            , CrewInfo crewMember
            , String originalCredit
            , out Boolean subtypeMatch
            , out Boolean typeMatch)
        {
            Boolean useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, originalCredit, originalCredit, out subtypeMatch, out typeMatch);

            if ((subtypeMatch == false) && (originalCredit.Contains(":")))
            {
                String shortCredit = (originalCredit.Split(':'))[0].TrimEnd();

                useCredit = FormatCreditType(defaultValues, creditTypeMatch, crewMember, originalCredit, shortCredit, out subtypeMatch, out typeMatch);
            }

            return (useCredit);
        }

        private static Boolean FormatCreditType(DefaultValues defaultValues
            , Match creditTypeMatch
            , CrewInfo crewMember
            , String originalCredit
            , String shortCredit
            , out Boolean subtypeMatch
            , out Boolean typeMatch)
        {
            Boolean useCredit = true;

            typeMatch = false;

            subtypeMatch = false;

            foreach (CreditType creditType in IMDbParser.TransformationData.CreditTypeList)
            {
                if (creditType.IMDbCreditType.Equals(creditTypeMatch.Groups["CreditType"].Value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    switch (creditType.DVDProfilerCreditType)
                    {
                        case (CreditTypesDataGridViewHelper.CreditTypes.Direction):
                            {
                                useCredit = defaultValues.CreditTypeDirection;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.Writing):
                            {
                                useCredit = defaultValues.CreditTypeWriting;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.Production):
                            {
                                useCredit = defaultValues.CreditTypeProduction;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.Cinematography):
                            {
                                useCredit = defaultValues.CreditTypeCinematography;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.FilmEditing):
                            {
                                useCredit = defaultValues.CreditTypeFilmEditing;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.Music):
                            {
                                useCredit = defaultValues.CreditTypeMusic;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.Sound):
                            {
                                useCredit = defaultValues.CreditTypeSound;

                                break;
                            }
                        case (CreditTypesDataGridViewHelper.CreditTypes.Art):
                            {
                                useCredit = defaultValues.CreditTypeArt;

                                break;
                            }
                    }

                    if (useCredit == false)
                    {
                        break;
                    }

                    crewMember.CreditType = creditType.DVDProfilerCreditType;

                    typeMatch = true;

                    if (creditType.CreditSubtypeList != null)
                    {
                        foreach (CreditSubtype creditSubtype in creditType.CreditSubtypeList)
                        {
                            if (creditSubtype.IMDbCreditSubtype != null)
                            {
                                if (creditSubtype.IMDbCreditSubtype.Value.Equals(shortCredit, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    SetCreditSubtype(defaultValues, crewMember, originalCredit, shortCredit, creditSubtype);

                                    subtypeMatch = true;

                                    break;
                                }
                                else if ((creditSubtype.IMDbCreditSubtype.StartsWithSpecified) && (creditSubtype.IMDbCreditSubtype.StartsWith)
                                    && (shortCredit.StartsWith(creditSubtype.IMDbCreditSubtype.Value + " ", StringComparison.InvariantCultureIgnoreCase)))
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

            return (useCredit);
        }

        private static void SetCreditSubtype(DefaultValues defaultValues
            , CrewInfo crewMember
            , String originalCredit
            , String shortCredit
            , CreditSubtype creditSubtype)
        {
            crewMember.CreditSubtype = creditSubtype.DVDProfilerCreditSubtype;

            if ((defaultValues.RetainOriginalCredit)
                && (creditSubtype.DVDProfilerCreditSubtype.Equals(shortCredit, StringComparison.InvariantCultureIgnoreCase) == false))
            {
                if (String.IsNullOrEmpty(originalCredit) == false)
                {
                    crewMember.CustomRole = FormatCredit(originalCredit, defaultValues.CapitalizeCustomRole);
                }
                else if ((String.IsNullOrEmpty(creditSubtype.DVDProfilerCustomRole) == false))
                {
                    crewMember.CustomRole = creditSubtype.DVDProfilerCustomRole;
                }
            }
            else if ((String.IsNullOrEmpty(creditSubtype.DVDProfilerCustomRole) == false))
            {
                crewMember.CustomRole = creditSubtype.DVDProfilerCustomRole;
            }
        }
    }
}