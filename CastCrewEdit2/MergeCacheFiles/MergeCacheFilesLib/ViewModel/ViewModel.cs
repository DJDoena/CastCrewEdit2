using System;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    public class ViewModel : IViewModel
    {
        private readonly IModel m_Model;

        private readonly IUIServices m_UIServices;

        private readonly IIOServices m_IOServices;

        public ViewModel(IUIServices uiServices
            , IIOServices ioServices
            , IModel model)
        {
            if (uiServices == null)
            {
                throw (new ArgumentNullException("uiServices"));
            }
            if (ioServices == null)
            {
                throw (new ArgumentNullException("ioServices"));
            }
            if (model == null)
            {
                throw (new ArgumentNullException("model"));
            }
            m_UIServices = uiServices;
            m_IOServices = ioServices;
            m_Model = model;
        }

        #region IViewModel Members

        public virtual string LeftFileName
        {
            get
            {
                return (this.Model.LeftFileName);
            }
            protected set
            {
                this.Model.LeftFileName = value;
            }
        }

        public virtual string RightFileName
        {
            get
            {
                return (this.Model.RightFileName);
            }
            protected set
            {
                this.Model.RightFileName = value;
            }
        }

        public virtual string TargetFileName
        {
            get
            {
                return (this.Model.TargetFileName);
            }
            protected set
            {
                this.Model.TargetFileName = value;
            }
        }

        public virtual bool CanExecuteMerge()
        {
            return ((string.IsNullOrEmpty(this.LeftFileName) == false)
                && (string.IsNullOrEmpty(this.RightFileName) == false));
        }

        public virtual bool CanExecuteMergeIntoThirdFile()
        {
            return ((this.CanExecuteMerge())
                && (string.IsNullOrEmpty(this.TargetFileName) == false));
        }

        public virtual bool CanExecuteClearFileNames()
        {
            return ((string.IsNullOrEmpty(this.LeftFileName) == false)
              || (string.IsNullOrEmpty(this.RightFileName) == false)
              || (string.IsNullOrEmpty(this.TargetFileName)) == false);
        }

        public void Save()
        {
            this.Model.SaveSettings();
        }

        public void SelectLeftFileName()
        {
            string fileName;

            if (this.ShowOpenFileDialog(this.LeftFileName, out fileName))
            {
                this.LeftFileName = fileName;
            }
        }

        public void SelectRightFileName()
        {
            string fileName;

            if (this.ShowOpenFileDialog(this.RightFileName, out fileName))
            {
                this.RightFileName = fileName;
            }
        }

        public void SelectTargetFileName()
        {
            string fileName;

            if (this.ShowSaveFileDialog(this.TargetFileName, out fileName))
            {
                this.TargetFileName = fileName;
            }
        }

        public void ClearFileNames()
        {
            this.LeftFileName = string.Empty;
            this.RightFileName = string.Empty;
            this.TargetFileName = string.Empty;
        }

        public virtual void Merge()
        {
            this.Model.Merge();
        }

        public virtual void MergeThirdFile()
        {
            this.Model.MergeIntoThirdFile();
        }

        #endregion

        private IModel Model
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (m_Model);
            }
        }

        private IUIServices UIServices
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (m_UIServices);
            }
        }

        private IIOServices IOServices
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (m_IOServices);
            }
        }

        private bool ShowOpenFileDialog(string currentFileName
            , out string fileName)
        {
            OpenFileDialogOptions options;

            options = new OpenFileDialogOptions();
            options.CheckFileExists = true;
            options.Filter = "Cast file|cast*.xml|Crew file|crew*.xml";
            options.InitialFolder = this.GetInitialDirectory(currentFileName);
            options.RestoreFolder = true;
            options.Title = "Please select cache file.";
            return (this.UIServices.ShowOpenFileDialog(options, out fileName));
        }

        private bool ShowSaveFileDialog(string currentFileName
            , out string fileName)
        {
            SaveFileDialogOptions options;

            options = new SaveFileDialogOptions();
            options.AddExtension = true;
            options.DefaultExt = ".xml";
            options.Filter = "Cast file|cast*.xml|Crew file|crew*.xml";
            options.InitialFolder = this.GetInitialDirectory(currentFileName);
            options.OverwritePrompt = true;
            options.RestoreFolder = true;
            options.ValidateName = true;
            options.Title = "Please select target cache file.";
            return (this.UIServices.ShowSaveFileDialog(options, out fileName));
        }

        private string GetInitialDirectory(string fileName)
        {
            string iniDir = null;

            if ((string.IsNullOrEmpty(fileName) == false) && (this.IOServices.File.Exists(fileName)))
            {
                var fi = this.IOServices.GetFile(fileName);

                iniDir = fi.FolderName + @"\";
            }

            return (iniDir);
        }
    }
}