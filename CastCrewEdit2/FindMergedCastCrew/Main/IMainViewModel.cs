using System;
using System.ComponentModel;
using System.Windows.Input;
using DoenaSoft.ToolBox.Commands;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IMainViewModel : INotifyPropertyChanged
    {
        String SourceFileName { get; }

        String TargetFileName { get; }

        ICommand SelectSourceFileCommand { get; }

        ICommand SelectTargetFileCommand { get; }

        ICancelableCommand ProcessCommand { get; }

        ICommand PauseCommand { get; }

        ICommand LoadSessionDataCommand { get; }

        ICommand SaveSessionDataCommand { get; }

        Boolean TaskIsRunning { get; }

        Boolean TaskIsNotRunning { get; }

        Int32 ProgressMax { get; }

        Int32 ProgressValue { get; }

        String ProgressText { get; }

        Boolean ProgressIndeterminate { get; }
    }
}