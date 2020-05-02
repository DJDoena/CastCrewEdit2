using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    internal partial class EpisodeForm : CastCrewEdit2ParseBaseForm
    {
        private List<EpisodeInfo> Episodes;

        public EpisodeForm(List<EpisodeInfo> episodes)
        {
            Episodes = episodes;

            InitializeComponent();

            TheProgressBar = ProgressBar;

            Icon = Properties.Resource.djdsoft;
        }

        private void OnFormLoad(Object sender, EventArgs e)
        {
            ComponentResourceManager resources;

            SuspendLayout();
            LayoutForm();
            CreateDataGridViewColumns();
            SetCheckBoxes();
            ResumeLayout();
            RegisterEvents();
            resources = new ComponentResourceManager(typeof(EpisodeForm));
            if (String.IsNullOrEmpty(TVShowTitle) == false)
            {
                Text = resources.GetString("$Text") + " - " + TVShowTitle;
            }
            else
            {
                Text = resources.GetString("$Text");
            }
        }

        private void RegisterEvents()
        {
            CastDataGridView.CellValueChanged += OnCastDataGridViewCellValueChanged;
            CastDataGridView.CellContentClick += OnDataGridViewCellContentClick;
            CrewDataGridView.CellValueChanged += OnCrewDataGridViewCellValueChanged;
            CrewDataGridView.CellContentClick += OnDataGridViewCellContentClick;
            SettingsToolStripMenuItem.Click += OnSettingsToolStripMenuItemClick;
            FirstnamePrefixesToolStripMenuItem.Click += OnFirstnamePrefixesToolStripMenuItemClick;
            LastnamePrefixesToolStripMenuItem.Click += OnLastnamePrefixesToolStripMenuItemClick;
            LastnameSuffixesToolStripMenuItem.Click += OnLastnameSuffixesToolStripMenuItemClick;
            KnownNamesToolStripMenuItem.Click += OnKnownNamesToolStripMenuItemClick;
            IgnoreCustomInIMDbCreditTypeToolStripMenuItem.Click += OnIgnoreCustomInIMDbCreditTypeToolStripMenuItemClick;
            IgnoreIMDbCreditTypeInOtherToolStripMenuItem.Click += OnIgnoreIMDbCreditTypeInOtherToolStripMenuItemClick;
            ForcedFakeBirthYearsToolStripMenuItem.Click += OnForcedFakeBirthYearsToolStripMenuItemClick;
            IMDbToDVDProfilerTransformationDataToolStripMenuItem.Click
                += OnIMDbToDVDProfilerTransformationDataToolStripMenuItemClick;
            ReadmeToolStripMenuItem.Click += OnReadmeToolStripMenuItemClick;
            AboutToolStripMenuItem.Click += OnAboutToolStripMenuItemClick;
            BirthYearsInLocalCacheLabel.LinkClicked += OnBirthYearsInLocalCacheLabelLinkClicked;
            PersonsInLocalCacheLabel.LinkClicked += OnPersonsInLocalCacheLabelLinkClicked;
        }

        private void CreateDataGridViewColumns()
        {
            DataGridViewHelper.CreateCastColumns(CastDataGridView);
            DataGridViewHelper.CreateCrewColumns(CrewDataGridView);
        }

        private void LayoutForm()
        {
            if (Program.Settings.EpisodeForm.WindowState == FormWindowState.Normal)
            {
                Left = Program.Settings.EpisodeForm.Left;
                Top = Program.Settings.EpisodeForm.Top;
                if (Program.Settings.EpisodeForm.Width > MinimumSize.Width)
                {
                    Width = Program.Settings.EpisodeForm.Width;
                }
                else
                {
                    Width = MinimumSize.Width;
                }
                if (Program.Settings.EpisodeForm.Height > MinimumSize.Height)
                {
                    Height = Program.Settings.EpisodeForm.Height;
                }
                else
                {
                    Height = MinimumSize.Height;
                }
            }
            else
            {
                Left = Program.Settings.EpisodeForm.RestoreBounds.X;
                Top = Program.Settings.EpisodeForm.RestoreBounds.Y;
                if (Program.Settings.EpisodeForm.RestoreBounds.Width > MinimumSize.Width)
                {
                    Width = Program.Settings.EpisodeForm.RestoreBounds.Width;
                }
                else
                {
                    Width = MinimumSize.Width;
                }
                if (Program.Settings.EpisodeForm.RestoreBounds.Height > MinimumSize.Height)
                {
                    Height = Program.Settings.EpisodeForm.RestoreBounds.Height;
                }
                else
                {
                    Height = MinimumSize.Height;
                }
            }
            if (Program.Settings.EpisodeForm.WindowState != FormWindowState.Minimized)
            {
                WindowState = Program.Settings.EpisodeForm.WindowState;
            }
        }

        private void SetCheckBoxes()
        {
            ParseCastCheckBox.Checked = Program.Settings.DefaultValues.ParseCast;
            ParseCrewCheckBox.Checked = Program.Settings.DefaultValues.ParseCrew;
            ParseRoleSlashCheckBox.Checked = Program.Settings.DefaultValues.ParseRoleSlash;
            ParseVoiceOfCheckBox.Checked = Program.Settings.DefaultValues.ParseVoiceOf;
            IgnoreUncreditedCheckBox.Checked = Program.Settings.DefaultValues.IgnoreUncredited;
            IgnoreCreditOnlyCheckBox.Checked = Program.Settings.DefaultValues.IgnoreCreditOnly;
            IgnoreScenesDeletedCheckBox.Checked = Program.Settings.DefaultValues.IgnoreScenesDeleted;
            IgnoreArchiveFootageCheckBox.Checked = Program.Settings.DefaultValues.IgnoreArchiveFootage;
            IgnoreLanguageVersionCheckBox.Checked = Program.Settings.DefaultValues.IgnoreLanguageVersion;
            IgnoreUnconfirmedCheckBox.Checked = Program.Settings.DefaultValues.IgnoreUnconfirmed;
            RetainCreditedAsOnCastCheckBox.Checked = Program.Settings.DefaultValues.RetainCastCreditedAs;
            CustomCreditsCheckBox.Checked = Program.Settings.DefaultValues.IncludeCustomCredits;
            RetainOriginalCreditCheckBox.Checked = Program.Settings.DefaultValues.RetainOriginalCredit;
            IncludePrefixOnOtherCreditsCheckBox.Checked
                = Program.Settings.DefaultValues.IncludePrefixOnOtherCredits;
            CapitalizeCustomRoleCheckBox.Checked = Program.Settings.DefaultValues.CapitalizeCustomRole;
            RetainCreditedAsOnCrewCheckBox.Checked = Program.Settings.DefaultValues.RetainCrewCreditedAs;
            CreditTypeDirectionCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeDirection;
            CreditTypeWritingCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeWriting;
            CreditTypeProductionCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeProduction;
            CreditTypeCinematographyCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeCinematography;
            CreditTypeFilmEditingCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeFilmEditing;
            CreditTypeMusicCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeMusic;
            CreditTypeSoundCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeSound;
            CreditTypeArtCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeArt;
            CreditTypeOtherCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeOther;
            CreditTypeSoundtrackCheckBox.Checked = Program.Settings.DefaultValues.CreditTypeSoundtrack;
        }

        private void FillRows()
        {
            CastDataGridView.Rows.Clear();

            CrewDataGridView.Rows.Clear();

            CreateCastTitleRow();

            CreateCrewTitleRow();

            for (Int32 i = 0; i < Episodes.Count; i++)
            {
                UpdateUI(Episodes[i], i == 0, i == Episodes.Count - 1);
            }
        }

        private void UpdateUI(EpisodeInfo episode, Boolean isFirstDivider, Boolean isLastDivider)
        {
            UpdateCastUI(episode, isFirstDivider, isLastDivider);
            UpdateCrewUI(episode);
            UpdateUI(episode.CastList, episode.CrewList, CastDataGridView, CrewDataGridView
                , ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, TVShowTitleLink, TVShowTitle);
            if (Log.Length > 0)
            {
                Log.Show(LogWebBrowser);
            }
        }

        private void UpdateCrewUI(EpisodeInfo episode)
        {
            #region Update Crew UI
            if (ParseCrewCheckBox.Checked)
            {
                CrewInfo divider;

                divider = new CrewInfo();
                divider.FirstName = FirstNames.Divider;
                GetEpisodeTitle(episode, divider);
                divider.BirthYear = String.Empty;
                divider.CreditType = null;
                divider.CreditSubtype = null;
                divider.CreditedAs = String.Empty;
                divider.CustomRole = String.Empty;
                divider.PersonLink = episode.Link;
                DataGridViewHelper.FillCrewRows(CrewDataGridView
                    , new List<CrewInfo>(new CrewInfo[] { divider }));
            }
            #endregion
        }

        private static void GetEpisodeTitle(EpisodeInfo episode, PersonInfo divider)
        {
            String text;

            text = Program.Settings.DefaultValues.EpisodeDividerFormat;
            text = text.Replace("{season}", episode.SeasonNumber);
            if (Program.Settings.DefaultValues.UseDoubleDigitsEpisodeNumber)
            {
                text = text.Replace("{episode}", episode.EpisodeNumber.PadLeft(2, '0'));
            }
            else
            {
                text = text.Replace("{episode}", episode.EpisodeNumber);
            }
            divider.MiddleName = text;
            divider.LastName = episode.EpisodeName;
        }

        private void UpdateCastUI(EpisodeInfo episode, Boolean isFirstDivider, Boolean isLastDivider)
        {
            #region Update Cast UI
            if (ParseCastCheckBox.Checked)
            {
                CastInfo divider;

                divider = new CastInfo(episode.Identifier);
                divider.FirstName = FirstNames.Divider;
                GetEpisodeTitle(episode, divider);
                divider.BirthYear = String.Empty;
                divider.Role = String.Empty;
                divider.Voice = "False";
                divider.Uncredited = "False";
                divider.CreditedAs = String.Empty;
                divider.PersonLink = episode.Link;
                DataGridViewHelper.FillCastRows(CastDataGridView, new List<CastInfo>(new CastInfo[] { divider })
                    , isFirstDivider, isLastDivider);
            }
            #endregion
        }

        private void CreateCastTitleRow()
        {
            #region Title for Cast
            if (ParseCastCheckBox.Checked)
            {
                CastInfo title;

                title = new CastInfo(-1);
                title.FirstName = FirstNames.Title;
                title.MiddleName = String.Empty;
                title.LastName = TVShowTitle;
                title.BirthYear = String.Empty;
                title.Role = String.Empty;
                title.Voice = "False";
                title.Uncredited = "False";
                title.CreditedAs = String.Empty;
                title.PersonLink = TVShowTitleLink;
                DataGridViewHelper.FillCastRows(CastDataGridView, new List<CastInfo>(new CastInfo[] { title })
                    , false, false);
            }
            #endregion
        }

        private void CreateCrewTitleRow()
        {
            #region Title for Crew
            if (ParseCrewCheckBox.Checked)
            {
                CrewInfo title;

                title = new CrewInfo();
                title.FirstName = FirstNames.Title;
                title.MiddleName = String.Empty;
                title.LastName = TVShowTitle;
                title.BirthYear = String.Empty;
                title.CreditType = null;
                title.CreditSubtype = null;
                title.CreditedAs = String.Empty;
                title.CustomRole = String.Empty;
                title.PersonLink = TVShowTitleLink;
                DataGridViewHelper.FillCrewRows(CrewDataGridView, new List<CrewInfo>(new CrewInfo[] { title }));
            }
            #endregion
        }

        private void OnCastFormClosing(Object sender, FormClosingEventArgs e)
        {
            Program.Settings.EpisodeForm.Left = Left;
            Program.Settings.EpisodeForm.Top = Top;
            Program.Settings.EpisodeForm.Width = Width;
            Program.Settings.EpisodeForm.Height = Height;
            Program.Settings.EpisodeForm.WindowState = WindowState;
            Program.Settings.EpisodeForm.RestoreBounds = RestoreBounds;
        }

        private void OnCastGenerateButtonClick(Object sender
            , EventArgs e)
        {
            if (HasAgreed == false)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            DataGridViewHelper.CopyCastToClipboard(CastDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, false);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CastDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader, MessageBoxButtons.OK
                    , MessageBoxIcon.Information);
            }
        }

        private void OnCloseButtonClick(Object sender, EventArgs e)
        {
            Close();
        }

        private void OnGetBirthYearsButtonClick(Object sender, EventArgs e)
        {
            GetBirthYears(false, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton
                , LogWebBrowser);
            ProcessMessageQueue();
        }

        private void OnFormShown(Object sender
            , EventArgs e)
        {
            StartLongAction();

            FillRows();

            BirthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;

            PersonsInLocalCacheLabel.Text = Program.PersonCacheCountString;

            if (Log.Length > 0)
            {
                Log.Show(LogWebBrowser);
            }

            EndLongActionWithGrids();

            if ((Program.Settings.DefaultValues.DisableParsingCompleteMessageBox == false)
                && (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing == false)
                && (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing == false))
            {
                ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK
                    , MessageBoxIcon.Information);
            }

            if (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                GetBirthYears(Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing, CastDataGridView, CrewDataGridView
                    , BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
            }

            if (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                OnGetHeadshotsButtonClick(sender, e);
            }

            ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(CastDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(CrewDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
        }

        private void EndLongActionWithGrids()
        {
            EndLongAction();

            CastDataGridView.Refresh();
            CrewDataGridView.Refresh();
        }

        private void OnCastDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewHelper.OnCastDataGridViewCellValueChanged(sender, e);
        }

        private void OnCrewDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewHelper.OnCrewDataGridViewCellValueChanged(sender, e);
        }

        private void OnCrewGenerateButtonClick(Object sender
            , EventArgs e)
        {
            if (HasAgreed == false)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            HasAgreed = true;

            DataGridViewHelper.CopyCrewToClipboard(CrewDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, false);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CrewDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnSettingsToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm(true, true))
            {
                settingsForm.SetValues(Program.Settings.SettingsForm.Left, Program.Settings.SettingsForm.Top
                    , Program.Settings.DefaultValues);
                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    SetCheckBoxes();
                    SettingsHaveChanged = true;
                }
                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnReApplySettingsAndFiltersButtonClick(Object sender, EventArgs e)
        {
            StartLongAction();

            DefaultValues defaultValues = CopyDefaultValues();

            Int32 progressMax = 0;

            foreach (EpisodeInfo episode in Episodes)
            {
                progressMax += episode.CastMatches.Count;

                foreach (KeyValuePair<Match, List<Match>> kvp in episode.CrewMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                foreach (KeyValuePair<String, List<Match>> kvp in episode.SoundtrackMatches)
                {
                    progressMax += kvp.Value.Count;
                }
            }

            StartProgress(progressMax, Color.LightBlue);

            foreach (EpisodeInfo episode in Episodes)
            {
                episode.CastList = new List<CastInfo>();
                episode.CrewList = new List<CrewInfo>();

                ProcessLines(episode.CastList, episode.CastMatches, episode.CrewList, episode.CrewMatches, episode.SoundtrackMatches, defaultValues);
            }

            FillRows();

            if (Log.Length > 0)
            {
                Log.Show(LogWebBrowser);
            }

            EndLongActionWithGrids();

            if ((Program.Settings.DefaultValues.DisableParsingCompleteMessageBox == false)
                && (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing == false)
                && (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing == false))
            {
                ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK
                    , MessageBoxIcon.Information);
            }

            if (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                GetBirthYears(Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing, CastDataGridView, CrewDataGridView
                    , BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
            }
            else
            {
                Program.FlushPersonCache();
            }

            if (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                GetHeadshots(CastDataGridView, CrewDataGridView, GetHeadshotsButton);
            }

            ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(CastDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(CrewDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
        }

        private DefaultValues CopyDefaultValues()
        {
            DefaultValues defaultValues;
            defaultValues = new DefaultValues();
            defaultValues.ParseRoleSlash = ParseRoleSlashCheckBox.Checked;
            defaultValues.ParseVoiceOf = ParseVoiceOfCheckBox.Checked;
            defaultValues.IgnoreUncredited = IgnoreUncreditedCheckBox.Checked;
            defaultValues.IgnoreCreditOnly = IgnoreCreditOnlyCheckBox.Checked;
            defaultValues.IgnoreScenesDeleted = IgnoreScenesDeletedCheckBox.Checked;
            defaultValues.IgnoreArchiveFootage = IgnoreArchiveFootageCheckBox.Checked;
            defaultValues.IgnoreLanguageVersion = IgnoreLanguageVersionCheckBox.Checked;
            defaultValues.IgnoreUnconfirmed = IgnoreUnconfirmedCheckBox.Checked;
            defaultValues.RetainCastCreditedAs = RetainCreditedAsOnCastCheckBox.Checked;
            defaultValues.IncludeCustomCredits = CustomCreditsCheckBox.Checked;
            defaultValues.RetainOriginalCredit = RetainOriginalCreditCheckBox.Checked;
            defaultValues.IncludePrefixOnOtherCredits
                = IncludePrefixOnOtherCreditsCheckBox.Checked;
            defaultValues.CapitalizeCustomRole = CapitalizeCustomRoleCheckBox.Checked;
            defaultValues.RetainCrewCreditedAs = RetainCreditedAsOnCrewCheckBox.Checked;
            defaultValues.CreditTypeDirection = CreditTypeDirectionCheckBox.Checked;
            defaultValues.CreditTypeWriting = CreditTypeWritingCheckBox.Checked;
            defaultValues.CreditTypeProduction = CreditTypeProductionCheckBox.Checked;
            defaultValues.CreditTypeCinematography = CreditTypeCinematographyCheckBox.Checked;
            defaultValues.CreditTypeFilmEditing = CreditTypeFilmEditingCheckBox.Checked;
            defaultValues.CreditTypeMusic = CreditTypeMusicCheckBox.Checked;
            defaultValues.CreditTypeSound = CreditTypeSoundCheckBox.Checked;
            defaultValues.CreditTypeArt = CreditTypeArtCheckBox.Checked;
            defaultValues.CreditTypeOther = CreditTypeOtherCheckBox.Checked;
            defaultValues.CreditTypeSoundtrack = CreditTypeSoundtrackCheckBox.Checked;
            defaultValues.CheckPersonLinkForRedirect = Program.Settings.DefaultValues.CheckPersonLinkForRedirect;
            return (defaultValues);
        }

        protected override void RemoveRow(CastInfo castMember)
        {
            Int32 index;

            index = -1;
            foreach (EpisodeInfo episode in Episodes)
            {
                index = FindIndexOfCastMember(episode.CastList, castMember);
                if (index != -1)
                {
                    episode.CastList.RemoveAt(index);
                    UpdateUI();
                    break;
                }
            }
            if (index == -1)
            {
                Debug.Assert(false, "Invalid Index");
            }
        }

        protected override void MoveRow(CastInfo castMember
            , Boolean up)
        {
            Int32 index = -1;

            if (castMember.FirstName == FirstNames.Divider)
            {
                for (Int32 i = 0; i < Episodes.Count; i++)
                {
                    if (Episodes[i].Identifier == castMember.Identifier)
                    {
                        index = i;

                        break;
                    }
                }

                if (index != -1)
                {
                    EpisodeInfo temp = Episodes[index];

                    if (up)
                    {
                        Episodes[index] = Episodes[index - 1];

                        Episodes[index - 1] = temp;
                    }
                    else
                    {
                        Episodes[index] = Episodes[index + 1];

                        Episodes[index + 1] = temp;
                    }

                    UpdateUI();
                }
                else
                {
                    Debug.Assert(false, "Invalid Index");
                }
            }
            else
            {
                foreach (EpisodeInfo episode in Episodes)
                {
                    index = FindIndexOfCastMember(episode.CastList, castMember);

                    if (index != -1)
                    {
                        CastInfo temp = episode.CastList[index];

                        if (up)
                        {
                            episode.CastList[index] = episode.CastList[index - 1];

                            episode.CastList[index - 1] = temp;
                        }
                        else
                        {
                            episode.CastList[index] = episode.CastList[index + 1];

                            episode.CastList[index + 1] = temp;
                        }

                        UpdateUI();

                        break;
                    }
                }

                if (index == -1)
                {
                    Debug.Assert(false, "Invalid Index");
                }
            }
        }

        private void UpdateUI()
        {
            CastDataGridView.Rows.Clear();

            CreateCastTitleRow();

            for (Int32 i = 0; i < Episodes.Count; i++)
            {
                UpdateCastUI(Episodes[i], i == 0, i == Episodes.Count - 1);

                UpdateUI(Episodes[i].CastList, null, CastDataGridView, null, true, false, TVShowTitleLink, TVShowTitle);
            }
        }

        private void OnGetHeadshotsButtonClick(Object sender, EventArgs e)
        {
            GetHeadshots(CastDataGridView, CrewDataGridView, GetHeadshotsButton);
        }

        private void OnLogWebBrowserNavigating(Object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri.StartsWith("https://www.imdb.com/"))
            {
                Process.Start(e.Url.AbsoluteUri);
                e.Cancel = true;
            }
        }

        private void OnCopyExtendedCastToClipboardToolStripMenuItemClick(Object sender
            , EventArgs e)
        {
            if (HasAgreed == false)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            DataGridViewHelper.CopyExtendedCastToClipboard(CastDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CastDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader, MessageBoxButtons.OK
                    , MessageBoxIcon.Information);
            }
        }

        private void OnCcopyExtendedCrewToClipboardToolStripMenuItemClick(Object sender
            , EventArgs e)
        {
            if (HasAgreed == false)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            HasAgreed = true;

            DataGridViewHelper.CopyExtendedCrewToClipboard(CrewDataGridView, TVShowTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CrewDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}