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

        bool TaskIsRunning { get; }

        int ProgressMax { get; }

        int ProgressValue { get; }

        bool ProgressInfinity { get; }
    }
}