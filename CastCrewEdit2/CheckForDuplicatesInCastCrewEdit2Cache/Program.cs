using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CheckForDuplicatesInCastCrewEdit2Cache
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if((args != null) && (args.Length > 0))
            {
                Boolean found;

                found = false;
                for(Int32 i = 0; i < args.Length; i++)
                {
                    if(args[i] == "/skipversioncheck")
                    {
                        break;
                    }
                }
                if(found)
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
