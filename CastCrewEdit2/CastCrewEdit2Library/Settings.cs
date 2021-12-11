namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    [Serializable]
    public class BaseForm
    {
        public int Top = 50;

        public int Left = 50;
    }

    [Serializable]
    public class SizableForm : BaseForm
    {
        public int Height = 500;

        public int Width = 900;

        public FormWindowState WindowState = FormWindowState.Normal;

        public Rectangle RestoreBounds;
    }
}