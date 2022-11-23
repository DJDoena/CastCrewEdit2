namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using DVDProfilerHelper;
    using Forms;
    using Helper;
    using Microsoft.Win32;
    using Resources;

    public static class Program
    {
        public static readonly CastCrewEditAdapterEventHandler AdapterEventHandler;

        internal static Settings Settings;

        internal static Dictionary<string, PersonInfo> CastCache;

        internal static Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> PossibleCastDuplicateCache;

        internal static Dictionary<string, PersonInfo> CrewCache;

        internal static Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> PossibleCrewDuplicateCache;

        internal static string RootPath
        {
            get => _rootPath;
            set => _rootPath = value;
        }

        internal static string LogFile { get; set; }

        internal static bool DebugMode { get; set; }

        private static string _rootPath;

        private static readonly string _errorFile;

        private static readonly WindowHandle _windowHandle;

        private static readonly bool _isAtLeastWindows10Update1803;

        private static readonly bool _runsAsElevated;

        private static string _settingsFile;

        private static string _castCacheFile;

        private static string _crewCacheFile;

        private static string _dataPath;

        private static BrowserControlSelection _selectedBrowserControl;

        internal static DefaultValues DefaultValues => Settings.DefaultValues;

        static Program()
        {
            _isAtLeastWindows10Update1803 = GetIsAtLeastWindows10Update1803();

            _runsAsElevated = false;

            RegistryAccess.Init("Doena Soft.", "CastCrewEdit2");

            _windowHandle = new WindowHandle();

            RootPath = GetRootPath();

            InitDataPaths();

            _errorFile = Path.Combine(Path.GetTempPath(), "CastCrewEdit2Crash.xml");

            DebugMode = false;

            _selectedBrowserControl = BrowserControlSelection.Undefined;

            AdapterEventHandler = new CastCrewEditAdapterEventHandler();
        }

        /// <remarks>
        /// This method is neccessary for external callers that want ot use the Adapter library
        /// </remarks>
        private static void InitDataPaths()
        {
            _dataPath = Path.Combine(RootPath, "Data");

            _settingsFile = Path.Combine(_dataPath, "CastCrewEdit2Settings.xml");

            _castCacheFile = Path.Combine(_dataPath, "Cast.xml");

            _crewCacheFile = Path.Combine(_dataPath, "Crew.xml");

            LogFile = Path.Combine(_dataPath, "Log.html");
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

        internal static string GetRootPath()
        {
            var rootPath = RegistryAccess.DataRootPath;

            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Application.StartupPath;
            }

            return rootPath;
        }

        internal static int PersonCacheCount => CastCache.Values.Count + CrewCache.Values.Count;

        internal static string PersonCacheCountString => PersonCacheCount.ToString("#,##0");

        [STAThread]
        public static void Main(string[] args)
        {
            var embedded = false;

            var processes = Process.GetProcessesByName("CastCrewEdit2");

            if (processes.Length > 1)
            {
                if (MessageBox.Show(_windowHandle, MessageBoxTexts.AnotherInstanceRunning, MessageBoxTexts.ContinueHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                {
                    return;
                }
            }

            if (args != null && args.Length > 0)
            {
                for (var argIndex = 0; argIndex < args.Length; argIndex++)
                {
                    var arg = args[argIndex].ToLowerInvariant();

                    if (arg == "/lang=de")
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de");
                    }
                    else if (arg == "/lang=en")
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    }
                    else if (arg == "/debug")
                    {
                        DebugMode = true;
                    }
                    else if (arg == "embedded")
                    {
                        embedded = true;
                    }
                    else if (arg == "/forceoldbrowser" || arg == "/browser=ie6")
                    {
                        _selectedBrowserControl = BrowserControlSelection.FormsDefault;
                    }
                    else if (arg == "/browser=webviewcompatible")
                    {
                        _selectedBrowserControl = BrowserControlSelection.WebViewCompatible;
                    }
                    else if (arg == "/browser=webview")
                    {
                        _selectedBrowserControl = BrowserControlSelection.WebView;
                    }
                    else if (arg == "/browser=webview2")
                    {
                        _selectedBrowserControl = BrowserControlSelection.WebView2;
                    }
                }

                MessageBoxTexts.Culture = Thread.CurrentThread.CurrentUICulture;
            }
            try
            {
                if (!Directory.Exists(_dataPath))
                {
                    Directory.CreateDirectory(_dataPath);
                }
                if (File.Exists(_dataPath + "Persons.xml"))
                {
                    MessageBox.Show(_windowHandle, MessageBoxTexts.UpdatePersonsXml, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
                if (File.Exists(_settingsFile))
                {
                    try
                    {
                        Settings = DVDProfilerSerializer<Settings>.Deserialize(_settingsFile);
                    }
                    catch
                    {
                        MessageBox.Show(_windowHandle, string.Format(MessageBoxTexts.FileCantBeRead, _settingsFile), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                CreateSettings();

                CreateCache(_castCacheFile, ref CastCache, ref PossibleCastDuplicateCache, DataGridViewTexts.Cast);

                CreateCache(_crewCacheFile, ref CrewCache, ref PossibleCrewDuplicateCache, DataGridViewTexts.Crew);

                IMDbParser.Initialize(_windowHandle);

#if !UnitTest

                Application.EnableVisualStyles();

                if (!embedded)
                {
                    Application.SetCompatibleTextRenderingDefault(false);
                }

                try
                {
                    RunMainWindow(args, embedded);
                }
                finally
                {
                    try
                    {
                        DVDProfilerSerializer<Settings>.Serialize(_settingsFile, Settings);
                    }
                    catch
                    {
                        MessageBox.Show(_windowHandle, string.Format(MessageBoxTexts.FileCantBeWritten, _settingsFile), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    FlushPersonCache();
                }

#endif

            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(_windowHandle, string.Format(MessageBoxTexts.CriticalError, ex.Message, _errorFile), MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    if (File.Exists(_errorFile))
                    {
                        File.Delete(_errorFile);
                    }

                    var exceptionXml = new ExceptionXml(ex);

                    DVDProfilerSerializer<ExceptionXml>.Serialize(_errorFile, exceptionXml);

                    WriteError(ex, true);
                }
                catch
                {
                    MessageBox.Show(_windowHandle, string.Format(MessageBoxTexts.FileCantBeWritten, _errorFile), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            AdapterEventHandler.RaiseProgramClosed();
        }

        private static void RunMainWindow(string[] args, bool embedded)
        {
            var skipversioncheck = false;

            if (args?.Length > 0)
            {
                for (var argIndex = 0; argIndex < args.Length; argIndex++)
                {
                    var arg = args[argIndex].ToLowerInvariant();

                    if (arg == "/skipversioncheck")
                    {
                        skipversioncheck = true;

                        break;
                    }
                }
            }

            if (_selectedBrowserControl == BrowserControlSelection.Undefined)
            {
                if (_isAtLeastWindows10Update1803 && !_runsAsElevated)
                {
                    _selectedBrowserControl = BrowserControlSelection.WebView;
                }
                else
                {
                    _selectedBrowserControl = BrowserControlSelection.FormsDefault;
                }
            }

            using (var mainForm = new MainForm(skipversioncheck, _selectedBrowserControl))
            {
                AdapterEventHandler.MainForm = mainForm;

                if (!embedded)
                {
                    Application.Run(mainForm);
                }
                else
                {
                    mainForm.ShowDialog();
                }

                AdapterEventHandler.MainForm = null;
            }
        }

        private static void CreateCache(string cacheFile, ref Dictionary<string, PersonInfo> cache, ref Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>> possibleDuplicateCache, string type)
        {
            PersonInfos personInfoList = null;

            CreateBackup(cacheFile);

            if (File.Exists(cacheFile))
            {
                try
                {
                    personInfoList = PersonInfos.Deserialize(cacheFile);
                }
                catch
                {
                    MessageBox.Show(_windowHandle, string.Format(MessageBoxTexts.FileCantBeRead, cacheFile), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (personInfoList != null
                && personInfoList.PersonInfoList != null
                && personInfoList.PersonInfoList.Length > 0)
            {

                cache = new Dictionary<string, PersonInfo>(personInfoList.PersonInfoList.Length + 1000);

                possibleDuplicateCache = new Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>>(personInfoList.PersonInfoList.Length + 1000);

                foreach (var personInfo in personInfoList.PersonInfoList)
                {
                    personInfo.Type = type;

                    var piwby = new PersonInfoWithoutBirthYear(personInfo);

                    cache.Add(personInfo.PersonLink, personInfo);

                    //For existing files
                    //TODO: Remove next next major release.
                    if (!personInfo.BirthYearWasRetrieved && !string.IsNullOrEmpty(personInfo.BirthYear))
                    {
                        personInfo.BirthYearWasRetrieved = true;
                    }

                    if (!possibleDuplicateCache.ContainsKey(piwby))
                    {
                        var list = new List<PersonInfo>(1)
                        {
                            personInfo,
                        };

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
                cache = new Dictionary<string, PersonInfo>(1000);

                possibleDuplicateCache = new Dictionary<PersonInfoWithoutBirthYear, List<PersonInfo>>(1000);
            }
        }

        private static void CreateBackup(string cacheFile)
        {
            var fileBaseName = cacheFile.Substring(0, cacheFile.LastIndexOf("."));

            try
            {
                var fileName = fileBaseName + ".5.xml";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                for (var backupNumber = 4; backupNumber > 0; backupNumber--)
                {
                    var fileName2 = fileBaseName + "." + backupNumber.ToString() + ".xml";

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
            FlushPersonCache(CastCache, _castCacheFile);
            FlushPersonCache(CrewCache, _crewCacheFile);
        }

        private static void FlushPersonCache(Dictionary<string, PersonInfo> cache, string cacheFile)
        {
            try
            {
                var personInfoList = new PersonInfos();

                var list = new List<PersonInfo>(cache.Values);

                list.Sort(new Comparison<PersonInfo>(PersonInfo.CompareForSorting));

                personInfoList.PersonInfoList = list.ToArray();
                personInfoList.Serialize(cacheFile);
            }
            catch
            {
                MessageBox.Show(_windowHandle, string.Format(MessageBoxTexts.FileCantBeWritten, cacheFile), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        internal static void WriteError(Exception ex, bool forceWrite = false)
        {
            if (DebugMode)
            {
                var xml = new ExceptionXml(ex);

                var now = DateTime.Now;

                var filename = new StringBuilder();

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

                if (!Directory.Exists("errors"))
                {
                    Directory.CreateDirectory("errors");
                }

                DVDProfilerSerializer<ExceptionXml>.Serialize(@"errors\" + filename.ToString(), xml);
            }
        }
    }
}