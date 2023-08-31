namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IFileHelper
    {
        bool ShowOpenFileDialog(string suggestedFileName
            , string filter
            , string title
            , out string fileName);

        bool ShowSaveFileDialog(string suggestedFileName
            , string filter
            , string title
            , out string fileName);

        IProcessDataForSerialization LoadSessionData(string sourceFileName);

        void SaveSessionData(string sourceFileName
            , IProcessDataForSerialization processData);
    }
}