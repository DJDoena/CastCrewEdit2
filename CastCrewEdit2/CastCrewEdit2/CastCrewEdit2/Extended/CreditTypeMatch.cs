using System;
using System.Diagnostics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

[DebuggerDisplay("{CreditType}")]
public sealed class CreditTypeMatch : Tuple<string>
{
    public string CreditType
        => this.Item1;


    public CreditTypeMatch(string creditType) : base(creditType)
    {
    }
}