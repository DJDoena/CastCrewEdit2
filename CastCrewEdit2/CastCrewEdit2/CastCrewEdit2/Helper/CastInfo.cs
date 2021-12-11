namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using Resources;

    [Serializable]
    public sealed class CastInfo : PersonInfo
    {
        private static int _identifier;

        public int Identifier;

        public string Role;

        public string Voice;

        public bool IsBracketVoice;

        public string Uncredited;

        public string CreditedAs;

        public bool IsAdditionalRow = false;

        static CastInfo()
        {
            _identifier = 0;
        }

        public CastInfo() : base(DataGridViewTexts.Cast)
        {
            Identifier = ++_identifier;
        }

        public CastInfo(int identifier) : base(DataGridViewTexts.Cast)
        {
            Identifier = identifier;
        }
    }
}