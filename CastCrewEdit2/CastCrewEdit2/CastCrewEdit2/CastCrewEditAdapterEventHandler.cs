using System;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public sealed class CastCrewEditAdapterEventHandler : ICastCrewEditAdapterEventHandler
    {
        internal Form MainForm { get; set; }

        public event EventHandler<XmlEventArgs> CastCompleted;

        public event EventHandler<XmlEventArgs> CrewCompleted;

        public event EventHandler ProgramClosed;

        public void Close()
        {
            MainForm?.Close();
        }

        internal void RaiseCastCompleted(String xml)
        {
            CastCompleted?.Invoke(this, new XmlEventArgs(xml));
        }

        internal void RaiseCrewCompleted(String xml)
        {
            CrewCompleted?.Invoke(this, new XmlEventArgs(xml));
        }

        internal void RaiseProgramClosed()
        {
            ProgramClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}