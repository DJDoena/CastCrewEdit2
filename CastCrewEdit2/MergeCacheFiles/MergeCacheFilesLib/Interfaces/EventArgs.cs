using System;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.MergeCacheFiles
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