using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using Microsoft.Win32;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    public static class Program
    {
        internal static Settings Settings;

        private static String SettingsFile;

        internal static Dictionary<String, PersonInfo> CastCache;

        internal static Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> PossibleCastDuplicateCache;

        private static String CastCacheFile;

        internal static Dictionary<String, PersonInfo> CrewCache;

        internal static Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> PossibleCrewDuplicateCache;

        private static String CrewCacheFile;

        private static readonly String ErrorFile;

        private static String DataPath;

        private static WindowHandle WindowHandle;

        internal static String RootPath;

        internal static String LogFile;

        private static Boolean DebugMode;

        public static readonly CastCrewEditAdapterEventHandler AdapterEventHandler;

        private static readonly bool IsAtLeastWindows10Update1803;

        private static readonly bool RunsAsElevated;

        internal static bool ShowNewBrowser => IsAtLeastWindows10Update1803 && !RunsAsElevated;

        static Program()
        {
            IsAtLeastWindows10Update1803 = GetIsAtLeastWindows10Update1803();

            RunsAsElevated = false;

            RegistryAccess.Init("Doena Soft.", "CastCrewEdit2");
            WindowHandle = new WindowHandle();
            InitPaths();
            ErrorFile = Environment.GetEnvironmentVariable("TEMP") + @"\CastCrewEdit2Crash.xml";
            DebugMode = false;
            AdapterEventHandler = new CastCrewEditAdapterEventHandler();
        }

        private static bool GetIsAtLeastWindows10Update1803()
        {
            var isAtLeastWindows10Update1803 = false;

            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            var productName = key.GetValue("ProductName") as string ?? string.Empty;

            if (productName.StartsWith("Windows 10"))
            {
                var releaseIdString = key.GetValue("ReleaseId") as string ?? string.Empty;

                if (int.TryParse(releaseIdString, out var releaseId) && releaseId >= 1803)
                {
                    isAtLeastWindows10Update1803 = true;
                }
            }
            else if (productName.StartsWith("Windows 11"))
            {
                isAtLeastWindows10Update1803 = true;
            }

            return isAtLeastWindows10Update1803;
        }

        internal static void InitPaths()
        {
            RootPath = RegistryAccess.DataRootPath;

            if (String.IsNullOrEmpty(RootPath))
            {
                RootPath = Application.StartupPath;
            }

            InitDataPaths();
        }

        private static void InitDataPaths()
        {
            DataPath = RootPath + @"\Data\";
            SettingsFile = DataPath + "CastCrewEdit2Settings.xml";
            CastCacheFile = DataPath + "Cast.xml";
            CrewCacheFile = DataPath + "Crew.xml";
            LogFile = DataPath + "Log.html";
        }

        internal static Int32 PersonCacheCount
            => (CastCache.Values.Count + CrewCache.Values.Count);

        internal static String PersonCacheCountString
            => (PersonCacheCount.ToString("#,##0"));

        [STAThread]
        public static void Main(String[] args)
        {
            Boolean embedded = false;

            Process[] processes = Process.GetProcessesByName("CastCrewEdit2");

            if (processes.Length > 1)
            {
                if (MessageBox.Show(WindowHandle, MessageBoxTexts.AnotherInstanceRunning
                    , MessageBoxTexts.ContinueHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                {
                    return;
                }
            }

            if ((args != null) && (args.Length > 0))
            {
                for (Int32 i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/lang=de")
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de");
                    }
                    else if (args[i] == "/lang=en")
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    }
                    else if (args[i] == "/debug")
                    {
                        DebugMode = true;
                    }
                    else if (args[i] == "embedded")
                    {
                        embedded = true;
                    }
                }

                MessageBoxTexts.Culture = Thread.CurrentThread.CurrentUICulture;
            }
            try
            {
                if (Directory.Exists(DataPath) == false)
                {
                    Directory.CreateDirectory(DataPath);
                }
                if (File.Exists(DataPath + "Persons.xml"))
                {
                    MessageBox.Show(WindowHandle, MessageBoxTexts.UpdatePersonsXml
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (File.Exists(SettingsFile))
                {
                    try
                    {
                        Settings = DVDProfilerSerializer<Settings>.Deserialize(SettingsFile);
                    }
                    catch
                    {
                        MessageBox.Show(WindowHandle, String.Format(MessageBoxTexts.FileCantBeRead, SettingsFile)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                CreateSettings();
                CreateCache(CastCacheFile, ref CastCache, ref PossibleCastDuplicateCache, DataGridViewTexts.Cast);
                CreateCache(CrewCacheFile, ref CrewCache, ref PossibleCrewDuplicateCache, DataGridViewTexts.Crew);
                IMDbParser.Initialize(WindowHandle);
#if UnitTest == false

                Application.EnableVisualStyles();

                if (embedded == false)
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                }

                try
                {
                    RunMainWindo(args, embedded);
                }
                finally
                {
                    try
                    {
                        DVDProfilerSerializer<Settings>.Serialize(SettingsFile, Settings);
                    }
                    catch
                    {
                        MessageBox.Show(WindowHandle, String.Format(MessageBoxTexts.FileCantBeWritten, SettingsFile)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    FlushPersonCache();
                }
#endif
            }
            catch (Exception ex)
            {
                try
                {
                    ExceptionXml exceptionXml;

                    MessageBox.Show(WindowHandle, String.Format(MessageBoxTexts.CriticalError, ex.Message, ErrorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (File.Exists(ErrorFile))
                    {
                        File.Delete(ErrorFile);
                    }
                    exceptionXml = new ExceptionXml(ex);
                    DVDProfilerSerializer<ExceptionXml>.Serialize(ErrorFile, exceptionXml);
                    WriteError(ex);
                }
                catch
                {
                    MessageBox.Show(WindowHandle, String.Format(MessageBoxTexts.FileCantBeWritten, ErrorFile), MessageBoxTexts.ErrorHeader
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            AdapterEventHandler.RaiseProgramClosed();
        }

        private static void RunMainWindo(String[] args
            , Boolean embedded)
        {
            Boolean skipversioncheck = false;

            if (args?.Length > 0)
            {
                for (Int32 i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/skipversioncheck")
                    {
                        skipversioncheck = true;

                        break;
                    }
                }
            }

            MainForm mainForm = new MainForm(skipversioncheck);

            AdapterEventHandler.MainForm = mainForm;

            if (embedded == false)
            {
                Application.Run(mainForm);
            }
            else
            {
                mainForm.ShowDialog();
            }

            AdapterEventHandler.MainForm = null;
        }

        private static void CreateCache(String cacheFile, ref Dictionary<String, PersonInfo> cache
            , ref Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> possibleDuplicateCache, String type)
        {
            PersonInfos personInfoList;

            personInfoList = null;
            CreateBackup(cacheFile);
            if (File.Exists(cacheFile))
            {
                try
                {
                    personInfoList = PersonInfos.Deserialize(cacheFile);
                }
                catch
                {
                    MessageBox.Show(WindowHandle, String.Format(MessageBoxTexts.FileCantBeRead, cacheFile)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if ((personInfoList != null) && (personInfoList.PersonInfoList != null)
                && (personInfoList.PersonInfoList.Length > 0))
            {
                cache = new Dictionary<String, PersonInfo>(personInfoList.PersonInfoList.Length + 1000);
                possibleDuplicateCache = new Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>>(personInfoList.PersonInfoList.Length + 1000);
                foreach (PersonInfo personInfo in personInfoList.PersonInfoList)
                {
                    PersonInfoWithoutBirthYear piwby;

                    personInfo.Type = type;
                    piwby = new PersonInfoWithoutBirthYear(personInfo);
                    cache.Add(personInfo.PersonLink, personInfo);
                    //For existing files
                    //TODO: Remove next next major release.
                    if ((personInfo.BirthYearWasRetrieved == false)
                        && (String.IsNullOrEmpty(personInfo.BirthYear) == false))
                    {
                        personInfo.BirthYearWasRetrieved = true;
                    }
                    if (possibleDuplicateCache.ContainsKey(piwby) == false)
                    {
                        List<PersonInfo> list;

                        list = new List<PersonInfo>(1);
                        list.Add(personInfo);
                        possibleDuplicateCache.Add(piwby, list);
                    }
                    else
                    {
                        possibleDuplicateCache[piwby].Add(personInfo);
                    }
                }
            }
            else
            {
                cache = new Dictionary<String, PersonInfo>(1000);
                possibleDuplicateCache = new Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>>(1000);
            }
        }

        private static void CreateBackup(String cacheFile)
        {
            String fileBaseName;

            fileBaseName = cacheFile.Substring(0, cacheFile.LastIndexOf("."));
            try
            {
                String fileName;

                fileName = fileBaseName + ".5.xml";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                for (Int32 i = 4; i > 0; i--)
                {
                    String fileName2;

                    fileName2 = fileBaseName + "." + i.ToString() + ".xml";
                    if (File.Exists(fileName2))
                    {
                        File.Move(fileName2, fileName);
                    }
                    fileName = fileName2;
                }
                if (File.Exists(cacheFile))
                {
                    File.Copy(cacheFile, fileName);
                }
            }
            catch (IOException)
            { }
        }

        internal static void FlushPersonCache()
        {
            FlushPersonCache(CastCache, CastCacheFile);
            FlushPersonCache(CrewCache, CrewCacheFile);
        }

        private static void FlushPersonCache(Dictionary<String, PersonInfo> cache, String cacheFile)
        {
            try
            {
                PersonInfos personInfoList;
                List<PersonInfo> list;

                personInfoList = new PersonInfos();
                list = new List<PersonInfo>(cache.Values);
                list.Sort(new Comparison<PersonInfo>(PersonInfo.CompareForSorting));
                personInfoList.PersonInfoList = list.ToArray();
                personInfoList.Serialize(cacheFile);
            }
            catch
            {
                MessageBox.Show(WindowHandle, String.Format(MessageBoxTexts.FileCantBeWritten, cacheFile)
                    , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private static void MoveFilesFromOldVersion()
        //{
        //    String settingsFile;
        //    String personCacheFile;

        //    settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        //       + "\\Doena Soft\\CastCrewEdit2\\settings.xml";
        //    personCacheFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        //        + "\\Doena Soft\\CastCrewEdit2\\persons.xml";
        //    if((File.Exists(settingsFile)) && (File.Exists(SettingsFile) == false))
        //    {
        //        File.Move(settingsFile, SettingsFile);
        //    }
        //    if((File.Exists(personCacheFile)) && (File.Exists(PersonCacheFile) == false))
        //    {
        //        File.Move(personCacheFile, PersonCacheFile);
        //    }
        //}

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
            if (Settings.EpisodesForm == null)
            {
                Settings.EpisodesForm = new SizableForm();
            }
            if (Settings.EpisodeForm == null)
            {
                Settings.EpisodeForm = new SizableForm();
            }
            if (Settings.SettingsForm == null)
            {
                Settings.SettingsForm = new BaseForm();
            }
            if (Settings.EditConfigFilesForm == null)
            {
                Settings.EditConfigFilesForm = new SizableForm();
            }
            if (Settings.EditKnownNamesConfigFileForm == null)
            {
                Settings.EditKnownNamesConfigFileForm = new SizableForm();
            }
            if (Settings.DefaultValues == null)
            {
                Settings.DefaultValues = new DefaultValues();
            }
        }

        internal static void WriteError(Exception ex)
        {
            if (DebugMode)
            {
                ExceptionXml xml;
                DateTime now;
                StringBuilder filename;

                xml = new ExceptionXml(ex);
                now = DateTime.Now;
                filename = new StringBuilder();
                filename.Append(now.Year);
                filename.Append("-");
                filename.Append(now.Month);
                filename.Append("-");
                filename.Append(now.Day);
                filename.Append("_");
                filename.Append(now.Hour);
                filename.Append("-");
                filename.Append(now.Minute);
                filename.Append("-");
                filename.Append(now.Second);
                filename.Append(".xml");
                if (Directory.Exists("errors") == false)
                {
                    Directory.CreateDirectory("errors");
                }
                DVDProfilerSerializer<ExceptionXml>.Serialize(@"errors\" + filename.ToString(), xml);
            }
        }
    }
}