using System;
using System.Windows.Input;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal class RelayCommand : ICommand
    {
        private readonly Func<bool> m_CanExecuteCallback;

        private readonly Action m_ExecuteCallback;

        public RelayCommand(Action executeCallback
            , Func<bool> canExecuteCallback = null)
        {
            if (executeCallback == null)
            {
                throw (new ArgumentNullException("executeCallback"));
            }
            m_ExecuteCallback = executeCallback;
            m_CanExecuteCallback = canExecuteCallback;
        }

        #region ICommand Members

        public bool CanExecute(Object parameter)
        {
            if (CanExecuteCallback != null)
            {
                return (CanExecuteCallback());
            }
            else
            {
                return (true);
            }
        }

        public virtual void Execute(Object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecuteCallback();
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (CanExecuteCallback != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (CanExecuteCallback != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        #endregion

        private Func<bool> CanExecuteCallback
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (m_CanExecuteCallback);
            }
        }

        protected Action ExecuteCallback
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (m_ExecuteCallback);
            }
        }
    }
}