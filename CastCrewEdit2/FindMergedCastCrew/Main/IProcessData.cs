using System.Collections.Generic;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IProcessData
    {
        HashSet<string> Removals { get; }

        Dictionary<string, string> Updates { get; }

        HashSet<string> ProcessedPersons { get; }

        void Clear();
    }
}