using System;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal sealed class ViewModelForms : ViewModel, IViewModelForms
    {
        public ViewModelForms(IUIServices uiServices
            , IIOServices ioServices
            , IModel model)
            : base(uiServices, ioServices, model)
        {
        }

        #region IViewModel Members

        public override string LeftFileName
        {
            get
            {
                return (base.LeftFileName);
            }
            protected set
            {
                base.LeftFileName = value;
                OnPropertyChanged(LeftFileNameChanged);
            }
        }

        public override string RightFileName
        {
            get
            {
                return (base.RightFileName);
            }
            protected set
            {
                base.RightFileName = value;
                OnPropertyChanged(RightFileNameChanged);
            }
        }

        public override string TargetFileName
        {
            get
            {
                return (base.TargetFileName);
            }
            protected set
            {
                base.TargetFileName = value;
                OnPropertyChanged(TargetFileNameChanged);
            }
        }

        #endregion

        #region IViewModelForms Members

        public event EventHandler LeftFileNameChanged;

        public event EventHandler RightFileNameChanged;

        public event EventHandler TargetFileNameChanged;

        #endregion

        private void OnPropertyChanged(EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }
    }
}