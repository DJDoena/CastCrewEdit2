namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using Helper;
    using Resources;

    internal partial class EpisodeForm : CastCrewEdit2ParseBaseForm
    {
        private readonly List<EpisodeInfo> _episodes;

        public EpisodeForm(List<EpisodeInfo> episodes)
        {
            _episodes = episodes;

            InitializeComponent();

            _progressBar = ProgressBar;

            Icon = Properties.Resource.djdsoft;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            SuspendLayout();

            LayoutForm();

            CreateDataGridViewColumns();

            SetCheckBoxes();

            ResumeLayout();

            RegisterEvents();

            var resources = new ComponentResourceManager(typeof(EpisodeForm));

            if (!string.IsNullOrEmpty(_tvShowTitle))
            {
                Text = resources.GetString("$this.Text") + " - " + _tvShowTitle;
            }
            else
            {
                Text = resources.GetString("$this.Text");
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

            IMDbToDVDProfilerTransformationDataToolStripMenuItem.Click += OnIMDbToDVDProfilerTransformationDataToolStripMenuItemClick;

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
            ParseCastCheckBox.Checked = Program.DefaultValues.ParseCast;

            ParseCrewCheckBox.Checked = Program.DefaultValues.ParseCrew;

            ParseRoleSlashCheckBox.Checked = Program.DefaultValues.ParseRoleSlash;

            ParseVoiceOfCheckBox.Checked = Program.DefaultValues.ParseVoiceOf;

            IgnoreUncreditedCheckBox.Checked = Program.DefaultValues.IgnoreUncredited;

            IgnoreCreditOnlyCheckBox.Checked = Program.DefaultValues.IgnoreCreditOnly;

            IgnoreScenesDeletedCheckBox.Checked = Program.DefaultValues.IgnoreScenesDeleted;

            IgnoreArchiveFootageCheckBox.Checked = Program.DefaultValues.IgnoreArchiveFootage;

            IgnoreLanguageVersionCheckBox.Checked = Program.DefaultValues.IgnoreLanguageVersion;

            IgnoreUnconfirmedCheckBox.Checked = Program.DefaultValues.IgnoreUnconfirmed;

            RetainCreditedAsOnCastCheckBox.Checked = Program.DefaultValues.RetainCastCreditedAs;

            CustomCreditsCheckBox.Checked = Program.DefaultValues.IncludeCustomCredits;

            RetainOriginalCreditCheckBox.Checked = Program.DefaultValues.RetainOriginalCredit;

            IncludePrefixOnOtherCreditsCheckBox.Checked = Program.DefaultValues.IncludePrefixOnOtherCredits;

            CapitalizeCustomRoleCheckBox.Checked = Program.DefaultValues.CapitalizeCustomRole;

            RetainCreditedAsOnCrewCheckBox.Checked = Program.DefaultValues.RetainCrewCreditedAs;

            CreditTypeDirectionCheckBox.Checked = Program.DefaultValues.CreditTypeDirection;

            CreditTypeWritingCheckBox.Checked = Program.DefaultValues.CreditTypeWriting;

            CreditTypeProductionCheckBox.Checked = Program.DefaultValues.CreditTypeProduction;

            CreditTypeCinematographyCheckBox.Checked = Program.DefaultValues.CreditTypeCinematography;

            CreditTypeFilmEditingCheckBox.Checked = Program.DefaultValues.CreditTypeFilmEditing;

            CreditTypeMusicCheckBox.Checked = Program.DefaultValues.CreditTypeMusic;

            CreditTypeSoundCheckBox.Checked = Program.DefaultValues.CreditTypeSound;

            CreditTypeArtCheckBox.Checked = Program.DefaultValues.CreditTypeArt;

            CreditTypeOtherCheckBox.Checked = Program.DefaultValues.CreditTypeOther;

            CreditTypeSoundtrackCheckBox.Checked = Program.DefaultValues.CreditTypeSoundtrack;
        }

        private void FillRows()
        {
            CastDataGridView.Rows.Clear();

            CrewDataGridView.Rows.Clear();

            CreateCastTitleRow();

            CreateCrewTitleRow();

            for (var episodeIndex = 0; episodeIndex < _episodes.Count; episodeIndex++)
            {
                UpdateUI(_episodes[episodeIndex], episodeIndex == 0, episodeIndex == _episodes.Count - 1);
            }
        }

        private void UpdateUI(EpisodeInfo episode, bool isFirstDivider, bool isLastDivider)
        {
            UpdateCastUI(episode, isFirstDivider, isLastDivider);

            UpdateCrewUI(episode);

            UpdateUI(episode.CastList, episode.CrewList, CastDataGridView, CrewDataGridView, ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, _tvShowTitleLink, _tvShowTitle);

            if (_log.Length > 0)
            {
                _log.Show(LogWebBrowser);
            }
        }

        private void UpdateCrewUI(EpisodeInfo episode)
        {
            #region Update Crew UI

            if (ParseCrewCheckBox.Checked)
            {
                var divider = new CrewInfo()
                {
                    FirstName = FirstNames.Divider,
                    BirthYear = string.Empty,
                    CreditType = null,
                    CreditSubtype = null,
                    CreditedAs = string.Empty,
                    CustomRole = string.Empty,
                    PersonLink = episode.Link,
                };

                GetEpisodeTitle(episode, divider);

                DataGridViewHelper.FillCrewRows(CrewDataGridView, new List<CrewInfo>(new CrewInfo[] { divider }));
            }

            #endregion
        }

        private static void GetEpisodeTitle(EpisodeInfo episode, PersonInfo divider)
        {
            var text = Program.DefaultValues.EpisodeDividerFormat;

            text = text.Replace("{season}", episode.SeasonNumber);

            if (Program.DefaultValues.UseDoubleDigitsEpisodeNumber)
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

        private void UpdateCastUI(EpisodeInfo episode, bool isFirstDivider, bool isLastDivider)
        {
            #region Update Cast UI

            if (ParseCastCheckBox.Checked)
            {
                var divider = new CastInfo(episode.Identifier)
                {
                    FirstName = FirstNames.Divider,
                    BirthYear = string.Empty,
                    Role = string.Empty,
                    Voice = "False",
                    Uncredited = "False",
                    CreditedAs = string.Empty,
                    PersonLink = episode.Link,
                };

                GetEpisodeTitle(episode, divider);

                DataGridViewHelper.FillCastRows(CastDataGridView, new List<CastInfo>(new CastInfo[] { divider }), isFirstDivider, isLastDivider);
            }

            #endregion
        }

        private void CreateCastTitleRow()
        {
            #region Title for Cast

            if (ParseCastCheckBox.Checked)
            {
                var title = new CastInfo(-1)
                {
                    FirstName = FirstNames.Title,
                    MiddleName = string.Empty,
                    LastName = _tvShowTitle,
                    BirthYear = string.Empty,
                    Role = string.Empty,
                    Voice = "False",
                    Uncredited = "False",
                    CreditedAs = string.Empty,
                    PersonLink = _tvShowTitleLink,
                };

                DataGridViewHelper.FillCastRows(CastDataGridView, new List<CastInfo>(new CastInfo[] { title }), false, false);
            }

            #endregion
        }

        private void CreateCrewTitleRow()
        {
            #region Title for Crew

            if (ParseCrewCheckBox.Checked)
            {
                var title = new CrewInfo()
                {
                    FirstName = FirstNames.Title,
                    MiddleName = string.Empty,
                    LastName = _tvShowTitle,
                    BirthYear = string.Empty,
                    CreditType = null,
                    CreditSubtype = null,
                    CreditedAs = string.Empty,
                    CustomRole = string.Empty,
                    PersonLink = _tvShowTitleLink
                };

                DataGridViewHelper.FillCrewRows(CrewDataGridView, new List<CrewInfo>(new CrewInfo[] { title }));
            }

            #endregion
        }

        private void OnCastFormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Settings.EpisodeForm.Left = Left;
            Program.Settings.EpisodeForm.Top = Top;
            Program.Settings.EpisodeForm.Width = Width;
            Program.Settings.EpisodeForm.Height = Height;
            Program.Settings.EpisodeForm.WindowState = WindowState;
            Program.Settings.EpisodeForm.RestoreBounds = RestoreBounds;
        }

        private void OnCastGenerateButtonClick(object sender, EventArgs e) => GenerateCastXml(true);

        private string GenerateCastXml(bool showMessageBox) => GenerateCastXml(CastDataGridView, _tvShowTitle, showMessageBox, LogWebBrowser);

        private void OnCrewGenerateButtonClick(object sender, EventArgs e) => GenerateCrewXml(true);

        private string GenerateCrewXml(bool showMessageBox) => GenerateCrewXml(CrewDataGridView, _tvShowTitle, showMessageBox, LogWebBrowser);

        private void OnCloseButtonClick(object sender, EventArgs e) => Close();

        private void OnGetBirthYearsButtonClick(object sender, EventArgs e)
        {
            GetBirthYears(false, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);

            ProcessMessageQueue();
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            StartLongAction();

            FillRows();

            BirthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;

            PersonsInLocalCacheLabel.Text = Program.PersonCacheCountString;

            if (_log.Length > 0)
            {
                _log.Show(LogWebBrowser);
            }

            EndLongActionWithGrids();

            if (!Program.DefaultValues.DisableParsingCompleteMessageBox
                && !Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing
                && !Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                GetBirthYears(Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
            }

            if (Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                OnGetHeadshotsButtonClick(sender, e);
            }

            ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(CastDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(CrewDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
        }

        private void EndLongActionWithGrids()
        {
            EndLongAction();

            CastDataGridView.Refresh();

            CrewDataGridView.Refresh();
        }

        private void OnCastDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e) => DataGridViewHelper.OnCastDataGridViewCellValueChanged(sender, e);

        private void OnCrewDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e) => DataGridViewHelper.OnCrewDataGridViewCellValueChanged(sender, e);

        private void OnSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(true, true))
            {
                settingsForm.SetValues(Program.Settings.SettingsForm.Left, Program.Settings.SettingsForm.Top, Program.DefaultValues);

                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    SetCheckBoxes();

                    _settingsHaveChanged = true;
                }

                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnReApplySettingsAndFiltersButtonClick(object sender, EventArgs e)
        {
            StartLongAction();

            var defaultValues = GetDefaultValues();

            var progressMax = 0;

            foreach (var episode in _episodes)
            {
                progressMax += episode.CastMatches.Count;

                foreach (var kvp in episode.CrewMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                foreach (var kvp in episode.SoundtrackMatches)
                {
                    progressMax += kvp.Value.Count;
                }
            }

            StartProgress(progressMax, Color.LightBlue);

            foreach (var episode in _episodes)
            {
                episode.CastList = new List<CastInfo>();
                episode.CrewList = new List<CrewInfo>();

                ProcessLines(episode.CastList, episode.CastMatches, episode.CrewList, episode.CrewMatches, episode.SoundtrackMatches, defaultValues);
            }

            FillRows();

            if (_log.Length > 0)
            {
                _log.Show(LogWebBrowser);
            }

            EndLongActionWithGrids();

            if (!Program.DefaultValues.DisableParsingCompleteMessageBox
                && !Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing
                && !Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                GetBirthYears(Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
            }
            else
            {
                Program.FlushPersonCache();
            }

            if (Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                GetHeadshots(CastDataGridView, CrewDataGridView, GetHeadshotsButton);
            }

            ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(CastDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(CrewDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
        }

        private DefaultValues GetDefaultValues()
        {
            var defaultValues = DefaultValues.GetFromProgramSettings();

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
            defaultValues.IncludePrefixOnOtherCredits = IncludePrefixOnOtherCreditsCheckBox.Checked;
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

            return defaultValues;
        }

        protected override void RemoveRow(CastInfo castMember)
        {
            var index = -1;

            foreach (var episode in _episodes)
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

        protected override void MoveRow(CastInfo castMember, bool up)
        {
            var index = -1;

            if (castMember.FirstName == FirstNames.Divider)
            {
                for (var episodeIndex = 0; episodeIndex < _episodes.Count; episodeIndex++)
                {
                    if (_episodes[episodeIndex].Identifier == castMember.Identifier)
                    {
                        index = episodeIndex;

                        break;
                    }
                }

                if (index != -1)
                {
                    var temp = _episodes[index];

                    if (up)
                    {
                        _episodes[index] = _episodes[index - 1];

                        _episodes[index - 1] = temp;
                    }
                    else
                    {
                        _episodes[index] = _episodes[index + 1];

                        _episodes[index + 1] = temp;
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
                foreach (var episode in _episodes)
                {
                    index = FindIndexOfCastMember(episode.CastList, castMember);

                    if (index != -1)
                    {
                        var temp = episode.CastList[index];

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

            for (var episodeIndex = 0; episodeIndex < _episodes.Count; episodeIndex++)
            {
                UpdateCastUI(_episodes[episodeIndex], episodeIndex == 0, episodeIndex == _episodes.Count - 1);

                UpdateUI(_episodes[episodeIndex].CastList, null, CastDataGridView, null, true, false, _tvShowTitleLink, _tvShowTitle);
            }
        }

        private void OnGetHeadshotsButtonClick(object sender, EventArgs e) => GetHeadshots(CastDataGridView, CrewDataGridView, GetHeadshotsButton);

        private void OnLogWebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri.StartsWith("https://www.imdb.com/"))
            {
                Process.Start(e.Url.AbsoluteUri);

                e.Cancel = true;
            }
        }

        private void OnCopyExtendedCastToClipboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!HasAgreed)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            DataGridViewHelper.CopyExtendedCastToClipboard(CastDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage);

            _log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (!Program.DefaultValues.DisableCopyingSuccessfulMessageBox)
            {
                MessageBox.Show(this, MessageBoxTexts.CastDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnCcopyExtendedCrewToClipboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!HasAgreed)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            HasAgreed = true;

            DataGridViewHelper.CopyExtendedCrewToClipboard(CrewDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage);

            _log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (!Program.DefaultValues.DisableCopyingSuccessfulMessageBox)
            {
                MessageBox.Show(this, MessageBoxTexts.CrewDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void OnEpisodeFormKeyDown(object sender, KeyEventArgs e)
        {
            if (TabControl.Enabled)
            {
                if (CtrlSWasPressed(e))
                {
                    if (Program.DefaultValues.SendToCastCrewCopyPaste
                        && (TabControl.SelectedIndex == 0 || TabControl.SelectedIndex == 1))
                    {
                        if (ParseCastCheckBox.Checked)
                        {
                            var xml = GenerateCastXml(false);

                            await CastCrewCopyPasteSender.Send(xml);
                        }

                        if (ParseCrewCheckBox.Checked)
                        {
                            var xml = GenerateCrewXml(false);

                            await CastCrewCopyPasteSender.Send(xml);
                        }
                    }
                }
                else if (CtrlCWasPressed(e))
                {
                    if (TabControl.SelectedIndex == 0)
                    {
                        OnCastGenerateButtonClick(this, EventArgs.Empty);
                    }
                    else if (TabControl.SelectedIndex == 1)
                    {
                        OnCrewGenerateButtonClick(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}