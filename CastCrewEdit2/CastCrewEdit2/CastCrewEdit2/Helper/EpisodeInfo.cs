using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    internal class EpisodeInfo
    {
        private static Int32 s_Indentifier;

        public Int32 Identifier;

        internal String Link;

        internal String SeasonNumber;

        internal String EpisodeNumber;

        internal String EpisodeName;

        internal List<CastInfo> CastList;

        internal List<CrewInfo> CrewList;

        internal List<Match> CastMatches;

        internal List<KeyValuePair<Match, List<Match>>> CrewMatches;

        internal Dictionary<String, List<Match>> SoundtrackMatches;

        static EpisodeInfo()
        {
            s_Indentifier = Int32.MinValue;
        }

        public EpisodeInfo()
        {
            Identifier = s_Indentifier++;
        }
    }
}