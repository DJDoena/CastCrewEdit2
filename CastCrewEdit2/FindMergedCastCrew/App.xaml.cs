using System;
using System.Windows;
using DoenaSoft.DVDProfiler.FindMergedCastCrew.Implementations;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>    
    public partial class App : Application
    {
        private UInt32 PreviousExecutionState { get; set; }

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            PreviousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);

            IWindowFactory windowFactory = new WindowFactory();

            windowFactory.OpenMainWindow();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (PreviousExecutionState != 0)
            {
                NativeMethods.SetThreadExecutionState(PreviousExecutionState);
            }

            base.OnExit(e);
        }
    }
}