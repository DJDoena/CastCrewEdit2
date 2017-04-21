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

        public virtual String LeftFileName
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

        public virtual String RightFileName
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

        public virtual String TargetFileName
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

        public virtual Boolean CanExecuteMerge()
        {
            return ((String.IsNullOrEmpty(LeftFileName) == false)
                && (String.IsNullOrEmpty(RightFileName) == false));
        }

        public virtual Boolean CanExecuteMergeIntoThirdFile()
        {
            return ((CanExecuteMerge())
                && (String.IsNullOrEmpty(TargetFileName) == false));
        }

        public virtual Boolean CanExecuteClearFileNames()
        {
            return ((String.IsNullOrEmpty(LeftFileName) == false)
              || (String.IsNullOrEmpty(RightFileName) == false)
              || (String.IsNullOrEmpty(TargetFileName)) == false);
        }

        public void Save()
        {
            Model.SaveSettings();
        }

        public void SelectLeftFileName()
        {
            String fileName;

            if (ShowOpenFileDialog(LeftFileName, out fileName))
            {
                LeftFileName = fileName;
            }
        }

        public void SelectRightFileName()
        {
            String fileName;

            if (ShowOpenFileDialog(RightFileName, out fileName))
            {
                RightFileName = fileName;
            }
        }

        public void SelectTargetFileName()
        {
            String fileName;

            if (ShowSaveFileDialog(TargetFileName, out fileName))
            {
                TargetFileName = fileName;
            }
        }

        public void ClearFileNames()
        {
            LeftFileName = String.Empty;
            RightFileName = String.Empty;
            TargetFileName = String.Empty;
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

        private Boolean ShowOpenFileDialog(String currentFileName
            , out String fileName)
        {
            OpenFileDialogOptions options;

            options = new OpenFileDialogOptions();
            options.CheckFileExists = true;
            options.Filter = "Cast file|cast*.xml|Crew file|crew*.xml";
            options.InitialDirectory = GetInitialDirectory(currentFileName);
            options.RestoreDirectory = true;
            options.Title = "Please select cache file.";
            return (UIServices.ShowOpenFileDialog(options, out fileName));
        }

        private Boolean ShowSaveFileDialog(String currentFileName
            , out String fileName)
        {
            SaveFileDialogOptions options;

            options = new SaveFileDialogOptions();
            options.AddExtension = true;
            options.DefaultExt = ".xml";
            options.Filter = "Cast file|cast*.xml|Crew file|crew*.xml";
            options.InitialDirectory = GetInitialDirectory(currentFileName);
            options.OverwritePrompt = true;
            options.RestoreDirectory = true;
            options.ValidateNames = true;
            options.Title = "Please select target cache file.";
            return (UIServices.ShowSaveFileDialog(options, out fileName));
        }

        private String GetInitialDirectory(String fileName)
        {
            String iniDir = null;

            if ((String.IsNullOrEmpty(fileName) == false) && (IOServices.File.Exists(fileName)))
            {
                IFileInfo fi = IOServices.GetFileInfo(fileName);

                iniDir = fi.DirectoryName + @"\";
            }

            return (iniDir);
        }
    }
}