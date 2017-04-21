using System;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IFileHelper
    {
        Boolean ShowOpenFileDialog(String suggestedFileName
            , String filter
            , String title
            , out String fileName);

        Boolean ShowSaveFileDialog(String suggestedFileName
            , String filter
            , String title
            , out String fileName);

        IProcessDataForSerialization LoadSessionData(String sourceFileName);

        void SaveSessionData(String sourceFileName
            , IProcessDataForSerialization processData);
    }
}