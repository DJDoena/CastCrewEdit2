namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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

        internal List<Match> CastMatches;

        internal List<KeyValuePair<Match, List<Match>>> CrewMatches;

        internal Dictionary<string, List<Match>> SoundtrackMatches;

        static EpisodeInfo()
        {
            _indentifier = int.MinValue;
        }

        public EpisodeInfo()
        {
            Identifier = _indentifier++;
        }
    }
}