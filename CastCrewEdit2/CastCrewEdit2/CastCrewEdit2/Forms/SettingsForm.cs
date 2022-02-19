namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using DVDProfilerHelper;
    using Resources;

    internal partial class SettingsForm : Form
    {
        private DefaultValues _defaultValues;

        private int _formLeft;

        private int _formTop;

        private readonly bool _showCastOptions;

        private readonly bool _showCrewOptions;

        public SettingsForm(bool castOptions, bool crewOptions)
        {
            _showCastOptions = castOptions;

            _showCrewOptions = crewOptions;

            this.InitializeComponent();

            this.Icon = Properties.Resource.djdsoft;
        }

        public void SetValues(int left, int top, DefaultValues defaultValues)
        {
            _formLeft = left;

            _formTop = top;

            _defaultValues = defaultValues;
        }

        public void GetValues(out int left, out int top)
        {
            left = _formLeft;

            top = _formTop;
        }

        private void LoadDataSettings()
        {
            ParseFirstNameInitialsIntoFirstAndMiddleNameCheckBox.Checked = _defaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName;

            TakeBirthYearFromLocalPersonCacheCheckBox.Checked = _defaultValues.TakeBirthYearFromLocalCache;

            RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Checked = _defaultValues.RetrieveBirthYearWhenLocalCacheEmpty;

            GetBirthYearsDirectlyAfterNameParsingCheckBox.Checked = _defaultValues.GetBirthYearsDirectlyAfterNameParsing;

            CheckLinksCheckBox.Checked = _defaultValues.CheckPersonLinkForRedirect;

            ParseCastCheckBox.Checked = _defaultValues.ParseCast;

            ParseRoleSlashCheckBox.Checked = _defaultValues.ParseRoleSlash;

            ParseVoiceOfCheckBox.Checked = _defaultValues.ParseVoiceOf;

            IgnoreUncreditedCheckBox.Checked = _defaultValues.IgnoreUncredited;

            IgnoreCreditOnlyCheckBox.Checked = _defaultValues.IgnoreCreditOnly;

            IgnoreScenesDeletedCheckBox.Checked = _defaultValues.IgnoreScenesDeleted;

            IgnoreArchiveFootageCheckBox.Checked = _defaultValues.IgnoreArchiveFootage;

            IgnoreLanguageVersionCheckBox.Checked = _defaultValues.IgnoreLanguageVersion;

            IgnoreUnconfirmedCheckBox.Checked = _defaultValues.IgnoreUnconfirmed;

            RetainCreditedAsOnCastCheckBox.Checked = _defaultValues.RetainCastCreditedAs;

            ParseCrewCheckBox.Checked = _defaultValues.ParseCrew;

            IncludingCustomCredits.Checked = _defaultValues.IncludeCustomCredits;

            RetainOriginalCreditCheckbox.Checked = _defaultValues.RetainOriginalCredit;

            IncludePrefixOnOtherCreditsCheckBox.Checked = _defaultValues.IncludePrefixOnOtherCredits;

            CapitalizeCustomRoleCheckBox.Checked = _defaultValues.CapitalizeCustomRole;

            RetainCreditedAsOnCrewCheckBox.Checked = _defaultValues.RetainCrewCreditedAs;

            IncludingCreditTypeDirectionCheckBox.Checked = _defaultValues.CreditTypeDirection;

            IncludingCreditTypeWritingCheckBox.Checked = _defaultValues.CreditTypeWriting;

            IncludingCreditTypeProductionCheckBox.Checked = _defaultValues.CreditTypeProduction;

            IncludingCreditTypeCinematographyCheckBox.Checked = _defaultValues.CreditTypeCinematography;

            IncludingCreditTypeFilmEditingCheckBox.Checked = _defaultValues.CreditTypeFilmEditing;

            IncludingCreditTypeMusicCheckBox.Checked = _defaultValues.CreditTypeMusic;

            IncludingCreditTypeSoundCheckBox.Checked = _defaultValues.CreditTypeSound;

            IncludingCreditTypeArtCheckBox.Checked = _defaultValues.CreditTypeArt;

            IncludingCreditTypeOtherCheckBox.Checked = _defaultValues.CreditTypeOther;

            IncludingCreditTypeSoundtrackCheckBox.Checked = _defaultValues.CreditTypeSoundtrack;

            DisableParsingCompleteMessageBoxCheckBox.Checked = _defaultValues.DisableParsingCompleteMessageBox;

            DisableParsingCompleteMessageBoxForGetBirthYearsCheckBox.Checked = _defaultValues.DisableParsingCompleteMessageBoxForGetBirthYears;

            DisableParsingCompleteMessageBoxForGetHeadshotsCheckBox.Checked = _defaultValues.DisableParsingCompleteMessageBoxForGetHeadshots;

            DisableCopyingSuccessfulMessageBoxCheckBox.Checked = _defaultValues.DisableCopyingSuccessfulMessageBox;

            DisableDuplicatesMessageBoxCheckBox.Checked = _defaultValues.DisableDuplicatesMessageBox;

            EpisodeFormatTextBox.Text = _defaultValues.EpisodeDividerFormat;

            UseDoubleDigitsEpisodeNumberCheckBox.Checked = _defaultValues.UseDoubleDigitsEpisodeNumber;

            DataPathTextBox.Text = Program.RootPath;

            GetCastHeadshotCheckBox.Checked = _defaultValues.GetCastHeadShots;

            GetCrewHeadshotCheckBox.Checked = _defaultValues.GetCrewHeadShots;

            AutoCopyHeadShotsCheckBox.Checked = _defaultValues.AutoCopyHeadShots;

            GetHeadshotsDirectlyAfterNameParsingCheckBox.Checked = _defaultValues.GetHeadShotsDirectlyAfterNameParsing;

            OverwriteExistingHeadshotsCheckBox.Checked = _defaultValues.OverwriteExistingImages;

            DownloadTriviaCheckBox.Checked = _defaultValues.DownloadTrivia;

            DownloadGoofsCheckBox.Checked = _defaultValues.DownloadGoofs;

            UseFakeBirthYearsCheckBox.Checked = _defaultValues.UseFakeBirthYears;

            SaveLogFileCheckBox.Checked = _defaultValues.SaveLogFile;

            StoreHeadshotsPerSessionCheckBox.Checked = _defaultValues.StoreHeadshotsPerSession;

            SendToCastCrewCopyPasteCheckBox.Checked = _defaultValues.SendToCastCrewCopyPaste;

            GroupSoundtrackCreditsCheckBox.Checked = _defaultValues.GroupSoundtrackCredits;

            StandardizeJuniorSeniorCheckBox.Checked = _defaultValues.StandardizeJuniorSenior;

            this.OnEpisodeFormatChanged(null, null);

            this.OnParseCastCheckBoxCheckedChanged(null, null);

            this.OnParseCrewCheckBoxCheckedChanged(null, null);

            this.OnTakeBirthYearFromLocalPersonCacheCheckBoxCheckedChanged(null, null);
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void OnAcceptButtonClick(object sender, EventArgs e)
        {
            this.SaveDataSettings();

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void SaveDataSettings()
        {
            _defaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName = ParseFirstNameInitialsIntoFirstAndMiddleNameCheckBox.Checked;
            _defaultValues.TakeBirthYearFromLocalCache = TakeBirthYearFromLocalPersonCacheCheckBox.Checked;
            _defaultValues.RetrieveBirthYearWhenLocalCacheEmpty = RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Checked;
            _defaultValues.GetBirthYearsDirectlyAfterNameParsing = GetBirthYearsDirectlyAfterNameParsingCheckBox.Checked;
            _defaultValues.CheckPersonLinkForRedirect = CheckLinksCheckBox.Checked;
            _defaultValues.ParseCast = ParseCastCheckBox.Checked;
            _defaultValues.ParseRoleSlash = ParseRoleSlashCheckBox.Checked;
            _defaultValues.ParseVoiceOf = ParseVoiceOfCheckBox.Checked;
            _defaultValues.IgnoreUncredited = IgnoreUncreditedCheckBox.Checked;
            _defaultValues.IgnoreCreditOnly = IgnoreCreditOnlyCheckBox.Checked;
            _defaultValues.IgnoreScenesDeleted = IgnoreScenesDeletedCheckBox.Checked;
            _defaultValues.IgnoreArchiveFootage = IgnoreArchiveFootageCheckBox.Checked;
            _defaultValues.IgnoreLanguageVersion = IgnoreLanguageVersionCheckBox.Checked;
            _defaultValues.IgnoreUnconfirmed = IgnoreUnconfirmedCheckBox.Checked;
            _defaultValues.RetainCastCreditedAs = RetainCreditedAsOnCastCheckBox.Checked;
            _defaultValues.ParseCrew = ParseCrewCheckBox.Checked;
            _defaultValues.IncludeCustomCredits = IncludingCustomCredits.Checked;
            _defaultValues.RetainOriginalCredit = RetainOriginalCreditCheckbox.Checked;
            _defaultValues.IncludePrefixOnOtherCredits = IncludePrefixOnOtherCreditsCheckBox.Checked;
            _defaultValues.CapitalizeCustomRole = CapitalizeCustomRoleCheckBox.Checked;
            _defaultValues.RetainCrewCreditedAs = RetainCreditedAsOnCrewCheckBox.Checked;
            _defaultValues.CreditTypeDirection = IncludingCreditTypeDirectionCheckBox.Checked;
            _defaultValues.CreditTypeWriting = IncludingCreditTypeWritingCheckBox.Checked;
            _defaultValues.CreditTypeProduction = IncludingCreditTypeProductionCheckBox.Checked;
            _defaultValues.CreditTypeCinematography = IncludingCreditTypeCinematographyCheckBox.Checked;
            _defaultValues.CreditTypeFilmEditing = IncludingCreditTypeFilmEditingCheckBox.Checked;
            _defaultValues.CreditTypeMusic = IncludingCreditTypeMusicCheckBox.Checked;
            _defaultValues.CreditTypeSound = IncludingCreditTypeSoundCheckBox.Checked;
            _defaultValues.CreditTypeArt = IncludingCreditTypeArtCheckBox.Checked;
            _defaultValues.CreditTypeOther = IncludingCreditTypeOtherCheckBox.Checked;
            _defaultValues.CreditTypeSoundtrack = IncludingCreditTypeSoundtrackCheckBox.Checked;
            _defaultValues.DisableParsingCompleteMessageBox = DisableParsingCompleteMessageBoxCheckBox.Checked;
            _defaultValues.DisableParsingCompleteMessageBoxForGetBirthYears = DisableParsingCompleteMessageBoxForGetBirthYearsCheckBox.Checked;
            _defaultValues.DisableParsingCompleteMessageBoxForGetHeadshots = DisableParsingCompleteMessageBoxForGetHeadshotsCheckBox.Checked;
            _defaultValues.DisableCopyingSuccessfulMessageBox = DisableCopyingSuccessfulMessageBoxCheckBox.Checked;
            _defaultValues.DisableDuplicatesMessageBox = DisableDuplicatesMessageBoxCheckBox.Checked;
            _defaultValues.EpisodeDividerFormat = EpisodeFormatTextBox.Text;
            _defaultValues.UseDoubleDigitsEpisodeNumber = UseDoubleDigitsEpisodeNumberCheckBox.Checked;
            _defaultValues.GetCastHeadShots = GetCastHeadshotCheckBox.Checked;
            _defaultValues.GetCrewHeadShots = GetCrewHeadshotCheckBox.Checked;
            _defaultValues.AutoCopyHeadShots = AutoCopyHeadShotsCheckBox.Checked;
            _defaultValues.GetHeadShotsDirectlyAfterNameParsing = GetHeadshotsDirectlyAfterNameParsingCheckBox.Checked;
            _defaultValues.OverwriteExistingImages = OverwriteExistingHeadshotsCheckBox.Checked;
            _defaultValues.DownloadTrivia = DownloadTriviaCheckBox.Checked;
            _defaultValues.DownloadGoofs = DownloadGoofsCheckBox.Checked;
            _defaultValues.UseFakeBirthYears = UseFakeBirthYearsCheckBox.Checked;
            _defaultValues.SaveLogFile = SaveLogFileCheckBox.Checked;
            _defaultValues.StoreHeadshotsPerSession = StoreHeadshotsPerSessionCheckBox.Checked;
            _defaultValues.SendToCastCrewCopyPaste = SendToCastCrewCopyPasteCheckBox.Checked;
            _defaultValues.GroupSoundtrackCredits = GroupSoundtrackCreditsCheckBox.Checked;
            _defaultValues.StandardizeJuniorSenior = StandardizeJuniorSeniorCheckBox.Checked;
        }

        private void OnSettingsFormLoad(object sender, EventArgs e)
        {
            this.Left = _formLeft;

            this.Top = _formTop;

            this.LoadDataSettings();

            if (!_showCastOptions)
            {
                SettingsTabControl.Controls.Remove(CastParsingTab);
            }

            if (!_showCrewOptions)
            {
                SettingsTabControl.Controls.Remove(CrewParsingTab);
            }

            if (!_showCastOptions || !_showCrewOptions)
            {
                SettingsTabControl.Controls.Remove(ParsingTab);
            }
        }

        private void OnSettingsFormClosing(object sender, FormClosingEventArgs e)
        {
            _formLeft = this.Left;

            _formTop = this.Top;
        }

        private void OnTakeBirthYearFromLocalPersonCacheCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (TakeBirthYearFromLocalPersonCacheCheckBox.Checked)
            {
                RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Enabled = true;
            }
            else
            {
                RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Checked = false;
                RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Enabled = false;
            }
        }

        private void OnEpisodeFormatChanged(object sender, EventArgs e)
        {
            EpisodeSeperatorSampleLabel.Text = EpisodeFormatTextBox.Text;
            EpisodeSeperatorSampleLabel.Text = EpisodeSeperatorSampleLabel.Text.Replace("{season}", "1");

            if (UseDoubleDigitsEpisodeNumberCheckBox.Checked)
            {
                EpisodeSeperatorSampleLabel.Text = EpisodeSeperatorSampleLabel.Text.Replace("{episode}", "01");
            }
            else
            {
                EpisodeSeperatorSampleLabel.Text = EpisodeSeperatorSampleLabel.Text.Replace("{episode}", "1");
            }

            EpisodeSeperatorSampleLabel.Text += " Pilot";
            EpisodeSeperatorSampleLabel.Text = EpisodeSeperatorSampleLabel.Text.Trim();
        }

        private void OnSelectDataPathButtonClick(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                SelectedPath = Program.RootPath,
                Description = Resources.SelectDataPath,
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var target = fbd.SelectedPath;

                    if (target != Program.RootPath)
                    {
                        if (MessageBox.Show(MessageBoxTexts.FilesWillNowBeMoved, MessageBoxTexts.ContinueHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            try
                            {
                                if (!Directory.Exists(target + @"\Data"))
                                {
                                    Directory.CreateDirectory(target + @"\Data");
                                }

                                if (!Directory.Exists(target + @"\Images"))
                                {
                                    Directory.CreateDirectory(target + @"\Images");
                                }

                                if (!Directory.Exists(target + @"\Images\CastCrewEdit2"))
                                {
                                    Directory.CreateDirectory(target + @"\Images\CastCrewEdit2");
                                }

                                if (!Directory.Exists(target + @"\Images\CCViewer"))
                                {
                                    Directory.CreateDirectory(target + @"\Images\CCViewer");
                                }

                                if (!Directory.Exists(target + @"\Images\DVD Profiler"))
                                {
                                    Directory.CreateDirectory(target + @"\Images\DVD Profiler");
                                }

                                var files = Directory.GetFiles(Program.RootPath + @"\Data", "*.*", SearchOption.AllDirectories);

                                foreach (var file in files)
                                {
                                    var newFile = file.Replace(Program.RootPath + @"\Data", target + @"\Data");

                                    File.Copy(file, newFile, true);
                                    File.Delete(file);
                                }

                                files = Directory.GetFiles(Program.RootPath + @"\Images", "*.*", SearchOption.AllDirectories);

                                foreach (var file in files)
                                {
                                    var newFile = file.Replace(Program.RootPath + @"\Images", target + @"\Images");

                                    File.Copy(file, newFile, true);
                                    //File.Delete(file);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                Program.WriteError(ex);

                                return;
                            }

                            Directory.Delete(Program.RootPath + @"\Data", true);
                            Directory.Delete(Program.RootPath + @"\Images", true);

                            RegistryAccess.DataRootPath = target;

                            Program.GetRootPath();

                            DataPathTextBox.Text = target;
                        }
                    }
                }
            }
        }

        private void OnParseCastCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var enabled = ParseCastCheckBox.Checked;

            ParseVoiceOfCheckBox.Enabled = enabled;

            ParseRoleSlashCheckBox.Enabled = enabled;

            RetainCreditedAsOnCastCheckBox.Enabled = enabled;

            IgnoreUncreditedCheckBox.Enabled = enabled;

            IgnoreScenesDeletedCheckBox.Enabled = enabled;

            IgnoreArchiveFootageCheckBox.Enabled = enabled;

            IgnoreCreditOnlyCheckBox.Enabled = enabled;

            IgnoreLanguageVersionCheckBox.Enabled = enabled;

            IgnoreUnconfirmedCheckBox.Enabled = enabled;
        }

        private void OnParseCrewCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var enabled = ParseCrewCheckBox.Checked;

            RetainOriginalCreditCheckbox.Enabled = enabled;

            RetainCreditedAsOnCrewCheckBox.Enabled = enabled;

            CapitalizeCustomRoleCheckBox.Enabled = enabled;

            IncludePrefixOnOtherCreditsCheckBox.Enabled = enabled;

            IncludingCustomCredits.Enabled = enabled;

            IncludingCreditTypeOtherCheckBox.Enabled = enabled;

            IncludingCreditTypeDirectionCheckBox.Enabled = enabled;

            IncludingCreditTypeWritingCheckBox.Enabled = enabled;

            IncludingCreditTypeProductionCheckBox.Enabled = enabled;

            IncludingCreditTypeCinematographyCheckBox.Enabled = enabled;

            IncludingCreditTypeFilmEditingCheckBox.Enabled = enabled;

            IncludingCreditTypeMusicCheckBox.Enabled = enabled;

            IncludingCreditTypeSoundCheckBox.Enabled = enabled;

            IncludingCreditTypeArtCheckBox.Enabled = enabled;

            IncludingCreditTypeSoundtrackCheckBox.Enabled = enabled;

            GroupSoundtrackCreditsCheckBox.Enabled = enabled;
        }
    }
}