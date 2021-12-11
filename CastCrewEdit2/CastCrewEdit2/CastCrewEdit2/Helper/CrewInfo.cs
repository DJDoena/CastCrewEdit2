namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
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
}