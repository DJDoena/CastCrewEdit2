﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal sealed class RelayCommandAsync : RelayCommand
    {
        public RelayCommandAsync(Action executeCallback
            , Func<Boolean> canExecuteCallback = null)
            : base(executeCallback, canExecuteCallback)
        { }

        #region ICommand Members
        public override void Execute(Object parameter)
        {
            if (CanExecute(parameter))
            {
                Task task;

                task = Task.Run(ExecuteCallback);
                task.ContinueWith(t => CommandManager.InvalidateRequerySuggested(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        #endregion
    }
}