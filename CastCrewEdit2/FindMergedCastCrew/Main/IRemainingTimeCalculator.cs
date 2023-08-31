namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IRemainingTimeCalculator
    {
        void Start();

        string Get(int progressValue
            , int progressMax);
    }
}