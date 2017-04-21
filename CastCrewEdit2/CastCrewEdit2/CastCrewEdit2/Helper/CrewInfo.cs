using System;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    [Serializable()]
    public class CrewInfo : PersonInfo
    {
        public String CreditType;

        public String CreditSubtype;

        public String CustomRole;

        public String CreditedAs;

        public CrewInfo()
            : base(DataGridViewTexts.Crew)
        {
        }
    }
}
