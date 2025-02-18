using System;
using System.IO;
using System.Threading;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main.Implementations
{
    internal class MainModel : IMainModel
    {
        private readonly IIOServices IOServices;

        private readonly IUIServices UIServices;

        private readonly IWebServices WebServices;

        private ILog Log { get; set; }

        private CancellationToken CancellationToken { get; set; }

        private string SourceFile { get; set; }

        private string TargetFile { get; set; }

        private IProcessData ProcessData { get; set; }

        public MainModel(IIOServices ioServices
            , IUIServices uiServices
            , IWebServices webServices)
        {
            IOServices = ioServices;
            UIServices = uiServices;
            WebServices = webServices;
        }

        #region IMainModel

        public event EventHandler<EventArgs<int>> ProgressMaxChanged;

        public event EventHandler<EventArgs<int>> ProgressValueChanged;

        public void Process(string sourceFile
            , string targetFile
            , IProcessData processData
            , ILog log
            , CancellationToken cancellationToken)
        {
            SourceFile = sourceFile;

            TargetFile = targetFile;

            ProcessData = processData;

            Log = log;

            CancellationToken = cancellationToken;

            try
            {
                TryProcess();
            }
            catch (Exception ex)
            {
                UIServices.ShowMessageBox(ex.Message, "Error", Buttons.OK, Icon.Error);
            }
        }

        #endregion

        private void TryProcess()
        {
            PersonInfos personInfos;
            if (ReadCache(out personInfos))
            {
                Process(personInfos);
            }
        }

        private void Process(PersonInfos personInfos)
        {
            IPersonsProcessor processor = new PersonsProcessor(ProcessData, Log, WebServices, CancellationToken);

            processor.ProgressValueChanged += OnProcessorProgressValueChanged;

            processor.ProgressMaxChanged += OnProcessorProgressMaxChanged;

            try
            {
                TryProcess(personInfos, processor);
            }
            finally
            {
                processor.ProgressValueChanged -= OnProcessorProgressValueChanged;

                processor.ProgressMaxChanged -= OnProcessorProgressMaxChanged;
            }
        }

        private void TryProcess(PersonInfos personInfos
            , IPersonsProcessor processor)
        {
            personInfos = processor.Process(personInfos);

            if (CancellationToken.IsCancellationRequested == false)
            {
                WriteCache(personInfos);
            }
        }

        private bool ReadCache(out PersonInfos personInfoList)
        {
            personInfoList = null;

            PersonInfo.CreatorActive = true;

            try
            {
                using (var stream = IOServices.GetFileStream(SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    personInfoList = XmlSerializer<PersonInfos>.Deserialize(stream);
                }
            }
            catch
            {
                UIServices.ShowMessageBox($"Could not read '{SourceFile}'", "Error", Buttons.OK, Icon.Error);

                return (false);
            }
            finally
            {
                PersonInfo.CreatorActive = false;
            }

            return (personInfoList.PersonInfoList != null);
        }

        private void WriteCache(PersonInfos personInfos)
        {
            if (personInfos != null)
            {
                try
                {
                    BackupHelper.BackupFile(TargetFile, IOServices);

                    using (var stream = IOServices.GetFileStream(TargetFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        XmlSerializer<PersonInfos>.Serialize(stream, personInfos);
                    }
                }
                catch
                {
                    UIServices.ShowMessageBox($"Could not write '{TargetFile}'", "Error", Buttons.OK, Icon.Error);
                }
            }
        }

        private void OnProcessorProgressValueChanged(object sender
            , EventArgs<int> e)
        {
            ProgressValueChanged?.Invoke(this, e);
        }

        private void OnProcessorProgressMaxChanged(object sender
            , EventArgs<int> e)
        {
            ProgressMaxChanged?.Invoke(this, e);
        }
    }
}