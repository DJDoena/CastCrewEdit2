namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Web;

    internal static class CastLineProcessor
    {
        private static readonly Regex _multiLanguageVoiceRegex;

        private static readonly Regex _creditOnlyRegex;

        private static readonly Regex _scenesDeletedRegex;

        private static readonly Regex _archiveFootageRegex;

        private static readonly Regex _languageVersionRegex;

        private static readonly Regex _voiceRegex;

        private static readonly Regex _unconfirmedRegex;

        static CastLineProcessor()
        {
            _voiceRegex = new Regex(@"\(voice\)", RegexOptions.Compiled);

            _multiLanguageVoiceRegex = new Regex(@"\(voice: ", RegexOptions.Compiled);

            _creditOnlyRegex = new Regex(@"\(credit only\)", RegexOptions.Compiled);

            _scenesDeletedRegex = new Regex(@"\(scenes{0,1} deleted\)", RegexOptions.Compiled);

            _archiveFootageRegex = new Regex(@"\(archive footage\)", RegexOptions.Compiled);

            _languageVersionRegex = new Regex(@"\([A-Za-z]+ version\)", RegexOptions.Compiled);

            _unconfirmedRegex = new Regex(@"(unconfirmed)", RegexOptions.Compiled);
        }

        internal static List<CastInfo> Process(Match match, DefaultValues defaultValues)
        {
            var castList = new List<CastInfo>(2);

            var name = NameParser.Parse(match.Groups["PersonName"].Value, defaultValues.StandardizeJuniorSenior);

            var castMember = new CastInfo
            {
                FirstName = name.FirstName.ToString(),
                MiddleName = name.MiddleName.ToString(),
                LastName = name.LastName.ToString(),
            };

            if (match.Groups[ColumnNames.Role].Success)
            {
                castMember.Voice = "False";
                castMember.IsBracketVoice = false;
                castMember.Uncredited = "False";
                castMember.CreditedAs = string.Empty;

                var role = match.Groups[ColumnNames.Role].Value;

                role = Regex.Replace(role, "<a .+?>", string.Empty, RegexOptions.Compiled);
                role = role.Replace("</a>", string.Empty);
                role = HttpUtility.HtmlDecode(role);

                castMember.OriginalCredit = role;

                var newMatch = _voiceRegex.Match(role);

                if (newMatch.Success)
                {
                    castMember.Voice = "True";
                    castMember.IsBracketVoice = true;

                    role = role.Replace("(voice)", string.Empty);
                }

                newMatch = _multiLanguageVoiceRegex.Match(role);

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
                        return castList;
                    }

                    castMember.Uncredited = "True";

                    role = role.Replace("(uncredited)", string.Empty);
                }

                if (defaultValues.IgnoreCreditOnly)
                {
                    newMatch = _creditOnlyRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return castList;
                    }
                }

                if (defaultValues.IgnoreScenesDeleted)
                {
                    newMatch = _scenesDeletedRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return castList;
                    }
                }

                if (defaultValues.IgnoreArchiveFootage)
                {
                    newMatch = _archiveFootageRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return castList;
                    }
                }

                if (defaultValues.IgnoreLanguageVersion)
                {
                    newMatch = _languageVersionRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return castList;
                    }
                }

                if (defaultValues.IgnoreUnconfirmed)
                {
                    newMatch = _unconfirmedRegex.Match(role);

                    if (newMatch.Success)
                    {
                        return castList;
                    }
                }

                var personLink = match.Groups["PersonLink"].Value;

                castMember.PersonLink = defaultValues.CheckPersonLinkForRedirect
                    ? IMDbParser.GetUpdatedPersonLink(personLink)
                    : personLink;

                castList.Add(castMember);

                newMatch = IMDbParser.CreditedAsRegex.Match(role);

                if (newMatch.Success)
                {
                    if (defaultValues.RetainCastCreditedAs)
                    {
                        castMember.CreditedAs = newMatch.Groups["CreditedAs"].ToString();
                    }

                    role = role.Replace("(as " + newMatch.Groups["CreditedAs"].ToString() + ")", string.Empty);
                }
                else
                {
                    if (name.OriginalName != name.PlainName && defaultValues.RetainCastCreditedAs)
                    {
                        castMember.CreditedAs = name.OriginalName;
                    }
                }

                if (defaultValues.ParseRoleSlash)
                {
                    var roles = role.Split('/');

                    roles[0] = CheckForVoiceOf(roles[0], castMember, defaultValues);

                    castMember.Role = roles[0].Trim();

                    if (roles.Length > 1)
                    {
                        for (var roleIndex = 1; roleIndex < roles.Length; roleIndex++)
                        {
                            var additionalCastMember = new CastInfo()
                            {
                                PersonLink = castMember.PersonLink,
                                IsAdditionalRow = true,
                                FirstName = castMember.FirstName,
                                MiddleName = castMember.MiddleName,
                                LastName = castMember.LastName,
                                BirthYear = castMember.BirthYear,
                                IsBracketVoice = castMember.IsBracketVoice,
                                Uncredited = castMember.Uncredited,
                                CreditedAs = castMember.CreditedAs,
                                OriginalCredit = castMember.OriginalCredit,
                            };

                            roles[roleIndex] = CheckForVoiceOf(roles[roleIndex], additionalCastMember, defaultValues);

                            additionalCastMember.Role = roles[roleIndex].Trim();

                            if (castMember.IsBracketVoice)
                            {
                                additionalCastMember.Voice = castMember.Voice;
                            }
                            else
                            {
                                additionalCastMember.Voice = "False";
                            }

                            castList.Add(additionalCastMember);
                        }
                    }
                }
                else
                {
                    role = CheckForVoiceOf(role, castMember, defaultValues);

                    castMember.Role = role.Trim();

                    var roles = castMember.Role.Split('/');

                    if (roles.Length > 1)
                    {
                        castMember.Role = string.Empty;

                        for (var roleIndex = 0; roleIndex < roles.Length - 1; roleIndex++)
                        {
                            castMember.Role += roles[roleIndex].Trim() + " / ";
                        }

                        castMember.Role += roles[roles.Length - 1];
                    }
                }
            }

            return castList;
        }

        private static string CheckForVoiceOf(string role, CastInfo castMember, DefaultValues defaultValues)
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

            return role;
        }
    }
}