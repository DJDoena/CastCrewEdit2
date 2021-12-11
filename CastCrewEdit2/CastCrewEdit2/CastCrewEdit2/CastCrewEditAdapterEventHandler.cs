namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;
    using System.Windows.Forms;

    public sealed class CastCrewEditAdapterEventHandler : ICastCrewEditAdapterEventHandler
    {
        internal Form MainForm { get; set; }

        public event EventHandler<XmlEventArgs> CastCompleted;

        public event EventHandler<XmlEventArgs> CrewCompleted;

        public event EventHandler ProgramClosed;

        public void Close()
        {
            this.MainForm?.Close();
        }

        internal void RaiseCastCompleted(string xml)
        {
            CastCompleted?.Invoke(this, new XmlEventArgs(xml));
        }

        internal void RaiseCrewCompleted(string xml)
        {
            CrewCompleted?.Invoke(this, new XmlEventArgs(xml));
        }

        internal void RaiseProgramClosed()
        {
            ProgramClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}