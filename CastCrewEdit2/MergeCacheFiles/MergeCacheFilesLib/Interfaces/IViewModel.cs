using System;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    public interface IViewModel
    {
        String LeftFileName { get; }

        String RightFileName { get; }

        String TargetFileName { get; }

        Boolean CanExecuteMerge();

        Boolean CanExecuteMergeIntoThirdFile();

        Boolean CanExecuteClearFileNames();

        void Save();

        void SelectLeftFileName();

        void SelectRightFileName();

        void SelectTargetFileName();

        void ClearFileNames();

        void Merge();

        void MergeThirdFile();
    }
}