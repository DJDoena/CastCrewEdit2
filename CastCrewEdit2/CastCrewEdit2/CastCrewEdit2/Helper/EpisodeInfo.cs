using System.Collections.Generic;
using System.Text.RegularExpressions;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;

internal sealed class EpisodeInfo
{
    private static int _indentifier;

    public int Identifier;

    internal string Link;

    internal string SeasonNumber;

    internal string EpisodeNumber;

    internal string EpisodeName;

    internal List<CastInfo> CastList;

    internal List<CrewInfo> CrewList;

    internal List<CastMatch> CastMatches;

    internal List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> CrewMatches;

    internal Dictionary<string, List<SoundtrackMatch>> SoundtrackMatches;

    static EpisodeInfo()
    {
        _indentifier = int.MinValue;
    }

    public EpisodeInfo()
    {
        Identifier = _indentifier++;
    }
}