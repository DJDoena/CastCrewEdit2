using System;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IProcessDataForSerialization : IProcessData
    {
        String Log { get; set; }
    }
}