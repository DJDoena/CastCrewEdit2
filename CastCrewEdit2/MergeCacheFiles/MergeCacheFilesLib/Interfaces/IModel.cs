using System;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    public interface IModel
    {
        string LeftFileName { get; set; }

        string RightFileName { get; set; }

        string TargetFileName { get; set; }

        void SaveSettings();

        void Merge();

        void MergeIntoThirdFile();

        event EventHandler<EventArgs<int>> ProgressMaxChanged;

        event EventHandler<EventArgs<int>> ProgressValueChanged;
    }
}