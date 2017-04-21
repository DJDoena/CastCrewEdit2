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

        public String Log { get; set; }

        #endregion

        #region IProcessData       

        public HashSet<String> Removals { get; private set; }

        public Dictionary<String, String> Updates { get; private set; }

        public HashSet<String> ProcessedPersons { get; private set; }

        public void Clear()
        {
            Log = null;

            ProcessedPersons = new HashSet<String>();

            Removals = new HashSet<String>();

            Updates = new Dictionary<String, String>();
        }

        #endregion

    }
}