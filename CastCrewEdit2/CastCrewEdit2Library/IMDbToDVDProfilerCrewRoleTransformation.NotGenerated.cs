using System.Diagnostics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{

    [DebuggerDisplay("{IMDbCreditType}")]
    public partial class CreditType
    {
    }

    [DebuggerDisplay("{Value}")]
    public partial class IMDbCreditSubtype
    {
        public IMDbCreditSubtype()
        {
            valueField = string.Empty;
        }
    }
}