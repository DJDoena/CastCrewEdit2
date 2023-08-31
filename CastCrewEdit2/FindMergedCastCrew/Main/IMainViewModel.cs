using System.ComponentModel;
using System.Windows.Input;
using DoenaSoft.AbstractionLayer.Commands;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IMainViewModel : INotifyPropertyChanged
    {
        string SourceFileName { get; }

        string TargetFileName { get; }

        ICommand SelectSourceFileCommand { get; }

        ICommand SelectTargetFileCommand { get; }

        ICancelableCommand ProcessCommand { get; }

        ICommand PauseCommand { get; }

        ICommand LoadSessionDataCommand { get; }

        ICommand SaveSessionDataCommand { get; }

        bool TaskIsRunning { get; }

        bool TaskIsNotRunning { get; }

        int ProgressMax { get; }

        int ProgressValue { get; }

        string ProgressText { get; }

        bool ProgressIndeterminate { get; }
    }
}