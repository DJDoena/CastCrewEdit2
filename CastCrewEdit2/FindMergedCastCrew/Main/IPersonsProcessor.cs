using System;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IPersonsProcessor
    {
        event EventHandler<EventArgs<int>> ProgressMaxChanged;

        event EventHandler<EventArgs<int>> ProgressValueChanged;

        PersonInfos Process(PersonInfos persons);
    }
}