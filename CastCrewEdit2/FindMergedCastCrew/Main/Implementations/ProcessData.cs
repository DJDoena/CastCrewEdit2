using System;
using System.Collections.Generic;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    [Serializable]
    internal sealed class ProcessData : IProcessDataForSerialization
    {
        public ProcessData()
        {
            Clear();
        }

        #region IProcessDataForSerialization

        public string Log { get; set; }

        #endregion

        #region IProcessData       

        public HashSet<string> Removals { get; private set; }

        public Dictionary<string, string> Updates { get; private set; }

        public HashSet<string> ProcessedPersons { get; private set; }

        public void Clear()
        {
            Log = null;

            ProcessedPersons = new HashSet<string>();

            Removals = new HashSet<string>();

            Updates = new Dictionary<string, string>();
        }

        #endregion

    }
}