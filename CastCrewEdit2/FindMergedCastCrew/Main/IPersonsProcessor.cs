using System;
using DoenaSoft.DVDProfiler.CastCrewEdit2;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal interface IPersonsProcessor
    {
        event EventHandler<EventArgs<Int32>> ProgressMaxChanged;

        event EventHandler<EventArgs<Int32>> ProgressValueChanged;

        PersonInfos Process(PersonInfos persons);
    }
}