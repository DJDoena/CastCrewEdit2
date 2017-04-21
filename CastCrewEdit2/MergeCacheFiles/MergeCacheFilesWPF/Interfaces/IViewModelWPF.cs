using System;
using System.ComponentModel;
using System.Windows.Input;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal interface IViewModelWPF : IViewModel, INotifyPropertyChanged
    {
        ICommand SelectLeftFileCommand { get; }

        ICommand SelectRightFileCommand { get; }

        ICommand SelectTargetFileCommand { get; }

        ICommand MergeCommand { get; }

        ICommand MergeIntoThirdFileCommand { get; }

        ICommand ClearFileNamesCommand { get; }

        Boolean TaskIsRunning { get; }

        Int32 ProgressMax { get; }

        Int32 ProgressValue { get; }

        Boolean ProgressInfinity { get; }
    }
}