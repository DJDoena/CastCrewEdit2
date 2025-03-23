using System.Diagnostics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

[DebuggerDisplay("{Name}")]
public sealed class CrewMatch
{
    public string Link { get; }

    public string Name { get; }

    public string Credit { get; }

    public bool CreditSuccess { get; }

    public CrewMatch(string link, string name, string credit, bool creditSuccess)
    {
        this.Link = link;
        this.Name = name;
        this.Credit = credit;
        this.CreditSuccess = creditSuccess;
    }
}