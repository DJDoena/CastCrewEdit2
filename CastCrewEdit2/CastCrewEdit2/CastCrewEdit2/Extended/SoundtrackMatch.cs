namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System;
    using System.Text.RegularExpressions;

    public sealed class SoundtrackMatch : Tuple<string, bool, Match>
    {
        public string Job => this.Item1;

        public bool IsSubtypeMatch => this.Item2;

        public Match CrewMatch => this.Item3;

        public SoundtrackMatch(string job, bool isSubTypeMatch, Match match) : base(job, isSubTypeMatch, match)
        {
        }
    }
}