using System;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    [Serializable()]
    public class CastInfo : PersonInfo
    {
        private static Int32 s_Identifier;

        public Int32 Identifier;

        public String Role;

        public String Voice;

        public Boolean IsBracketVoice;

        public String Uncredited;

        public String CreditedAs;

        public Boolean IsAdditionalRow = false;

        static CastInfo()
        {
            s_Identifier = 0;
        }

        public CastInfo()
            : base(DataGridViewTexts.Cast)
        {
            Identifier = ++s_Identifier;
        }

        public CastInfo(Int32 identifier)
            : base(DataGridViewTexts.Cast)
        {
            Identifier = identifier;
        }
    }
}
