using System.Text.RegularExpressions;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

public sealed class SoundtrackMatch
{
    public string Job { get; }

    public bool IsSubtypeMatch { get; }

    public Match CrewMatch { get; }

    public SoundtrackMatch(string job, bool isSubTypeMatch, Match match)
    {
        this.Job = job;
        this.IsSubtypeMatch = isSubTypeMatch;
        this.CrewMatch = match;
    }
}