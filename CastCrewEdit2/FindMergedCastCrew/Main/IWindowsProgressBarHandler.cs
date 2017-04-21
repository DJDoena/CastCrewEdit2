using System;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IWindowsProgressBarHandler
    {
        void Set(Int32 value
            , Int32 max);
    }
}