using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DoenaSoft.AbstractionLayer.Commands;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.Threading;
using DoenaSoft.AbstractionLayer.UI.Contracts;
using DoenaSoft.AbstractionLayer.UIServices;

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
        private bool m_TaskIsRunning;

        /// <summary>
        /// for access: only use the property to make sure it's thread-safe
        /// </summary>
        private int m_ProgressValue;

        /// <summary>
        /// for access: only use the property to make sure it's thread-safe
        /// </summary>
        private int m_ProgressMax;

        private string m_SourceFile;

        private string m_TargetFile;

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

            this.ProcessData = new ProcessData();
            WindowsProgressBarHandler = new WindowsProgressBarHandler();
            RemainingTimeCalculator = new RemainingTimeCalculator();
            FileHelper = new FileHelper(ioServices, uiServices);
            Synchronizer = new Synchronizer(Application.Current.Dispatcher);

            m_SourceFile = string.Empty;
            m_TargetFile = string.Empty;
            m_TaskIsRunning = false;
            m_ProgressValue = 0;
            m_ProgressMax = int.MaxValue;

            Model.ProgressMaxChanged += this.OnModelProgressMaxChanged;
            Model.ProgressValueChanged += this.OnModelProgressValueChanged;
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
                    m_ProcessCommand = new CancelableRelayCommandAsync(this.Process, this.CanExecuteProcess);
                }

                return (m_ProcessCommand);
            }
        }

        public ICommand PauseCommand
            => (new RelayCommand(this.PauseProcess, this.CanExecutePause));

        public bool ProgressIndeterminate
            => (Synchronizer.Invoke(() => (m_ProgressMax == int.MaxValue)));

        public ICommand LoadSessionDataCommand
            => (new RelayCommand(this.LoadSessionData, this.CanExecute));

        public ICommand SaveSessionDataCommand
            => (new RelayCommand(this.SaveSessionData, this.CanExecute));

        public int ProgressMax
        {
            get
            {
                return (Synchronizer.Invoke(() => m_ProgressMax));
            }
            private set
            {
                Action action = () =>
                    {
                        if (value != m_ProgressMax)
                        {
                            m_ProgressMax = value;

                            this.SetWindowsProgressBar();

                            this.OnPropertyChanged(nameof(this.ProgressMax));
                            this.OnPropertyChanged(nameof(this.ProgressIndeterminate));
                            this.OnPropertyChanged(nameof(this.ProgressText));
                        }
                    };

                Synchronizer.Invoke(action);
            }
        }

        public int ProgressValue
        {
            get
            {
                return (Synchronizer.Invoke(() => m_ProgressValue));
            }
            private set
            {
                Action action = () =>
                    {
                        if (value != m_ProgressValue)
                        {
                            m_ProgressValue = value;

                            this.SetWindowsProgressBar();

                            this.OnPropertyChanged(nameof(this.ProgressValue));
                            this.OnPropertyChanged(nameof(this.ProgressText));
                        }
                    };

                Synchronizer.Invoke(action);
            }
        }

        public string ProgressText
        {
            get
            {
                Func<string> func = () =>
                    {
                        if ((this.TaskIsNotRunning) || (this.ProgressMax == int.MaxValue))
                        {
                            return (string.Empty);
                        }

                        var remaining = RemainingTimeCalculator.Get(this.ProgressValue, this.ProgressMax);

                        return ($"{this.ProgressValue:#,##0} / {this.ProgressMax:#,##0}{remaining}");
                    };

                return (Synchronizer.Invoke(func));
            }
        }

        public ICommand SelectSourceFileCommand
            => (new RelayCommand(this.SelectSourceFile, this.CanExecute));

        public ICommand SelectTargetFileCommand
            => (new RelayCommand(this.SelectTargetFile, this.CanExecute));

        public string SourceFileName
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

                    this.OnPropertyChanged(nameof(this.SourceFileName));
                }
            }
        }

        public string TargetFileName
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

                    this.OnPropertyChanged(nameof(this.TargetFileName));
                }
            }
        }

        public bool TaskIsRunning
        {
            get
            {
                return (Synchronizer.Invoke(() => m_TaskIsRunning));
            }
            private set
            {
                Action action = () =>
                    {
                        if (value != m_TaskIsRunning)
                        {
                            m_TaskIsRunning = value;

                            this.SetWindowsProgressBar();

                            this.OnPropertyChanged(nameof(this.TaskIsNotRunning));
                            this.OnPropertyChanged(nameof(this.TaskIsRunning));
                            this.OnPropertyChanged(nameof(this.ProgressText));
                        }
                    };

                Synchronizer.Invoke(action);
            }
        }

        public bool TaskIsNotRunning
            => (this.TaskIsRunning == false);

        #endregion

        #region CanExecute

        private bool CanExecute()
            => (this.TaskIsNotRunning);

        private bool CanExecuteProcess()
            => ((this.CanExecute())
                && (string.IsNullOrEmpty(this.SourceFileName) == false)
                && (string.IsNullOrEmpty(this.TargetFileName) == false));

        private bool CanExecutePause()
            => ((this.TaskIsRunning) && (this.ProcessCommand.CancellationTokenSource?.IsCancellationRequested == false));

        #endregion

        #region Execute

        #region SelectFile

        private void SelectSourceFile()
        {
            const string filter = "Cast file|cast*.xml|Crew file|crew*.xml";

            const string title = "Please select source cache file";

            string fileName;
            if (FileHelper.ShowOpenFileDialog(this.SourceFileName, filter, title, out fileName))
            {
                this.SourceFileName = fileName;
            }
        }

        private void SelectTargetFile()
        {
            const string filter = "Cast file|cast*.xml|Crew file|crew*.xml";

            const string title = "Please select target cache file.";

            string fileName;
            if (FileHelper.ShowSaveFileDialog(this.TargetFileName, filter, title, out fileName))
            {
                this.TargetFileName = fileName;
            }
        }

        #endregion

        #region Process

        private void Process(CancellationToken cancellationToken)
        {
            RemainingTimeCalculator.Start();

            this.ProgressValue = 0;

            this.ProgressMax = int.MaxValue;

            this.TaskIsRunning = true;

            try
            {
                this.TryProcess(cancellationToken);
            }
            catch (Exception ex)
            {
                UIServices.ShowMessageBox(ex.Message, "Error", Buttons.OK, Icon.Error);
            }

            this.TaskIsRunning = false;
        }

        private void TryProcess(CancellationToken cancellationToken)
        {
            if (this.ClearCachedData())
            {
                Log.Clear();

                this.ProcessData.Clear();
            }

            Model.Process(this.SourceFileName, this.TargetFileName, this.ProcessData, Log, cancellationToken);

            if (cancellationToken.IsCancellationRequested == false)
            {
                var logFile = this.GetLogFileName();

                BackupHelper.BackupFile(logFile, IOServices);

                Log.Save(logFile);

                this.OpenOutputWindow();
            }
        }

        private string GetLogFileName()
        {
            var fi = IOServices.GetFileInfo(this.TargetFileName);

            var logFile = fi.NameWithoutExtension + ".html";

            logFile = IOServices.Path.Combine(fi.FolderName, logFile);

            return (logFile);
        }

        #endregion

        private bool ClearCachedData()
            => ((this.ProcessData.ProcessedPersons.Count == 0)
                || (UIServices.ShowMessageBox("There is cached data from a previous run. Reset (yes) or continue (no)?", "Reset or continue?", Buttons.YesNo, Icon.Question) == Result.Yes));


        #region SessionData

        private void LoadSessionData()
        {
            if (this.ClearCachedData())
            {
                var processData = FileHelper.LoadSessionData(this.SourceFileName);

                if (processData != null)
                {
                    this.ProcessData = processData;

                    Log.FromString(this.ProcessData.Log);

                    this.ProcessData.Log = null;
                }
            }
        }

        private void SaveSessionData()
        {
            this.ProcessData.Log = Log.ToString();

            FileHelper.SaveSessionData(this.SourceFileName, this.ProcessData);

            this.ProcessData.Log = null;
        }

        #endregion

        private void PauseProcess()
        {
            this.ProcessCommand.CancellationTokenSource.Cancel();
        }

        private void OpenOutputWindow()
        {
            Synchronizer.Invoke(() => WindowFactory.OpenOutputWindow(Log));
        }

        #endregion

        #region EventHandlers

        private void OnPropertyChanged(string attribute)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(attribute));
        }

        private void OnModelProgressValueChanged(object sender
            , EventArgs<int> e)
        {
            this.ProgressValue = e.Value;
        }

        private void OnModelProgressMaxChanged(object sender
            , EventArgs<int> e)
        {
            if (e.Value >= 0)
            {
                this.ProgressMax = e.Value;
            }
            else
            {
                this.ProgressMax = int.MaxValue;
            }
        }

        #endregion

        private void SetWindowsProgressBar()
        {
            WindowsProgressBarHandler.Set(this.TaskIsRunning ? this.ProgressValue : -1, this.ProgressMax);
        }
    }
}