namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    public interface IViewModel
    {
        string LeftFileName { get; }

        string RightFileName { get; }

        string TargetFileName { get; }

        bool CanExecuteMerge();

        bool CanExecuteMergeIntoThirdFile();

        bool CanExecuteClearFileNames();

        void Save();

        void SelectLeftFileName();

        void SelectRightFileName();

        void SelectTargetFileName();

        void ClearFileNames();

        void Merge();

        void MergeThirdFile();
    }
}