using System;
using System.Text.RegularExpressions;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    internal static class SountrackLineProcessor
    {
        internal static CrewInfo Process(String key
            , Match crewMatch
            , Boolean checkPersonLinkForRedirect)
        {
            CrewInfo crewMember = new CrewInfo();

            Name name = NameParser.Parse(crewMatch.Groups["PersonName"].Value.ToString());

            crewMember.FirstName = name.FirstName.ToString();
            crewMember.MiddleName = name.MiddleName.ToString();
            crewMember.LastName = name.LastName.ToString();

            String personLink = crewMatch.Groups["PersonLink"].Value;

            crewMember.PersonLink = checkPersonLinkForRedirect ? IMDbParser.GetUpdatedPersonLink(personLink) : personLink;

            crewMember.CreditType = CreditTypesDataGridViewHelper.CreditTypes.Music;
            crewMember.CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom;
            crewMember.CustomRole = "Soundtrack";

            if (String.IsNullOrEmpty(key) == false)
            {
                crewMember.CustomRole = "\"" + key + "\"";
            }

            return (crewMember);
        }
    }
}