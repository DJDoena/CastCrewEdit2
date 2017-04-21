using System;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IRemainingTimeCalculator
    {
        void Start();

        String Get(Int32 progressValue
            , Int32 progressMax);
    }
}