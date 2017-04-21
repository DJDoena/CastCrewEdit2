using System;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
{
    internal interface IViewModelForms : IViewModel
    {
        event EventHandler LeftFileNameChanged;

        event EventHandler RightFileNameChanged;

        event EventHandler TargetFileNameChanged;
    }
}
