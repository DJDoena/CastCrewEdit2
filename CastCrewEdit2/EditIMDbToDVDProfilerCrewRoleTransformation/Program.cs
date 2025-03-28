﻿using System;
using System.IO;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.DVDProfiler.EditIMDbToDVDProfilerCrewRoleTransformation
{
    internal static class Program
    {
        internal static Settings Settings;

        private static readonly string SettingsFile;

        private static readonly string ErrorFile;

        internal static IMDbToDVDProfilerCrewRoleTransformation TransformationData;

        private static readonly string FileName;

        private static readonly WindowHandle WindowHandle;

        static Program()
        {
            string rootPath;

            RegistryAccess.Init("Doena Soft.", "CastCrewEdit2");
            WindowHandle = new WindowHandle();
            rootPath = RegistryAccess.DataRootPath;
            if (string.IsNullOrEmpty(rootPath))
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
        private static void Main(string[] args)
        {
            try
            {
                MainForm mainForm;

                MoveFilesFromOldVersion();
                if (File.Exists(SettingsFile))
                {
                    try
                    {
                        Settings = XmlSerializer<Settings>.Deserialize(SettingsFile);
                    }
                    catch
                    {
                    }
                }
                CreateSettings();
                if (File.Exists(FileName))
                {
                    try
                    {
                        TransformationData = XmlSerializer<IMDbToDVDProfilerCrewRoleTransformation>.Deserialize(FileName);
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
                if (mainForm.DialogResult == DialogResult.Yes)
                {
                    if (File.Exists(FileName + ".bak"))
                    {
                        File.Delete(FileName + ".bak");
                    }
                    if (File.Exists(FileName))
                    {
                        File.Move(FileName, FileName + ".bak");
                    }
                    XmlSerializer<IMDbToDVDProfilerCrewRoleTransformation>.Serialize(FileName, TransformationData);
                    Environment.ExitCode = 1;
                }
                else
                {
                    Environment.ExitCode = -1;
                }
                try
                {
                    XmlSerializer<Settings>.Serialize(SettingsFile, Settings);
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
                    XmlSerializer<ExceptionXml>.Serialize(ErrorFile, exceptionXml);
                }
                catch
                {
                }
            }
        }

        private static void MoveFilesFromOldVersion()
        {
            string settingsFile;

            settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
               + "\\Doena Soft\\EditIMDbToDVDProfilerCrewRoleTransformation\\settings.xml";
            if ((File.Exists(settingsFile)) && (File.Exists(SettingsFile) == false))
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
