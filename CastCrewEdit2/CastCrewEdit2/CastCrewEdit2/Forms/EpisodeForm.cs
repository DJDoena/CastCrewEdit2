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

            this.InitializeComponent();

            _progressBar = ProgressBar;

            this.Icon = Properties.Resource.djdsoft;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.SuspendLayout();

            this.LayoutForm();

            this.CreateDataGridViewColumns();

            this.SetCheckBoxes();

            this.ResumeLayout();

            this.RegisterEvents();

            var resources = new ComponentResourceManager(typeof(EpisodeForm));

            if (!string.IsNullOrEmpty(_tvShowTitle))
            {
                this.Text = resources.GetString("$Text") + " - " + _tvShowTitle;
            }
            else
            {
                this.Text = resources.GetString("$Text");
            }
        }

        private void RegisterEvents()
        {
            CastDataGridView.CellValueChanged += this.OnCastDataGridViewCellValueChanged;
            CastDataGridView.CellContentClick += this.OnDataGridViewCellContentClick;

            CrewDataGridView.CellValueChanged += this.OnCrewDataGridViewCellValueChanged;
            CrewDataGridView.CellContentClick += this.OnDataGridViewCellContentClick;

            SettingsToolStripMenuItem.Click += this.OnSettingsToolStripMenuItemClick;

            FirstnamePrefixesToolStripMenuItem.Click += this.OnFirstnamePrefixesToolStripMenuItemClick;

            LastnamePrefixesToolStripMenuItem.Click += this.OnLastnamePrefixesToolStripMenuItemClick;

            LastnameSuffixesToolStripMenuItem.Click += this.OnLastnameSuffixesToolStripMenuItemClick;

            KnownNamesToolStripMenuItem.Click += this.OnKnownNamesToolStripMenuItemClick;

            IgnoreCustomInIMDbCreditTypeToolStripMenuItem.Click += this.OnIgnoreCustomInIMDbCreditTypeToolStripMenuItemClick;

            IgnoreIMDbCreditTypeInOtherToolStripMenuItem.Click += this.OnIgnoreIMDbCreditTypeInOtherToolStripMenuItemClick;

            ForcedFakeBirthYearsToolStripMenuItem.Click += this.OnForcedFakeBirthYearsToolStripMenuItemClick;

            IMDbToDVDProfilerTransformationDataToolStripMenuItem.Click += this.OnIMDbToDVDProfilerTransformationDataToolStripMenuItemClick;

            ReadmeToolStripMenuItem.Click += this.OnReadmeToolStripMenuItemClick;

            AboutToolStripMenuItem.Click += this.OnAboutToolStripMenuItemClick;

            BirthYearsInLocalCacheLabel.LinkClicked += this.OnBirthYearsInLocalCacheLabelLinkClicked;

            PersonsInLocalCacheLabel.LinkClicked += this.OnPersonsInLocalCacheLabelLinkClicked;
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
                this.Left = Program.Settings.EpisodeForm.Left;
                this.Top = Program.Settings.EpisodeForm.Top;

                if (Program.Settings.EpisodeForm.Width > this.MinimumSize.Width)
                {
                    this.Width = Program.Settings.EpisodeForm.Width;
                }
                else
                {
                    this.Width = this.MinimumSize.Width;
                }

                if (Program.Settings.EpisodeForm.Height > this.MinimumSize.Height)
                {
                    this.Height = Program.Settings.EpisodeForm.Height;
                }
                else
                {
                    this.Height = this.MinimumSize.Height;
                }
            }
            else
            {
                this.Left = Program.Settings.EpisodeForm.RestoreBounds.X;
                this.Top = Program.Settings.EpisodeForm.RestoreBounds.Y;

                if (Program.Settings.EpisodeForm.RestoreBounds.Width > this.MinimumSize.Width)
                {
                    this.Width = Program.Settings.EpisodeForm.RestoreBounds.Width;
                }
                else
                {
                    this.Width = this.MinimumSize.Width;
                }

                if (Program.Settings.EpisodeForm.RestoreBounds.Height > this.MinimumSize.Height)
                {
                    this.Height = Program.Settings.EpisodeForm.RestoreBounds.Height;
                }
                else
                {
                    this.Height = this.MinimumSize.Height;
                }
            }

            if (Program.Settings.EpisodeForm.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Program.Settings.EpisodeForm.WindowState;
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

            this.CreateCastTitleRow();

            this.CreateCrewTitleRow();

            for (var episodeIndex = 0; episodeIndex < _episodes.Count; episodeIndex++)
            {
                this.UpdateUI(_episodes[episodeIndex], episodeIndex == 0, episodeIndex == _episodes.Count - 1);
            }
        }

        private void UpdateUI(EpisodeInfo episode, bool isFirstDivider, bool isLastDivider)
        {
            this.UpdateCastUI(episode, isFirstDivider, isLastDivider);

            this.UpdateCrewUI(episode);

            this.UpdateUI(episode.CastList, episode.CrewList, CastDataGridView, CrewDataGridView, ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, _tvShowTitleLink, _tvShowTitle);

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
            Program.Settings.EpisodeForm.Left = this.Left;
            Program.Settings.EpisodeForm.Top = this.Top;
            Program.Settings.EpisodeForm.Width = this.Width;
            Program.Settings.EpisodeForm.Height = this.Height;
            Program.Settings.EpisodeForm.WindowState = this.WindowState;
            Program.Settings.EpisodeForm.RestoreBounds = this.RestoreBounds;
        }

        private void OnCastGenerateButtonClick(object sender, EventArgs e) => this.GenerateCastXml(true);

        private string GenerateCastXml(bool showMessageBox) => this.GenerateCastXml(CastDataGridView, _tvShowTitle, showMessageBox, LogWebBrowser);

        private void OnCrewGenerateButtonClick(object sender, EventArgs e) => this.GenerateCrewXml(true);

        private string GenerateCrewXml(bool showMessageBox) => this.GenerateCrewXml(CrewDataGridView, _tvShowTitle, showMessageBox, LogWebBrowser);

        private void OnCloseButtonClick(object sender, EventArgs e) => this.Close();

        private void OnGetBirthYearsButtonClick(object sender, EventArgs e)
        {
            this.GetBirthYears(false, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);

            this.ProcessMessageQueue();
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            this.StartLongAction();

            this.FillRows();

            BirthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;

            PersonsInLocalCacheLabel.Text = Program.PersonCacheCountString;

            if (_log.Length > 0)
            {
                _log.Show(LogWebBrowser);
            }

            this.EndLongActionWithGrids();

            if (!Program.DefaultValues.DisableParsingCompleteMessageBox
                && !Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing
                && !Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                this.ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                this.GetBirthYears(Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
            }

            if (Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                this.OnGetHeadshotsButtonClick(sender, e);
            }

            this.ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(CastDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(CrewDataGridView, _tvShowTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
        }

        private void EndLongActionWithGrids()
        {
            this.EndLongAction();

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
                    this.SetCheckBoxes();

                    _settingsHaveChanged = true;
                }

                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnReApplySettingsAndFiltersButtonClick(object sender, EventArgs e)
        {
            this.StartLongAction();

            var defaultValues = this.GetDefaultValues();

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

            this.StartProgress(progressMax, Color.LightBlue);

            foreach (var episode in _episodes)
            {
                episode.CastList = new List<CastInfo>();
                episode.CrewList = new List<CrewInfo>();

                this.ProcessLines(episode.CastList, episode.CastMatches, episode.CrewList, episode.CrewMatches, episode.SoundtrackMatches, defaultValues);
            }

            this.FillRows();

            if (_log.Length > 0)
            {
                _log.Show(LogWebBrowser);
            }

            this.EndLongActionWithGrids();

            if (!Program.DefaultValues.DisableParsingCompleteMessageBox
                && !Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing
                && !Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                this.ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                this.GetBirthYears(Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing, CastDataGridView, CrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
            }
            else
            {
                Program.FlushPersonCache();
            }

            if (Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                this.GetHeadshots(CastDataGridView, CrewDataGridView, GetHeadshotsButton);
            }

            this.ProcessMessageQueue();

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

                    this.UpdateUI();

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

                    this.UpdateUI();
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

                        this.UpdateUI();

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

            this.CreateCastTitleRow();

            for (var episodeIndex = 0; episodeIndex < _episodes.Count; episodeIndex++)
            {
                this.UpdateCastUI(_episodes[episodeIndex], episodeIndex == 0, episodeIndex == _episodes.Count - 1);

                this.UpdateUI(_episodes[episodeIndex].CastList, null, CastDataGridView, null, true, false, _tvShowTitleLink, _tvShowTitle);
            }
        }

        private void OnGetHeadshotsButtonClick(object sender, EventArgs e) => this.GetHeadshots(CastDataGridView, CrewDataGridView, GetHeadshotsButton);

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

            this.ProcessMessageQueue();

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

            this.ProcessMessageQueue();

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
                            var xml = this.GenerateCastXml(false);

                            await CastCrewCopyPasteSender.Send(xml);
                        }

                        if (ParseCrewCheckBox.Checked)
                        {
                            var xml = this.GenerateCrewXml(false);

                            await CastCrewCopyPasteSender.Send(xml);
                        }
                    }
                }
                else if (CtrlCWasPressed(e))
                {
                    if (TabControl.SelectedIndex == 0)
                    {
                        this.OnCastGenerateButtonClick(this, EventArgs.Empty);
                    }
                    else if (TabControl.SelectedIndex == 1)
                    {
                        this.OnCrewGenerateButtonClick(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}