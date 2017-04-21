using System;
using System.Xml.Serialization;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using System.IO;
using System.Windows.Forms;
using System.Text;
using DoenaSoft.DVDProfiler.DVDProfilerXML;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.EditIMDbToDVDProfilerCrewRoleTransformation
{
    static class Program
    {
        internal static Settings Settings;

        private static readonly String SettingsFile;

        private static readonly String ErrorFile;

        internal static IMDbToDVDProfilerCrewRoleTransformation TransformationData;

        private static readonly String FileName;

        private static WindowHandle WindowHandle;

        static Program()
        {
            String rootPath;

            RegistryAccess.Init("Doena Soft.", "CastCrewEdit2");
            WindowHandle = new WindowHandle();
            rootPath = RegistryAccess.DataRootPath;
            if(String.IsNullOrEmpty(rootPath))
            {
                rootPath = Application.StartupPath;
            }
            SettingsFile = rootPath + @"\Data\EditIMDbToDVDProfilerCrewRoleTransformationSettings.xml";
            ErrorFile = Environment.GetEnvironmentVariable("TEMP") 
                + @"\EditIMDbToDVDProfilerCrewRoleTransformationCrash.xml";
            FileName = rootPath + @"\Data\IMDbToDVDProfilerCrewRoleTransformation.xml";
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        static void Main(String[] args)
        {
            try
            {
                MainForm mainForm;

                MoveFilesFromOldVersion();
                if (File.Exists(SettingsFile))
                {
                    try
                    {
                        Settings = Serializer<Settings>.Deserialize(SettingsFile);
                    }
                    catch
                    {
                    }
                }
                CreateSettings();
                if(File.Exists(FileName))
                {
                    try
                    {
                        TransformationData = Serializer<IMDbToDVDProfilerCrewRoleTransformation>.Deserialize(FileName);
                    }
                    catch
                    {
                    }
                }
                CreateTransformationData();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if ((args != null) && (args.Length == 1) && (args[0] == "/skipversioncheck"))
                {
                    mainForm = new MainForm(true);                    
                }
                else
                {
                    mainForm = new MainForm(false);                    
                }                
                Application.Run(mainForm);
                if(mainForm.DialogResult == DialogResult.Yes)
                {
                    if(File.Exists(FileName + ".bak"))
                    {
                        File.Delete(FileName + ".bak");
                    }
                    if(File.Exists(FileName))
                    {
                        File.Move(FileName, FileName + ".bak");
                    }
                    Serializer<IMDbToDVDProfilerCrewRoleTransformation>.Serialize(FileName, TransformationData);
                    Environment.ExitCode = 1;
                }
                else
                {
                    Environment.ExitCode = -1;
                }
                try
                {
                    Serializer<Settings>.Serialize(SettingsFile, Settings);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(WindowHandle, ex.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                try
                {
                    ExceptionXml exceptionXml;

                    if (File.Exists(ErrorFile))
                    {
                        File.Delete(ErrorFile);
                    }
                    exceptionXml = new ExceptionXml(ex);
                    Serializer<ExceptionXml>.Serialize(ErrorFile, exceptionXml);
                }
                catch
                {
                }
            }
        }

        private static void MoveFilesFromOldVersion()
        {
            String settingsFile;

            settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
               + "\\Doena Soft\\EditIMDbToDVDProfilerCrewRoleTransformation\\settings.xml";
            if((File.Exists(settingsFile)) && (File.Exists(SettingsFile) == false))
            {
                File.Move(settingsFile, SettingsFile);
            }
        }

        private static void CreateTransformationData()
        {
            if (TransformationData == null)
            {
                TransformationData = new IMDbToDVDProfilerCrewRoleTransformation();
            }
            if (TransformationData.CreditTypeList == null)
            {
                TransformationData.CreditTypeList = new CreditType[0];
            }
        }

        private static void CreateSettings()
        {
            if (Settings == null)
            {
                Settings = new Settings();
            }
            if (Settings.MainForm == null)
            {
                Settings.MainForm = new SizableForm();
            }
        }
    }
}
