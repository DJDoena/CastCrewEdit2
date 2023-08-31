using System;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CheckForDuplicatesInCastCrewEdit2Cache
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if ((args != null) && (args.Length > 0))
            {
                bool found;

                found = false;
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/skipversioncheck")
                    {
                        break;
                    }
                }
                if (found)
                {
                    Application.Run(new MainForm(true));
                }
                else
                {
                    Application.Run(new MainForm(false));
                }
            }
            else
            {
                Application.Run(new MainForm(false));
            }
        }
    }
}
