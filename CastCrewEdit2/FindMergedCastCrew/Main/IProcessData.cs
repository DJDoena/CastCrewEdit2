using System;
using System.Collections.Generic;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IProcessData
    {
        HashSet<String> Removals { get; }

        Dictionary<String, String> Updates { get; }

        HashSet<String> ProcessedPersons { get; }

        void Clear();
    }
}