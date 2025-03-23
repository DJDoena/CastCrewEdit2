using System.Diagnostics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

[DebuggerDisplay("{Name} ({Role})")]
public sealed class CastMatch
{
    public string Link { get; }

    public string Name { get; }

    public string Role { get; }

    public bool RoleSuccess { get; }

    public CastMatch(string link, string name, string role, bool roleSuccess)
    {
        this.Link = link;
        this.Name = name;
        this.Role = role;
        this.RoleSuccess = roleSuccess;
    }
}