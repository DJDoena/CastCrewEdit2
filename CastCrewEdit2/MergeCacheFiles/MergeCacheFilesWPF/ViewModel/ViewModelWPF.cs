using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal sealed class ViewModelWPF : ViewModel, IViewModelWPF
    {
        // for access only use the property to make sure it's thread-safe
        private Boolean m_TaskIsRunning;

        // for access only use the property to make sure it's thread-safe
        private Int32 m_ProgressValue;

        // for access only use the property to make sure it's thread-safe
        private Int32 m_ProgressMax;

        public ViewModelWPF(IUIServices uiServices
            , IIOServices ioServices
            , IModel model)
            : base(uiServices, ioServices, model)
        {
            model.ProgressMaxChanged += OnModelProgressMaxChanged;
            model.ProgressValueChanged += OnModelProgressValueChanged;
            m_TaskIsRunning = false;
            m_ProgressValue = 0;
            m_ProgressMax = Int32.MaxValue;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IViewModel Members

        public override String LeftFileName
        {
            get
            {
                return (base.LeftFileName);
            }
            protected set
            {
                base.LeftFileName = value;
                OnPropertyChanged("LeftFileName");
            }
        }

        public override String RightFileName
        {
            get
            {
                return (base.RightFileName);
            }
            protected set
            {
                base.RightFileName = value;
                OnPropertyChanged("RightFileName");
            }
        }

        public override String TargetFileName
        {
            get
            {
                return (base.TargetFileName);
            }
            protected set
            {
                base.TargetFileName = value;
                OnPropertyChanged("TargetFileName");
            }
        }

        public override void Merge()
        {
            SetTaskIsRunning();
            base.Merge();
            UnsetTaskIsRunning();
        }

        public override void MergeThirdFile()
        {
            SetTaskIsRunning();
            base.MergeThirdFile();
            UnsetTaskIsRunning();
        }

        public override Boolean CanExecuteMerge()
        {
            return ((CanExecute()) && (base.CanExecuteMerge()));
        }

        public override Boolean CanExecuteMergeIntoThirdFile()
        {
            return ((CanExecute()) && (base.CanExecuteMergeIntoThirdFile()));
        }

        public override Boolean CanExecuteClearFileNames()
        {
            return ((CanExecute()) && (base.CanExecuteClearFileNames()));
        }
        #endregion

        #region IViewModelWPF Members

        public ICommand SelectLeftFileCommand
        {
            get
            {
                return (new RelayCommand(SelectLeftFileName, CanExecute));
            }
        }

        public ICommand SelectRightFileCommand
        {
            get
            {
                return (new RelayCommand(SelectRightFileName, CanExecute));
            }
        }

        public ICommand SelectTargetFileCommand
        {
            get
            {
                return (new RelayCommand(SelectTargetFileName, CanExecute));
            }
        }

        public ICommand MergeCommand
        {
            get
            {
                return (new RelayCommandAsync(Merge, CanExecuteMerge));
            }
        }

        public ICommand MergeIntoThirdFileCommand
        {
            get
            {
                return (new RelayCommandAsync(MergeThirdFile, CanExecuteMergeIntoThirdFile));
            }
        }

        public ICommand ClearFileNamesCommand
        {
            get
            {
                return (new RelayCommand(ClearFileNames, CanExecuteClearFileNames));
            }
        }

        public Boolean TaskIsRunning
        {
            get
            {
                Func<Boolean> func;

                func = delegate()
                    {
                        return (m_TaskIsRunning);
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (func());
                }
                else
                {
                    return (Application.Current.Dispatcher.Invoke<Boolean>(func));
                }
            }
            set
            {
                Action action;

                action = delegate()
                    {
                        m_TaskIsRunning = value;
                        OnPropertyChanged("TaskIsRunning");
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(action);
                }
            }
        }

        public Int32 ProgressMax
        {
            get
            {
                Func<Int32> func;

                func = delegate()
                    {
                        return (m_ProgressMax);
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (func());
                }
                else
                {
                    return (Application.Current.Dispatcher.Invoke<Int32>(func));
                }
            }
            private set
            {
                Action action;

                action = delegate()
                    {
                        m_ProgressMax = value;
                        OnPropertyChanged("ProgressMax");
                        OnPropertyChanged("ProgressInfinity");
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(action);
                }
            }
        }

        public Int32 ProgressValue
        {
            get
            {
                Func<Int32> func;

                func = delegate()
                    {
                        return (m_ProgressValue);
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (func());
                }
                else
                {
                    return (Application.Current.Dispatcher.Invoke<Int32>(func));
                }
            }
            private set
            {
                Action action;

                action = delegate()
                    {
                        m_ProgressValue = value;
                        OnPropertyChanged("ProgressValue");
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(action);
                }
            }
        }

        public Boolean ProgressInfinity
        {
            get
            {
                Func<Boolean> func;

                func = delegate()
                    {
                        return (m_ProgressMax == Int32.MaxValue);
                    };
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (func());
                }
                else
                {
                    return (Application.Current.Dispatcher.Invoke<Boolean>(func));
                }
            }
        }

        #endregion

        private void OnModelProgressValueChanged(Object sender, EventArgs<Int32> e)
        {
            ProgressValue = e.Value;
        }

        private void OnModelProgressMaxChanged(Object sender, EventArgs<Int32> e)
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

        private void OnPropertyChanged(String attribute)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(attribute));
            }
        }

        private void SetTaskIsRunning()
        {
            TaskIsRunning = true;
        }

        public void UnsetTaskIsRunning()
        {
            TaskIsRunning = false;
        }

        private Boolean CanExecute()
        {
            return (TaskIsRunning == false);
        }
    }
}