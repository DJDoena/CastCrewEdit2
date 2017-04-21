using System;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    public interface IModel
    {
        String LeftFileName { get; set; }

        String RightFileName { get; set; }

        String TargetFileName { get; set; }

        void SaveSettings();

        void Merge();

        void MergeIntoThirdFile();

        event EventHandler<EventArgs<Int32>> ProgressMaxChanged;

        event EventHandler<EventArgs<Int32>> ProgressValueChanged;
    }
}