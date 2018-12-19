using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    internal static class CastLineProcessor
    {
        private static readonly Regex MultiLanguageVoiceRegex;
        private static readonly Regex CreditOnlyRegex;
        private static readonly Regex ScenesDeletedRegex;
        private static readonly Regex ArchiveFootageRegex;
        private static readonly Regex LanguageVersionRegex;
        private static readonly Regex VoiceRegex;
        private static readonly Regex UnconfirmedRegex;

        static CastLineProcessor()
        {
            VoiceRegex = new Regex(@"\(voice\)", RegexOptions.Compiled);
            MultiLanguageVoiceRegex = new Regex(@"\(voice: ", RegexOptions.Compiled);
            CreditOnlyRegex = new Regex(@"\(credit only\)", RegexOptions.Compiled);
            ScenesDeletedRegex = new Regex(@"\(scenes{0,1} deleted\)", RegexOptions.Compiled);
            ArchiveFootageRegex = new Regex(@"\(archive footage\)", RegexOptions.Compiled);
            LanguageVersionRegex = new Regex(@"\([A-Za-z]+ version\)", RegexOptions.Compiled);
            UnconfirmedRegex = new Regex(@"(unconfirmed)", RegexOptions.Compiled);
        }

        internal static List<CastInfo> Process(Match match
            , DefaultValues defaultValues)
        {
            List<CastInfo> castList = new List<CastInfo>(2);

            CastInfo castMember = new CastInfo();

            Name name = NameParser.Parse(match.Groups["PersonName"].Value);

            castMember.FirstName = name.FirstName.ToString();
            castMember.MiddleName = name.MiddleName.ToString();
            castMember.LastName = name.LastName.ToString();

            if (match.Groups[ColumnNames.Role].Success)
            {
                String role = String.Empty;

                castMember.Voice = "False";
                castMember.IsBracketVoice = false;
                castMember.Uncredited = "False";
                castMember.CreditedAs = String.Empty;

                role = match.Groups[ColumnNames.Role].Value;
                role = Regex.Replace(role, "<a .+?>", String.Empty, RegexOptions.Compiled);
                role = role.Replace("</a>", String.Empty);
                role = HttpUtility.HtmlDecode(role);

                castMember.OriginalCredit = role;

                Match newMatch = VoiceRegex.Match(role);

                if (newMatch.Success)
                {
                    castMember.Voice = "True";
                    castMember.IsBracketVoice = true;

                    role = role.Replace("(voice)", String.Empty);
                }

                newMatch = MultiLanguageVoiceRegex.Match(role);

                if (newMatch.Success)
                {
                    castMember.Voice = "True";
                    castMember.IsBracketVoice = true;

                    role = role.Replace("(voice: ", "(");
                }

                newMatch = IMDbParser.UncreditedRegex.Match(role);

                if (newMatch.Success)
                {
                    if (defaultValues.IgnoreUncredited)
                    {
                        return (castList);
                    }

                    castMember.Uncredited = "True";

                    role = role.Replace("(uncredited)", String.Empty);
                }

                if (defaultValues.IgnoreCreditOnly)
                {
                    newMatch = CreditOnlyRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return (castList);
                    }
                }

                if (defaultValues.IgnoreScenesDeleted)
                {
                    newMatch = ScenesDeletedRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return (castList);
                    }
                }

                if (defaultValues.IgnoreArchiveFootage)
                {
                    newMatch = ArchiveFootageRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return (castList);
                    }
                }

                if (defaultValues.IgnoreLanguageVersion)
                {
                    newMatch = LanguageVersionRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return (castList);
                    }
                }

                if (defaultValues.IgnoreUnconfirmed)
                {
                    newMatch = UnconfirmedRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return (castList);
                    }
                }

                String personLink = match.Groups["PersonLink"].Value;

                castMember.PersonLink = defaultValues.CheckPersonLinkForRedirect ? IMDbParser.GetUpdatedPersonLink(personLink) : personLink;

                castList.Add(castMember);

                newMatch = IMDbParser.CreditedAsRegex.Match(role);

                if (newMatch.Success)
                {
                    if (defaultValues.RetainCastCreditedAs)
                    {
                        castMember.CreditedAs = newMatch.Groups["CreditedAs"].ToString();
                    }

                    role = role.Replace("(as " + newMatch.Groups["CreditedAs"].ToString() + ")", String.Empty);
                }

                if (defaultValues.ParseRoleSlash)
                {
                    String[] roles = role.Split('/');

                    roles[0] = CheckForVoiceOf(roles[0], castMember, defaultValues);

                    castMember.Role = roles[0].Trim();

                    if (roles.Length > 1)
                    {
                        for (Int32 j = 1; j < roles.Length; j++)
                        {
                            CastInfo additionalCastMember = new CastInfo();

                            castList.Add(additionalCastMember);

                            additionalCastMember.PersonLink = castMember.PersonLink;
                            additionalCastMember.IsAdditionalRow = true;
                            additionalCastMember.FirstName = castMember.FirstName;
                            additionalCastMember.MiddleName = castMember.MiddleName;
                            additionalCastMember.LastName = castMember.LastName;
                            additionalCastMember.BirthYear = castMember.BirthYear;

                            if (castMember.IsBracketVoice)
                            {
                                additionalCastMember.Voice = castMember.Voice;
                            }
                            else
                            {
                                additionalCastMember.Voice = "False";
                            }

                            additionalCastMember.IsBracketVoice = castMember.IsBracketVoice;
                            additionalCastMember.Uncredited = castMember.Uncredited;
                            additionalCastMember.CreditedAs = castMember.CreditedAs;

                            roles[j] = CheckForVoiceOf(roles[j], additionalCastMember, defaultValues);

                            additionalCastMember.Role = roles[j].Trim();
                            additionalCastMember.OriginalCredit = castMember.OriginalCredit;
                        }
                    }
                }
                else
                {
                    role = CheckForVoiceOf(role, castMember, defaultValues);

                    castMember.Role = role.Trim();

                    String[] roles = castMember.Role.Split('/');

                    if (roles.Length > 1)
                    {
                        castMember.Role = String.Empty;

                        for (Int32 j = 0; j < roles.Length - 1; j++)
                        {
                            castMember.Role += roles[j].Trim() + " / ";
                        }

                        castMember.Role += roles[roles.Length - 1];
                    }
                }
            }

            return (castList);
        }

        private static String CheckForVoiceOf(String role
            , CastInfo castMember
            , DefaultValues defaultValues)
        {
            if (defaultValues.ParseVoiceOf)
            {
                if (role.ToLower().Trim().StartsWith("voice of "))
                {
                    castMember.Voice = "True";
                    castMember.IsBracketVoice = false;

                    role = role.Substring(9);
                }
            }

            return (role);
        }
    }
}