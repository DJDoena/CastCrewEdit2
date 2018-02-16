using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.ToolBox.Commands;
using DoenaSoft.ToolBox.Threading;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations
{
    internal class MainViewModel : IMainViewModel
    {
        #region Readonlies

        private readonly IMainModel Model;

        private readonly IIOServices IOServices;

        private readonly IUIServices UIServices;

        private readonly ILog Log;

        private readonly IWindowFactory WindowFactory;

        private readonly IWindowsProgressBarHandler WindowsProgressBarHandler;

        private readonly IRemainingTimeCalculator RemainingTimeCalculator;

        private readonly IFileHelper FileHelper;

        private readonly ISynchronizer Synchronizer;

        #endregion

        #region Fields

        /// <summary>
        /// for access: only use the property to make sure it's thread-safe
        /// </summary>
        private Boolean m_TaskIsRunning;

        /// <summary>
        /// for access: only use the property to make sure it's thread-safe
        /// </summary>
        private Int32 m_ProgressValue;

        /// <summary>
        /// for access: only use the property to make sure it's thread-safe
        /// </summary>
        private Int32 m_ProgressMax;

        private String m_SourceFile;

        private String m_TargetFile;

        private ICancelableCommand m_ProcessCommand;

        #endregion

        #region Properties

        private IProcessDataForSerialization ProcessData { get; set; }

        #endregion

        #region Constructor

        public MainViewModel(IMainModel model
            , IIOServices ioServices
            , IUIServices uiServices
            , ILog log
            , IWindowFactory windowFactory)
        {
            Model = model;
            IOServices = ioServices;
            UIServices = uiServices;
            Log = log;
            WindowFactory = windowFactory;

            ProcessData = new ProcessData();
            WindowsProgressBarHandler = new WindowsProgressBarHandler();
            RemainingTimeCalculator = new RemainingTimeCalculator();
            FileHelper = new FileHelper(ioServices, uiServices);
            Synchronizer = new Synchronizer(Application.Current.Dispatcher);

            m_SourceFile = String.Empty;
            m_TargetFile = String.Empty;
            m_TaskIsRunning = false;
            m_ProgressValue = 0;
            m_ProgressMax = Int32.MaxValue;

            Model.ProgressMaxChanged += OnModelProgressMaxChanged;
            Model.ProgressValueChanged += OnModelProgressValueChanged;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IMainViewModel

        public ICancelableCommand ProcessCommand
        {
            get
            {
                if (m_ProcessCommand == null)
                {
                    m_ProcessCommand = new CancelableRelayCommandAsync(Process, CanExecuteProcess);
                }

                return (m_ProcessCommand);
            }
        }

        public ICommand PauseCommand
            => (new RelayCommand(PauseProcess, CanExecutePause));

        public Boolean ProgressIndeterminate
            => (Synchronizer.InvokeOnUIThread(() => (m_ProgressMax == Int32.MaxValue)));

        public ICommand LoadSessionDataCommand
            => (new RelayCommand(LoadSessionData, CanExecute));

        public ICommand SaveSessionDataCommand
            => (new RelayCommand(SaveSessionData, CanExecute));

        public Int32 ProgressMax
        {
            get
            {
                return (Synchronizer.InvokeOnUIThread(() => m_ProgressMax));
            }
            private set
            {
                Action action = () =>
                    {
                        if (value != m_ProgressMax)
                        {
                            m_ProgressMax = value;

                            SetWindowsProgressBar();

                            OnPropertyChanged(nameof(ProgressMax));
                            OnPropertyChanged(nameof(ProgressIndeterminate));
                            OnPropertyChanged(nameof(ProgressText));
                        }
                    };

                Synchronizer.InvokeOnUIThread(action);
            }
        }

        public Int32 ProgressValue
        {
            get
            {
                return (Synchronizer.InvokeOnUIThread(() => m_ProgressValue));
            }
            private set
            {
                Action action = () =>
                    {
                        if (value != m_ProgressValue)
                        {
                            m_ProgressValue = value;

                            SetWindowsProgressBar();

                            OnPropertyChanged(nameof(ProgressValue));
                            OnPropertyChanged(nameof(ProgressText));
                        }
                    };

                Synchronizer.InvokeOnUIThread(action);
            }
        }

        public String ProgressText
        {
            get
            {
                Func<String> func = () =>
                    {
                        if ((TaskIsNotRunning) || (ProgressMax == Int32.MaxValue))
                        {
                            return (String.Empty);
                        }

                        String remaining = RemainingTimeCalculator.Get(ProgressValue, ProgressMax);

                        return ($"{ProgressValue:#,##0} / {ProgressMax:#,##0}{remaining}");
                    };

                return (Synchronizer.InvokeOnUIThread(func));
            }
        }

        public ICommand SelectSourceFileCommand
            => (new RelayCommand(SelectSourceFile, CanExecute));

        public ICommand SelectTargetFileCommand
            => (new RelayCommand(SelectTargetFile, CanExecute));

        public String SourceFileName
        {
            get
            {
                return (m_SourceFile);
            }
            private set
            {
                if (value != m_SourceFile)
                {
                    m_SourceFile = value;

                    OnPropertyChanged(nameof(SourceFileName));
                }
            }
        }

        public String TargetFileName
        {
            get
            {
                return (m_TargetFile);
            }
            private set
            {
                if (value != m_TargetFile)
                {
                    m_TargetFile = value;

                    OnPropertyChanged(nameof(TargetFileName));
                }
            }
        }

        public Boolean TaskIsRunning
        {
            get
            {
                return (Synchronizer.InvokeOnUIThread(() => m_TaskIsRunning));
            }
            private set
            {
                Action action = () =>
                    {
                        if (value != m_TaskIsRunning)
                        {
                            m_TaskIsRunning = value;

                            SetWindowsProgressBar();

                            OnPropertyChanged(nameof(TaskIsNotRunning));
                            OnPropertyChanged(nameof(TaskIsRunning));
                            OnPropertyChanged(nameof(ProgressText));
                        }
                    };

                Synchronizer.InvokeOnUIThread(action);
            }
        }

        public Boolean TaskIsNotRunning
            => (TaskIsRunning == false);

        #endregion

        #region CanExecute

        private Boolean CanExecute()
            => (TaskIsNotRunning);

        private Boolean CanExecuteProcess()
            => ((CanExecute())
                && (String.IsNullOrEmpty(SourceFileName) == false)
                && (String.IsNullOrEmpty(TargetFileName) == false));

        private Boolean CanExecutePause()
            => ((TaskIsRunning) && (ProcessCommand.CancellationTokenSource?.IsCancellationRequested == false));

        #endregion

        #region Execute

        #region SelectFile

        private void SelectSourceFile()
        {
            const String filter = "Cast file|cast*.xml|Crew file|crew*.xml";

            const String title = "Please select source cache file";

            String fileName;
            if (FileHelper.ShowOpenFileDialog(SourceFileName, filter, title, out fileName))
            {
                SourceFileName = fileName;
            }
        }

        private void SelectTargetFile()
        {
            const String filter = "Cast file|cast*.xml|Crew file|crew*.xml";

            const String title = "Please select target cache file.";

            String fileName;
            if (FileHelper.ShowSaveFileDialog(TargetFileName, filter, title, out fileName))
            {
                TargetFileName = fileName;
            }
        }

        #endregion

        #region Process

        private void Process(CancellationToken cancellationToken)
        {
            RemainingTimeCalculator.Start();

            ProgressValue = 0;

            ProgressMax = Int32.MaxValue;

            TaskIsRunning = true;

            try
            {
                TryProcess(cancellationToken);
            }
            catch (Exception ex)
            {
                UIServices.ShowMessageBox(ex.Message, "Error", Buttons.OK, Icon.Error);
            }

            TaskIsRunning = false;
        }

        private void TryProcess(CancellationToken cancellationToken)
        {
            if (ClearCachedData())
            {
                Log.Clear();

                ProcessData.Clear();
            }

            Model.Process(SourceFileName, TargetFileName, ProcessData, Log, cancellationToken);

            if (cancellationToken.IsCancellationRequested == false)
            {
                String logFile = GetLogFileName();

                BackupHelper.BackupFile(logFile, IOServices);

                Log.Save(logFile);

                OpenOutputWindow();
            }
        }

        private String GetLogFileName()
        {
            IFileInfo fi = IOServices.GetFileInfo(TargetFileName);

            String logFile = fi.NameWithoutExtension + ".html";

            logFile = IOServices.Path.Combine(fi.FolderName, logFile);

            return (logFile);
        }

        #endregion

        private Boolean ClearCachedData()
            => ((ProcessData.ProcessedPersons.Count == 0)
                || (UIServices.ShowMessageBox("There is cached data from a previous run. Reset (yes) or continue (no)?", "Reset or continue?", Buttons.YesNo, Icon.Question) == Result.Yes));


        #region SessionData

        private void LoadSessionData()
        {
            if (ClearCachedData())
            {
                IProcessDataForSerialization processData = FileHelper.LoadSessionData(SourceFileName);

                if (processData != null)
                {
                    ProcessData = processData;

                    Log.FromString(ProcessData.Log);

                    ProcessData.Log = null;
                }
            }
        }

        private void SaveSessionData()
        {
            ProcessData.Log = Log.ToString();

            FileHelper.SaveSessionData(SourceFileName, ProcessData);

            ProcessData.Log = null;
        }

        #endregion

        private void PauseProcess()
        {
            ProcessCommand.CancellationTokenSource.Cancel();
        }

        private void OpenOutputWindow()
        {
            Synchronizer.InvokeOnUIThread(() => WindowFactory.OpenOutputWindow(Log));
        }

        #endregion

        #region EventHandlers

        private void OnPropertyChanged(String attribute)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attribute));
        }

        private void OnModelProgressValueChanged(Object sender
            , EventArgs<Int32> e)
        {
            ProgressValue = e.Value;
        }

        private void OnModelProgressMaxChanged(Object sender
            , EventArgs<Int32> e)
        {
            if (e.Value >= 0)
            {
                ProgressMax = e.Value;
            }
            else
            {
                ProgressMax = Int32.MaxValue;
            }
        }

        #endregion

        private void SetWindowsProgressBar()
        {
            WindowsProgressBarHandler.Set(TaskIsRunning ? ProgressValue : -1, ProgressMax);
        }
    }
}