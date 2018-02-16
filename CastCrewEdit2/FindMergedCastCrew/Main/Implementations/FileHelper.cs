using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal sealed class FileHelper : IFileHelper
    {
        private readonly IIOServices IOServices;

        private readonly IUIServices UIServices;

        public FileHelper(IIOServices ioServices
            , IUIServices uiServices)
        {
            IOServices = ioServices;
            UIServices = uiServices;
        }

        #region IFileHelper

        public Boolean ShowOpenFileDialog(String suggestedFileName
            , String filter
            , String title
            , out String fileName)
        {
            OpenFileDialogOptions options = new OpenFileDialogOptions();

            options.CheckFileExists = true;
            options.Filter = filter;
            options.InitialFolder = GetInitialDirectory(suggestedFileName);
            options.RestoreFolder = true;
            options.Title = title;
            options.FileName = GetInitialFileName(suggestedFileName, true);

            Boolean result = UIServices.ShowOpenFileDialog(options, out fileName);

            return (result);
        }

        public Boolean ShowSaveFileDialog(String suggestedFileName
            , String filter
            , String title
            , out String fileName)
        {
            SaveFileDialogOptions options = new SaveFileDialogOptions();

            options.AddExtension = true;
            options.DefaultExt = ".xml";
            options.Filter = filter;
            options.InitialFolder = GetInitialDirectory(suggestedFileName);
            options.OverwritePrompt = true;
            options.RestoreFolder = true;
            options.ValidateName = true;
            options.Title = title;
            options.FileName = GetInitialFileName(suggestedFileName, false);

            Boolean result = UIServices.ShowSaveFileDialog(options, out fileName);

            return (result);
        }

        public IProcessDataForSerialization LoadSessionData(String fileName)
        {
            const String filter = "Session cache file|*.cache";

            const String title = "Please select session cache file";

            IProcessDataForSerialization processData = null;

            fileName = SuggestSessionFileName(fileName);

            if (ShowOpenFileDialog(fileName, filter, title, out fileName))
            {
                try
                {
                    processData = TryLoadSessionData(fileName);
                }
                catch (Exception ex)
                {
                    UIServices.ShowMessageBox($"The following error occured during load:{Environment.NewLine}{ex.Message}", "Error", Buttons.OK, Icon.Error);
                }
            }

            return (processData);
        }

        public void SaveSessionData(String fileName
            , IProcessDataForSerialization processData)
        {
            const String filter = "Session cache file|*.cache";

            const String title = "Please select session cache file";

            fileName = SuggestSessionFileName(fileName);

            if (ShowSaveFileDialog(fileName, filter, title, out fileName))
            {
                CommenceSaveSessionData(fileName, processData);
            }
        }

        #endregion

        private String GetInitialDirectory(String fileName)
        {
            String iniDir = null;

            if ((String.IsNullOrEmpty(fileName) == false) && (IOServices.File.Exists(fileName)))
            {
                IFileInfo fi = IOServices.GetFileInfo(fileName);

                iniDir = fi.FolderName + @"\";
            }

            return (iniDir);
        }

        private String GetInitialFileName(String fileName
            , Boolean mustExist)
        {
            String iniFile = null;

            if (String.IsNullOrEmpty(fileName) == false)
            {
                IFileInfo fi = IOServices.GetFileInfo(fileName);

                if ((mustExist == false) || (fi.Exists))
                {
                    iniFile = fi.Name;
                }
            }

            return (iniFile);
        }

        #region SessionData

        private IProcessDataForSerialization TryLoadSessionData(String fileName)
        {
            using (Stream fs = IOServices.GetFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();

                IProcessDataForSerialization processData = (IProcessDataForSerialization)(bf.Deserialize(fs));

                return (processData);
            }
        }

        private void CommenceSaveSessionData(String fileName
            , IProcessDataForSerialization processData)
        {
            using (Stream fs = IOServices.GetFileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(fs, processData);
            }
        }

        private String SuggestSessionFileName(String currentFileName)
        {
            String fileName = String.Empty;

            if (String.IsNullOrEmpty(currentFileName) == false)
            {
                IFileInfo fi = IOServices.GetFileInfo(currentFileName);

                fileName = fi.NameWithoutExtension + ".cache";

                fileName = IOServices.Path.Combine(fi.FolderName, fileName);
            }

            return (fileName);
        }

        #endregion
    }
}