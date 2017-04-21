using System;
using System.Threading;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IMainModel
    {
        void Process(String sourceFile
            , String targetFile
            , IProcessData processData
            , ILog log
            , CancellationToken cancellationToken);

        event EventHandler<EventArgs<Int32>> ProgressMaxChanged;

        event EventHandler<EventArgs<Int32>> ProgressValueChanged;
    }
}
