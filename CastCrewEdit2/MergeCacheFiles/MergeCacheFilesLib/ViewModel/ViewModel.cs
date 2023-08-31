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
                return (Model.LeftFileName);
            }
            protected set
            {
                Model.LeftFileName = value;
            }
        }

        public virtual string RightFileName
        {
            get
            {
                return (Model.RightFileName);
            }
            protected set
            {
                Model.RightFileName = value;
            }
        }

        public virtual string TargetFileName
        {
            get
            {
                return (Model.TargetFileName);
            }
            protected set
            {
                Model.TargetFileName = value;
            }
        }

        public virtual bool CanExecuteMerge()
        {
            return ((string.IsNullOrEmpty(LeftFileName) == false)
                && (string.IsNullOrEmpty(RightFileName) == false));
        }

        public virtual bool CanExecuteMergeIntoThirdFile()
        {
            return ((CanExecuteMerge())
                && (string.IsNullOrEmpty(TargetFileName) == false));
        }

        public virtual bool CanExecuteClearFileNames()
        {
            return ((string.IsNullOrEmpty(LeftFileName) == false)
              || (string.IsNullOrEmpty(RightFileName) == false)
              || (string.IsNullOrEmpty(TargetFileName)) == false);
        }

        public void Save()
        {
            Model.SaveSettings();
        }

        public void SelectLeftFileName()
        {
            string fileName;

            if (ShowOpenFileDialog(LeftFileName, out fileName))
            {
                LeftFileName = fileName;
            }
        }

        public void SelectRightFileName()
        {
            string fileName;

            if (ShowOpenFileDialog(RightFileName, out fileName))
            {
                RightFileName = fileName;
            }
        }

        public void SelectTargetFileName()
        {
            string fileName;

            if (ShowSaveFileDialog(TargetFileName, out fileName))
            {
                TargetFileName = fileName;
            }
        }

        public void ClearFileNames()
        {
            LeftFileName = string.Empty;
            RightFileName = string.Empty;
            TargetFileName = string.Empty;
        }

        public virtual void Merge()
        {
            Model.Merge();
        }

        public virtual void MergeThirdFile()
        {
            Model.MergeIntoThirdFile();
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
            options.InitialFolder = GetInitialDirectory(currentFileName);
            options.RestoreFolder = true;
            options.Title = "Please select cache file.";
            return (UIServices.ShowOpenFileDialog(options, out fileName));
        }

        private bool ShowSaveFileDialog(string currentFileName
            , out string fileName)
        {
            SaveFileDialogOptions options;

            options = new SaveFileDialogOptions();
            options.AddExtension = true;
            options.DefaultExt = ".xml";
            options.Filter = "Cast file|cast*.xml|Crew file|crew*.xml";
            options.InitialFolder = GetInitialDirectory(currentFileName);
            options.OverwritePrompt = true;
            options.RestoreFolder = true;
            options.ValidateName = true;
            options.Title = "Please select target cache file.";
            return (UIServices.ShowSaveFileDialog(options, out fileName));
        }

        private string GetInitialDirectory(string fileName)
        {
            string iniDir = null;

            if ((string.IsNullOrEmpty(fileName) == false) && (IOServices.File.Exists(fileName)))
            {
                var fi = IOServices.GetFileInfo(fileName);

                iniDir = fi.FolderName + @"\";
            }

            return (iniDir);
        }
    }
}