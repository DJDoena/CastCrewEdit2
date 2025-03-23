namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser
{
    using System.Collections.Generic;
    using Extended;

    internal static class SountrackLineProcessor
    {
        internal static CrewResult Process(string song, List<SoundtrackMatch> songMatches, DefaultValues defaultValues)
        {
            var crewList = new List<CrewInfo>(songMatches.Count);

            if (songMatches.Count > 0)
            {
                var needsDivider = defaultValues.GroupSoundtrackCredits && !string.IsNullOrWhiteSpace(song);

                if (needsDivider)
                {
                    crewList.Add(new CrewInfo()
                    {
                        FirstName = FirstNames.GroupDividerStart,
                        MiddleName = song,
                        LastName = string.Empty,
                        CreditType = CreditTypesDataGridViewHelper.CreditTypes.Music,
                        CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom,
                    });
                }

                for (var songMatchIndex = 0; songMatchIndex < songMatches.Count; songMatchIndex++)
                {
                    var crewMember = Process(song, songMatches[songMatchIndex], defaultValues);

                    if (crewMember != null)
                    {
                        crewList.Add(crewMember);
                    }
                }

                if (needsDivider)
                {
                    crewList.Add(new CrewInfo()
                    {
                        FirstName = FirstNames.GroupDividerEnd,
                        MiddleName = string.Empty,
                        LastName = string.Empty,
                        CreditType = CreditTypesDataGridViewHelper.CreditTypes.Music,
                        CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom,
                    });
                }
            }

            return new CrewResult(crewList, songMatches.Count);
        }

        private static CrewInfo Process(string song, SoundtrackMatch soundtrackMatch, DefaultValues defaultValues)
        {
            var job = soundtrackMatch.Job;

            var crewMatch = soundtrackMatch.CrewMatch;

            var name = NameParser.Parse(crewMatch.Groups["PersonName"].Value, defaultValues.StandardizeJuniorSenior);

            var personLink = crewMatch.Groups["PersonLink"].Value;

            string creditSubType;
            if (soundtrackMatch.IsSubtypeMatch)
            {
                creditSubType = job;

                job = null;
            }
            else
            {
                if (!defaultValues.IncludeCustomSoundtackCredits)
                {
                    return null;
                }

                creditSubType = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
            }

            var crewMember = new CrewInfo()
            {
                FirstName = name.FirstName.ToString(),
                MiddleName = name.MiddleName.ToString(),
                LastName = name.LastName.ToString(),
                PersonLink = defaultValues.CheckPersonLinkForRedirect ? PersonLinkParser.GetUpdatedPersonLink(personLink) : personLink,
                CreditType = CreditTypesDataGridViewHelper.CreditTypes.Music,
                CreditSubtype = creditSubType,
            };

            if (!defaultValues.GroupSoundtrackCredits || string.IsNullOrWhiteSpace(song))
            {
                if (!string.IsNullOrWhiteSpace(song))
                {
                    crewMember.CustomRole = "\"" + song + "\"";
                }
                else
                {
                    crewMember.CustomRole = "Soundtrack";
                }

                if (!string.IsNullOrEmpty(job))
                {
                    crewMember.CustomRole = $"{crewMember.CustomRole} ({job})";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(job))
                {
                    crewMember.CustomRole = job;
                }
            }

            if (name.OriginalName != name.PlainName && defaultValues.RetainCastCreditedAs)
            {
                crewMember.CreditedAs = name.OriginalName;
            }

            return crewMember;
        }


    }
}