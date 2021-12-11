namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System.Text.RegularExpressions;

    internal static class SountrackLineProcessor
    {
        internal static CrewInfo Process(string key, Match crewMatch, bool checkPersonLinkForRedirect)
        {
            var name = NameParser.Parse(crewMatch.Groups["PersonName"].Value.ToString());

            var personLink = crewMatch.Groups["PersonLink"].Value;

            var crewMember = new CrewInfo()
            {
                FirstName = name.FirstName.ToString(),
                MiddleName = name.MiddleName.ToString(),
                LastName = name.LastName.ToString(),
                PersonLink = checkPersonLinkForRedirect ? IMDbParser.GetUpdatedPersonLink(personLink) : personLink,
                CreditType = CreditTypesDataGridViewHelper.CreditTypes.Music,
                CreditSubtype = CreditTypesDataGridViewHelper.CreditSubtypes.Custom,
                CustomRole = "Soundtrack",
            };

            if (!string.IsNullOrEmpty(key))
            {
                crewMember.CustomRole = "\"" + key + "\"";
            }

            return crewMember;
        }
    }
}