namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using DVDProfilerHelper;
    using Extended;
    using Helper;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using Resources;

    [ComVisible(true)]
    public class CastCrewEdit2BaseForm : Form
    {
        protected static string _tvShowTitle;

        protected static string _tvShowTitleLink;

        protected static bool _hasAgreed;

        protected static bool _settingsHaveChanged;

        protected static readonly Regex _castBlockStartRegex;

        protected static readonly Regex _blockEndRegex;

        protected static readonly Regex _castLineRegex;

        protected static readonly Regex _crewBlockStartRegex;

#if UnitTest

        public int _progressMax;

        public int _progressInterval;

        public ColorProgressBar _progressBar;

        public int _progressValue;

#else

        private int _progressMax;

        private int _progressInterval;

        protected ColorProgressBar _progressBar;

        private int _progressValue;

#endif

        protected bool _suppressProgress;

        private readonly object _progressLock;

        private const int OneHundred = 100;

        static CastCrewEdit2BaseForm()
        {
            _tvShowTitle = string.Empty;

            _hasAgreed = false;

            _castBlockStartRegex = new Regex("<table (.*?)class=\"cast_list\"(.*?)>", RegexOptions.Compiled | RegexOptions.Multiline);

            _blockEndRegex = new Regex("</table>", RegexOptions.Compiled);

            _castLineRegex = new Regex("<tr (.*?)class=\"(odd|even)\"(.*?)>.*?</tr>", RegexOptions.Compiled | RegexOptions.Multiline);

            _crewBlockStartRegex = new Regex("<h4 (.*?)class=\"dataHeaderWithBorder\"(.*?)>", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public CastCrewEdit2BaseForm()
        {
            _progressLock = new object();
        }

        protected void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var aboutBox = new AboutBox(this.GetType().Assembly))
            {
                aboutBox.ShowDialog(this);
            }
        }

        protected void OpenReadme()
        {
            var helpFile = Path.Combine(Application.StartupPath, "ReadMe", "CCE2_ReadMe.html");

            if (File.Exists(helpFile))
            {
                using (var helpForm = new HelpForm(helpFile))
                {
                    helpForm.Text = "Read Me";
                    helpForm.ShowDialog(this);
                }
            }
        }

        protected static void ParseCastAndCrew(string key, bool parseCast, bool parseCrew, bool parseSoundtrack, bool initializeLists, ref List<Match> castMatches, ref List<CastInfo> castList, ref List<KeyValuePair<Match, List<Match>>> crewMatches, ref List<CrewInfo> crewList, ref Dictionary<string, List<SoundtrackMatch>> soundtrackMatches)
        {
            var targetUrl = $"{IMDbParser.TitleUrl}{key}/fullcredits";

            var webSite = IMDbParser.GetWebSite(targetUrl);

            if (initializeLists)
            {
                castList = new List<CastInfo>();
                crewList = new List<CrewInfo>();
            }

            castMatches = new List<Match>();

            crewMatches = new List<KeyValuePair<Match, List<Match>>>();

            soundtrackMatches = new Dictionary<string, List<SoundtrackMatch>>();

            #region Parse for Cast

            if (parseCast)
            {
                using (var sr = new StringReader(webSite))
                {
                    while (sr.Peek() != -1)
                    {
                        var line = ReadLine(sr);

                        if (_castBlockStartRegex.Match(line).Success)
                        {
                            var block = new StringBuilder();

                            block.Append(line.Trim());

                            while (!_blockEndRegex.Match(line).Success && sr.Peek() != -1)
                            {
                                line = ReadLine(sr);

                                block.Append(line.Trim());
                            }

                            var lineMatches = _castLineRegex.Matches(block.ToString());

                            if (lineMatches.Count > 0)
                            {
                                foreach (Match lineMatch in lineMatches)
                                {
                                    if (lineMatch.Success)
                                    {
                                        IMDbParser.ProcessCastLine(lineMatch.Value, castMatches);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Parse for Crew

            if (parseCrew)
            {
                using (var sr = new StringReader(webSite.ToString()))
                {
                    while (sr.Peek() != -1)
                    {
                        var line = ReadLine(sr);

                        if (_crewBlockStartRegex.Match(line).Success && !line.Contains("id=\"cast\""))
                        {
                            var block = new StringBuilder();

                            block.Append(line.Trim());

                            while (!_blockEndRegex.Match(line).Success && sr.Peek() != -1)
                            {
                                line = ReadLine(sr);
                                block.Append(line.Trim());
                            }

                            IMDbParser.ProcessCrewLine(block.ToString(), crewMatches);
                        }
                    }
                }
            }

            #endregion

            #region Soundtrack

            if (parseSoundtrack)
            {
                soundtrackMatches = ParseSoundtrack(key);
            }

            #endregion
        }

        private static string ReadLine(StringReader sr)
        {
            var line = sr.ReadLine();

            var openTagCount = GetOpenTagCount(line);

            var closeTagCount = GetCloseTagCount(line);

            while (openTagCount > closeTagCount && sr.Peek() != -1)
            {
                line += sr.ReadLine();

                openTagCount = GetOpenTagCount(line);

                closeTagCount = GetCloseTagCount(line);

                if (line.Contains("</script>"))
                {
                    break;
                }
            }

            return line;
        }

        private static int GetOpenTagCount(string line) => line.Count(c => c == '<') - (line.Split(new[] { "< " }, StringSplitOptions.None).Length - 1);
        private static int GetCloseTagCount(string line) => line.Count(c => c == '>');

        private void EditConfigFile(string fileName, string name, FileNameType fileNameType, bool createFile)
        {
            if (createFile && !File.Exists(fileName))
            {
                using (var temp = File.Create(fileName))
                {
                }
            }

            if (File.Exists(fileName))
            {
                switch (fileNameType)
                {
                    case FileNameType.KnownNames:
                        {
                            using (var editForm = new EditKnownNamesConfigFileForm(fileName, name))
                            {
                                if (editForm.ShowDialog(this) == DialogResult.Yes)
                                {
                                    IMDbParser.InitList(fileName, fileNameType, this);
                                }
                            }

                            break;
                        }
                    default:
                        {
                            using (var editForm = new EditConfigFileForm(fileName, name))
                            {
                                if (editForm.ShowDialog(this) == DialogResult.Yes)
                                {
                                    IMDbParser.InitList(fileName, fileNameType, this);
                                }
                            }

                            break;
                        }
                }
            }
            else
            {
                MessageBox.Show(this, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void OnReadmeToolStripMenuItemClick(object sender, EventArgs e) => this.OpenReadme();

        protected void OnIMDbToDVDProfilerTransformationDataToolStripMenuItemClick(object sender, EventArgs e)
        {
            var fileName = Application.StartupPath + "\\EditIMDbToDVDProfilerCrewRoleTransformation.exe";

            if (File.Exists(fileName))
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo(fileName)
                };

                process.Start();

                var counter = 0;

                while (!process.HasExited)
                {
                    counter++;

                    if (counter % 8 == 0)
                    {
                        this.Refresh();
                    }

                    Thread.Sleep(250);
                }

                if (process.ExitCode == 1)
                {
                    IMDbParser.InitIMDbToDVDProfilerCrewRoleTransformation(this);
                }
            }
            else
            {
                MessageBox.Show(this, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void OnKnownNamesToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\KnownNames.txt", EditWindowNames.KnownNames, FileNameType.KnownNames, false);

        protected void OnLastnameSuffixesToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\KnownLastNameSuffixes.txt", EditWindowNames.KnownLastnameSuffixes, FileNameType.LastnameSuffixes, false);

        protected void OnLastnamePrefixesToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\KnownLastnamePrefixes.txt", EditWindowNames.KnownLastnamePrefixes, FileNameType.LastnamePrefixes, false);

        protected void OnFirstnamePrefixesToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\KnownFirstnamePrefixes.txt", EditWindowNames.KnownFirstnamePrefixes, FileNameType.FirstnamePrefixes, false);

        protected void OnIgnoreCustomInIMDbCreditTypeToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\IgnoreCustomInIMDbCategory.txt", EditWindowNames.IgnoreCustominIMDbCategory, FileNameType.IgnoreCustomInIMDbCreditType, true);

        protected void OnIgnoreIMDbCreditTypeInOtherToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\IgnoreIMDbCategoryInOther.txt", EditWindowNames.IgnoreIMDbCategoryinOther, FileNameType.IgnoreIMDbCreditTypeInOther, true);

        protected void OnForcedFakeBirthYearsToolStripMenuItemClick(object sender, EventArgs e) => this.EditConfigFile(Program.RootPath + @"\Data\ForcedFakeBirthYears.txt", EditWindowNames.IgnoreIMDbCategoryinOther, FileNameType.ForcedFakeBirthYears, true);

        protected static Dictionary<string, List<SoundtrackMatch>> ParseSoundtrack(string titleLink)
        {
            Dictionary<string, List<SoundtrackMatch>> soundtrackEntries = null;

            var soundtrackUrl = $"{IMDbParser.TitleUrl}{titleLink}/soundtrack/";

            var webSite = IMDbParser.GetWebSite(soundtrackUrl);

            ParseSoundtrackNewStyle(webSite, ref soundtrackEntries);

            if (soundtrackEntries == null || soundtrackEntries.Count == 0)
            {
                ParseSoundtrackOldStyle(webSite, ref soundtrackEntries);
            }

            if (soundtrackEntries == null)
            {
                soundtrackEntries = new Dictionary<string, List<SoundtrackMatch>>(0);
            }

            return soundtrackEntries;
        }

        private static void ParseSoundtrackNewStyle(string webSite, ref Dictionary<string, List<SoundtrackMatch>> soundtrackEntries)
        {
            using (var sr = new StringReader(webSite))
            {
                string soundtrack = null;
                while (sr.Peek() != -1)
                {
                    var line = ReadLine(sr);

                    var indexOf = line.IndexOf(IMDbParser.SoundtrackStartNewStyle);

                    if (indexOf != -1)
                    {
                        soundtrack = line.Substring(indexOf);

                        break;
                    }
                }

                if (soundtrack != null)
                {
                    soundtrackEntries = IMDbParser.ParseSoundtrackNewStyle(soundtrack);
                }
            }
        }

        private static void ParseSoundtrackOldStyle(string webSite, ref Dictionary<string, List<SoundtrackMatch>> soundtrackEntries)
        {
            using (var sr = new StringReader(webSite))
            {
                var soundtrackFound = false;

                var soundtrack = new StringBuilder();

                while (sr.Peek() != -1)
                {
                    var line = ReadLine(sr);

                    if (!soundtrackFound)
                    {
                        var beginMatch = IMDbParser.SoundtrackStartOldStyleRegex.Match(line);

                        if (beginMatch.Success)
                        {
                            soundtrackFound = true;

                            continue;
                        }
                    }

                    if (soundtrackFound)
                    {
                        soundtrack.AppendLine(line);
                    }
                }

                if (soundtrack.Length > 0)
                {
                    soundtrackEntries = IMDbParser.ParseSoundtrackOldStyle(soundtrack);
                }
            }
        }

        #region Progress

        #region SetProgress

#if UnitTest

        public void SetProgress()

#else

        protected void SetProgress()

#endif
        {
            if (!_suppressProgress)
            {
                this.SetProgressSafe();
            }
        }

        private void SetProgressSafe()
        {
            lock (_progressLock)
            {
                _progressValue++;

                if (_progressValue == _progressMax || (_progressValue % _progressInterval) == 0)
                {
                    this.SetProgressBarValue();
                }
            }
        }

        private void SetProgressBarValue()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetProgress(this.SetProgressBarValueSync));
            }
            else
            {
                this.SetProgressBarValueSync();
            }
        }

        private void SetProgressBarValueSync()
        {
#if !UnitTest
            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressValue(_progressValue, _progressMax);
            }
#endif

            _progressBar.Value = _progressValue;

            Application.DoEvents();
        }

        #endregion

        protected void EndProgress()
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                TaskbarManager.Instance.OwnerHandle = IntPtr.Zero;
            }

            _progressBar.Value = 0;
            _progressBar.Text = string.Empty;
            _progressBar.BarColor = SystemColors.Control;

            Application.DoEvents();
        }

        #region StartProgress

        protected void StartProgress(int maxValue, Color color)
        {
            _progressValue = 0;

            _progressMax = maxValue;

            this.CalculateUpdateInterval();

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.OwnerHandle = this.Handle;
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                TaskbarManager.Instance.SetProgressValue(_progressValue, _progressMax);
            }

            _progressBar.Minimum = _progressValue;
            _progressBar.Maximum = _progressMax;
            _progressBar.BarColor = color;

            Application.DoEvents();
        }

        protected void RestartProgress() => this.StartProgress(_progressMax, _progressBar.BarColor);

        private void CalculateUpdateInterval()
        {
            _progressInterval = 1;

            if (_progressMax > OneHundred)
            {
                _progressInterval = _progressMax / OneHundred;

                if (this.IsNotDivisibleWithoutRemainder())
                {
                    //We don't want to finish after we've reached 100%
                    _progressInterval++;
                }
            }
        }

        private bool IsNotDivisibleWithoutRemainder() => (_progressMax % OneHundred) != 0;

        #endregion

        #endregion

        protected void ProcessLines(List<CastInfo> castList, List<Match> castMatches, List<CrewInfo> crewList, List<KeyValuePair<Match, List<Match>>> crewMatches, Dictionary<string, List<SoundtrackMatch>> sountrackMatches, DefaultValues defaultValues)
        {
            IMDbParser.ProcessCastLine(castList, castMatches, defaultValues, this.SetProgress);

            IMDbParser.ProcessCrewLine(crewList, crewMatches, defaultValues, this.SetProgress);

            IMDbParser.ProcessSoundtrackLine(crewList, sountrackMatches, defaultValues, this.SetProgress);
        }

        protected void StartLongAction()
        {
            this.Enabled = false;

            this.UseWaitCursor = true;

            this.Cursor = Cursors.WaitCursor;

            Application.DoEvents();
        }

        protected void EndLongAction()
        {
            this.UseWaitCursor = false;

            this.Cursor = Cursors.Default;

            this.Enabled = true;

            Application.DoEvents();
        }
    }
}