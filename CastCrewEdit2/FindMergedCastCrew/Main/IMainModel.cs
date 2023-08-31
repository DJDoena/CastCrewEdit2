using System;
using System.Threading;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IMainModel
    {
        void Process(string sourceFile
            , string targetFile
            , IProcessData processData
            , ILog log
            , CancellationToken cancellationToken);

        event EventHandler<EventArgs<int>> ProgressMaxChanged;

        event EventHandler<EventArgs<int>> ProgressValueChanged;
    }
}
