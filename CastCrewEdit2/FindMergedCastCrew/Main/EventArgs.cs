using System;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    public sealed class EventArgs<T> : EventArgs where T : struct
    {
        public T Value { get; private set; }

        internal EventArgs(T value)
        {
            Value = value;
        }
    }
}
