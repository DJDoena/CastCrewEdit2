namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.Collections.Generic;
    using Resources;

    [Serializable]
    public sealed class CrewInfo : PersonInfo
    {
        public string CreditType;

        public string CreditSubtype;

        public string CustomRole;

        public string CreditedAs;

        public CrewInfo() : base(DataGridViewTexts.Crew)
        {
        }
    }

    internal sealed class CrewResult : Tuple<List<CrewInfo>, int>
    {
        public List<CrewInfo> CrewMembers => Item1;

        public int MatchCount => Item2;

        public CrewResult(List<CrewInfo> crewMembers, int matchCount) : base(crewMembers, matchCount)
        {
        }
    }
}