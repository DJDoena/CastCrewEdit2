﻿using System;
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
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    [ComVisible(true)]
    public class CastCrewEdit2BaseForm : Form
    {
        protected static String TVShowTitle;
        protected static String TVShowTitleLink;
        protected static Boolean s_HasAgreed;
        protected static Boolean SettingsHaveChanged;
        protected static readonly Regex CastBlockStartRegex;
        protected static readonly Regex BlockEndRegex;
        protected static readonly Regex CastLineRegex;
        protected static readonly Regex CrewBlockStartRegex;
#if UnitTest
        public Int32 ProgressMax;
        public Int32 ProgressInterval;
        public ColorProgressBar TheProgressBar;
        public Int32 ProgressValue;
#else
        private Int32 ProgressMax;
        private Int32 ProgressInterval;
        protected ColorProgressBar TheProgressBar;
        private Int32 ProgressValue;
#endif
        protected Boolean SuppressProgress;
        private readonly Object ProgressLock;
        private const Int32 OneHundred = 100;

        static CastCrewEdit2BaseForm()
        {
            TVShowTitle = String.Empty;
            s_HasAgreed = false;
            CastBlockStartRegex = new Regex("<table (.*?)class=\"cast_list\"(.*?)>", RegexOptions.Compiled | RegexOptions.Multiline);
            BlockEndRegex = new Regex("</table>", RegexOptions.Compiled);
            CastLineRegex = new Regex("<tr (.*?)class=\"(odd|even)\"(.*?)>.*?</tr>", RegexOptions.Compiled | RegexOptions.Multiline);
            CrewBlockStartRegex = new Regex("<h4 (.*?)class=\"dataHeaderWithBorder\"(.*?)>", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public CastCrewEdit2BaseForm()
        {
            ProgressLock = new Object();
        }

        protected void OnAboutToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox(GetType().Assembly))
            {
                aboutBox.ShowDialog(this);
            }
        }

        protected void OpenReadme()
        {
            String helpFile;

            helpFile = Application.StartupPath + @"\ReadMe\CCE2_ReadMe.html";
            if (File.Exists(helpFile))
            {
                using (HelpForm helpForm = new HelpForm(helpFile))
                {
                    helpForm.Text = "Read Me";
                    helpForm.ShowDialog(this);
                }
            }
        }

        protected static void ParseCastAndCrew(DefaultValues defaultValues
            , String key
            , Boolean parseCast
            , Boolean parseCrew
            , Boolean parseSoundtrack
            , Boolean initializeLists
            , ref List<Match> castMatches
            , ref List<CastInfo> castList
            , ref List<KeyValuePair<Match, List<Match>>> crewMatches
            , ref List<CrewInfo> crewList
            , ref Dictionary<String, List<Match>> soundtrackMatches)
        {
            String targetUrl = IMDbParser.TitleUrl + key + "/fullcredits";

            String webSite = IMDbParser.GetWebSite(targetUrl);

            if (initializeLists)
            {
                castList = new List<CastInfo>();
                crewList = new List<CrewInfo>();
            }
            castMatches = new List<Match>();
            crewMatches = new List<KeyValuePair<Match, List<Match>>>();
            soundtrackMatches = new Dictionary<String, List<Match>>();
            #region Parse for Cast
            if (parseCast)
            {
                using (StringReader sr = new StringReader(webSite))
                {
                    while (sr.Peek() != -1)
                    {
                        String line = ReadLine(sr);
                        if (CastBlockStartRegex.Match(line).Success)
                        {
                            StringBuilder block;
                            MatchCollection lineMatches;

                            block = new StringBuilder();
                            block.Append(line.Trim());
                            while ((BlockEndRegex.Match(line).Success == false) && (sr.Peek() != -1))
                            {
                                line = ReadLine(sr);
                                block.Append(line.Trim());
                            }
                            lineMatches = CastLineRegex.Matches(block.ToString());
                            if (lineMatches.Count > 0)
                            {
                                foreach (Match lineMatch in lineMatches)
                                {
                                    if (lineMatch.Success)
                                    {
                                        IMDbParser.ProcessCastLine(lineMatch.Value, castList
                                            , castMatches, defaultValues);
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
                using (StringReader sr = new StringReader(webSite.ToString()))
                {
                    while (sr.Peek() != -1)
                    {
                        string line = ReadLine(sr);
                        if (CrewBlockStartRegex.Match(line).Success)
                        {
                            StringBuilder block;

                            block = new StringBuilder();
                            block.Append(line.Trim());
                            while ((BlockEndRegex.Match(line).Success == false) && (sr.Peek() != -1))
                            {
                                line = ReadLine(sr);
                                block.Append(line.Trim());
                            }
                            IMDbParser.ProcessCrewLine(block.ToString(), crewList, crewMatches
                                , defaultValues);
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

            var openTagCount = line.Count(c => c == '<');

            var closeTagCount = line.Count(c => c == '>');

            while (openTagCount > closeTagCount && sr.Peek() != -1)
            {
                line += sr.ReadLine();

                openTagCount = line.Count(c => c == '<');

                closeTagCount = line.Count(c => c == '>');

                if (line.Contains("</script>"))
                {
                    break;
                }
            }

            return line;
        }


        private void EditConfigFile(String fileName, String name, FileNameType fileNameType, Boolean createFile)
        {
            if ((createFile) && (File.Exists(fileName) == false))
            {
                using (FileStream temp = File.Create(fileName))
                {
                }
            }
            if (File.Exists(fileName))
            {
                switch (fileNameType)
                {
                    case (FileNameType.KnownNames):
                        {
                            using (EditKnownNamesConfigFileForm editForm = new EditKnownNamesConfigFileForm(fileName, name))
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
                            using (EditConfigFileForm editForm = new EditConfigFileForm(fileName, name))
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
                MessageBox.Show(this, String.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void OnReadmeToolStripMenuItemClick(Object sender, EventArgs e)
        {
            OpenReadme();
        }

        protected void OnIMDbToDVDProfilerTransformationDataToolStripMenuItemClick(Object sender, EventArgs e)
        {
            string fileName;

            fileName = Application.StartupPath + "\\EditIMDbToDVDProfilerCrewRoleTransformation.exe";
            if (File.Exists(fileName))
            {
                Process process;
                Int32 counter;

                process = new Process();
                process.StartInfo = new ProcessStartInfo(fileName);
                process.Start();
                counter = 0;
                while (process.HasExited == false)
                {
                    counter++;
                    if (counter % 8 == 0)
                    {
                        Refresh();
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
                MessageBox.Show(this, String.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void OnKnownNamesToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\KnownNames.txt", EditWindowNames.KnownNames
                , FileNameType.KnownNames, false);
        }

        protected void OnLastnameSuffixesToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\KnownLastNameSuffixes.txt", EditWindowNames.KnownLastnameSuffixes
                           , FileNameType.LastnameSuffixes, false);
        }

        protected void OnLastnamePrefixesToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\KnownLastnamePrefixes.txt", EditWindowNames.KnownLastnamePrefixes
               , FileNameType.LastnamePrefixes, false);
        }

        protected void OnFirstnamePrefixesToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\KnownFirstnamePrefixes.txt", EditWindowNames.KnownFirstnamePrefixes
               , FileNameType.FirstnamePrefixes, false);
        }

        protected void OnIgnoreCustomInIMDbCreditTypeToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\IgnoreCustomInIMDbCategory.txt", EditWindowNames.IgnoreCustominIMDbCategory
               , FileNameType.IgnoreCustomInIMDbCreditType, true);
        }

        protected void OnIgnoreIMDbCreditTypeInOtherToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\IgnoreIMDbCategoryInOther.txt", EditWindowNames.IgnoreIMDbCategoryinOther
               , FileNameType.IgnoreIMDbCreditTypeInOther, true);
        }

        protected void OnForcedFakeBirthYearsToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EditConfigFile(Program.RootPath + @"\Data\ForcedFakeBirthYears.txt", EditWindowNames.IgnoreIMDbCategoryinOther
               , FileNameType.ForcedFakeBirthYears, true);
        }

        protected static Dictionary<String, List<Match>> ParseSoundtrack(String titleLink)
        {
            Dictionary<String, List<Match>> soundtrackEntries;

            soundtrackEntries = null;

            String soundtrackUrl = IMDbParser.TitleUrl + titleLink + "/soundtrack";

            String webSite = IMDbParser.GetWebSite(soundtrackUrl);

            using (StringReader sr = new StringReader(webSite))
            {
                StringBuilder soundtrack;
                Boolean soundtrackFound;

                soundtrackFound = false;
                soundtrack = new StringBuilder();
                while (sr.Peek() != -1)
                {
                    String line;
                    Match beginMatch;
                    //Match endMatch;

                    line = ReadLine(sr);
                    if (soundtrackFound == false)
                    {
                        beginMatch = IMDbParser.SoundtrackStartRegex.Match(line);
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
                    soundtrackEntries = IMDbParser.ParseSoundtrack(soundtrack);
                }
            }
            if (soundtrackEntries == null)
            {
                soundtrackEntries = new Dictionary<String, List<Match>>(0);
            }
            return (soundtrackEntries);
        }

        #region Progress

        #region SetProgress

#if UnitTest
        public void SetProgress()
#else
        protected void SetProgress()
#endif
        {
            if (SuppressProgress == false)
            {
                SetProgressSafe();
            }
        }

        private void SetProgressSafe()
        {
            lock (ProgressLock)
            {
                ProgressValue++;

                if ((ProgressValue == ProgressMax) || ((ProgressValue % ProgressInterval) == 0))
                {
                    SetProgressBarValue();
                }
            }
        }

        private void SetProgressBarValue()
        {
            if (InvokeRequired)
            {
                Invoke(new SetProgress(SetProgressBarValueSync));
            }
            else
            {
                SetProgressBarValueSync();
            }
        }

        private void SetProgressBarValueSync()
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressValue(ProgressValue, ProgressMax);
            }

            TheProgressBar.Value = ProgressValue;

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

            TheProgressBar.Value = 0;
            TheProgressBar.Text = String.Empty;
            TheProgressBar.BarColor = SystemColors.Control;

            Application.DoEvents();
        }

        #region StartProgress

        protected void StartProgress(Int32 maxValue
            , Color color)
        {
            ProgressValue = 0;

            ProgressMax = maxValue;

            CalculateUpdateInterval();

            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.OwnerHandle = Handle;
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                TaskbarManager.Instance.SetProgressValue(ProgressValue, ProgressMax);
            }

            TheProgressBar.Minimum = ProgressValue;
            TheProgressBar.Maximum = ProgressMax;
            TheProgressBar.BarColor = color;

            Application.DoEvents();
        }

        protected void RestartProgress()
        {
            StartProgress(ProgressMax, TheProgressBar.BarColor);
        }

        private void CalculateUpdateInterval()
        {
            ProgressInterval = 1;

            if (ProgressMax > OneHundred)
            {
                ProgressInterval = ProgressMax / OneHundred;

                if (IsNotDivisibleWithoutRemainder())
                {
                    //We don't want to finish after we've reached 100%
                    ProgressInterval++;
                }
            }
        }

        private Boolean IsNotDivisibleWithoutRemainder()
            => ((ProgressMax % OneHundred) != 0);

        #endregion

        #endregion

        protected void ProcessLines(List<CastInfo> castList
            , List<Match> castMatches
            , List<CrewInfo> crewList
            , List<KeyValuePair<Match, List<Match>>> crewMatches
            , Dictionary<String, List<Match>> sountrackMatches
            , DefaultValues defaultValues)
        {
            IMDbParser.ProcessCastLine(castList, castMatches, defaultValues, SetProgress);

            IMDbParser.ProcessCrewLine(crewList, crewMatches, defaultValues, SetProgress);

            IMDbParser.ProcessSoundtrackLine(crewList, sountrackMatches, defaultValues, SetProgress);

            //Task castTask = Task.Run(() => IMDbParser.ProcessCastLine(castList, castMatches, defaultValues, SetProgress));

            //Task crewTask = Task.Run(() =>
            //        {
            //            IMDbParser.ProcessCrewLine(crewList, crewMatches, defaultValues, SetProgress);
            //            IMDbParser.ProcessSoundtrackLine(crewList, sountrackMatches, defaultValues, SetProgress);
            //        }
            //    );

            //while (castTask.IsCompleted == false)
            //{
            //    Application.DoEvents();
            //}
            //while (crewTask.IsCompleted == false)
            //{
            //    Application.DoEvents();
            //}
        }

        protected void StartLongAction()
        {
            Enabled = false;

            UseWaitCursor = true;

            Cursor = Cursors.WaitCursor;

            Application.DoEvents();
        }

        protected void EndLongAction()
        {
            UseWaitCursor = false;

            Cursor = Cursors.Default;

            Enabled = true;

            Application.DoEvents();
        }
    }
}