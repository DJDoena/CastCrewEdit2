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

        public bool ShowOpenFileDialog(string suggestedFileName
            , string filter
            , string title
            , out string fileName)
        {
            var options = new OpenFileDialogOptions();

            options.CheckFileExists = true;
            options.Filter = filter;
            options.InitialFolder = this.GetInitialDirectory(suggestedFileName);
            options.RestoreFolder = true;
            options.Title = title;
            options.FileName = this.GetInitialFileName(suggestedFileName, true);

            var result = UIServices.ShowOpenFileDialog(options, out fileName);

            return (result);
        }

        public bool ShowSaveFileDialog(string suggestedFileName
            , string filter
            , string title
            , out string fileName)
        {
            var options = new SaveFileDialogOptions();

            options.AddExtension = true;
            options.DefaultExt = ".xml";
            options.Filter = filter;
            options.InitialFolder = this.GetInitialDirectory(suggestedFileName);
            options.OverwritePrompt = true;
            options.RestoreFolder = true;
            options.ValidateName = true;
            options.Title = title;
            options.FileName = this.GetInitialFileName(suggestedFileName, false);

            var result = UIServices.ShowSaveFileDialog(options, out fileName);

            return (result);
        }

        public IProcessDataForSerialization LoadSessionData(string fileName)
        {
            const string filter = "Session cache file|*.cache";

            const string title = "Please select session cache file";

            IProcessDataForSerialization processData = null;

            fileName = this.SuggestSessionFileName(fileName);

            if (this.ShowOpenFileDialog(fileName, filter, title, out fileName))
            {
                try
                {
                    processData = this.TryLoadSessionData(fileName);
                }
                catch (Exception ex)
                {
                    UIServices.ShowMessageBox($"The following error occured during load:{Environment.NewLine}{ex.Message}", "Error", Buttons.OK, Icon.Error);
                }
            }

            return (processData);
        }

        public void SaveSessionData(string fileName
            , IProcessDataForSerialization processData)
        {
            const string filter = "Session cache file|*.cache";

            const string title = "Please select session cache file";

            fileName = this.SuggestSessionFileName(fileName);

            if (this.ShowSaveFileDialog(fileName, filter, title, out fileName))
            {
                this.CommenceSaveSessionData(fileName, processData);
            }
        }

        #endregion

        private string GetInitialDirectory(string fileName)
        {
            string iniDir = null;

            if ((string.IsNullOrEmpty(fileName) == false) && (IOServices.File.Exists(fileName)))
            {
                var fi = IOServices.GetFile(fileName);

                iniDir = fi.FolderName + @"\";
            }

            return (iniDir);
        }

        private string GetInitialFileName(string fileName
            , bool mustExist)
        {
            string iniFile = null;

            if (string.IsNullOrEmpty(fileName) == false)
            {
                var fi = IOServices.GetFile(fileName);

                if ((mustExist == false) || (fi.Exists))
                {
                    iniFile = fi.Name;
                }
            }

            return (iniFile);
        }

        #region SessionData

        private IProcessDataForSerialization TryLoadSessionData(string fileName)
        {
            using (var fs = IOServices.GetFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bf = new BinaryFormatter();

                var processData = (IProcessDataForSerialization)(bf.Deserialize(fs));

                return (processData);
            }
        }

        private void CommenceSaveSessionData(string fileName
            , IProcessDataForSerialization processData)
        {
            using (var fs = IOServices.GetFileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var bf = new BinaryFormatter();

                bf.Serialize(fs, processData);
            }
        }

        private string SuggestSessionFileName(string currentFileName)
        {
            var fileName = string.Empty;

            if (string.IsNullOrEmpty(currentFileName) == false)
            {
                var fi = IOServices.GetFile(currentFileName);

                fileName = fi.NameWithoutExtension + ".cache";

                fileName = IOServices.Path.Combine(fi.FolderName, fileName);
            }

            return (fileName);
        }

        #endregion
    }
}