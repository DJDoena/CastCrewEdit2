namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Extended
{
    using System;
    using System.Text.RegularExpressions;

    public sealed class SoundtrackMatch : Tuple<string, Match>
    {
        public const string Performer = "Performed by";

        public string Job => this.Item1;

        public Match CrewMatch => this.Item2;

        public SoundtrackMatch(string job, Match match) : base(job, match)
        {
        }
    }
}