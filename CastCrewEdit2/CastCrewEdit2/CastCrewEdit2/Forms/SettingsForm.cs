using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using System;
using System.IO;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2
{
    internal partial class SettingsForm : Form
    {
        private DefaultValues DefaultValues;
        private Int32 FormLeft;
        private Int32 FormTop;
        private Boolean ShowCastOptions;
        private Boolean ShowCrewOptions;

        public SettingsForm(Boolean castOptions, Boolean crewOptions)
        {
            InitializeComponent();
            ShowCastOptions = castOptions;
            ShowCrewOptions = crewOptions;
        }

        public void SetValues(Int32 left, Int32 top, DefaultValues defaultValues)
        {
            FormLeft = left;
            FormTop = top;
            DefaultValues = defaultValues;
        }

        public void GetValues(out Int32 left, out Int32 top)
        {
            left = FormLeft;
            top = FormTop;
        }

        private void LoadDataSettings()
        {
            ParseFirstNameInitialsIntoFirstAndMiddleNameCheckBox.Checked
                = DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName;
            TakeBirthYearFromLocalPersonCacheCheckBox.Checked = DefaultValues.TakeBirthYearFromLocalCache;
            RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Checked
                = DefaultValues.RetrieveBirthYearWhenLocalCacheEmpty;
            GetBirthYearsDirectlyAfterNameParsingCheckBox.Checked
                = DefaultValues.GetBirthYearsDirectlyAfterNameParsing;
            CheckLinksCheckBox.Checked = DefaultValues.CheckPersonLinkForRedirect;
            ParseCastCheckBox.Checked = DefaultValues.ParseCast;
            ParseRoleSlashCheckBox.Checked = DefaultValues.ParseRoleSlash;
            ParseVoiceOfCheckBox.Checked = DefaultValues.ParseVoiceOf;
            IgnoreUncreditedCheckBox.Checked = DefaultValues.IgnoreUncredited;
            IgnoreCreditOnlyCheckBox.Checked = DefaultValues.IgnoreCreditOnly;
            IgnoreScenesDeletedCheckBox.Checked = DefaultValues.IgnoreScenesDeleted;
            IgnoreArchiveFootageCheckBox.Checked = DefaultValues.IgnoreArchiveFootage;
            IgnoreLanguageVersionCheckBox.Checked = DefaultValues.IgnoreLanguageVersion;
            IgnoreUnconfirmedCheckBox.Checked = DefaultValues.IgnoreUnconfirmed;
            RetainCreditedAsOnCastCheckBox.Checked = DefaultValues.RetainCastCreditedAs;
            ParseCrewCheckBox.Checked = DefaultValues.ParseCrew;
            IncludingCustomCredits.Checked = DefaultValues.IncludeCustomCredits;
            RetainOriginalCreditCheckbox.Checked = DefaultValues.RetainOriginalCredit;
            IncludePrefixOnOtherCreditsCheckBox.Checked = DefaultValues.IncludePrefixOnOtherCredits;
            CapitalizeCustomRoleCheckBox.Checked = DefaultValues.CapitalizeCustomRole;
            RetainCreditedAsOnCrewCheckBox.Checked = DefaultValues.RetainCrewCreditedAs;
            IncludingCreditTypeDirectionCheckBox.Checked = DefaultValues.CreditTypeDirection;
            IncludingCreditTypeWritingCheckBox.Checked = DefaultValues.CreditTypeWriting;
            IncludingCreditTypeProductionCheckBox.Checked = DefaultValues.CreditTypeProduction;
            IncludingCreditTypeCinematographyCheckBox.Checked = DefaultValues.CreditTypeCinematography;
            IncludingCreditTypeFilmEditingCheckBox.Checked = DefaultValues.CreditTypeFilmEditing;
            IncludingCreditTypeMusicCheckBox.Checked = DefaultValues.CreditTypeMusic;
            IncludingCreditTypeSoundCheckBox.Checked = DefaultValues.CreditTypeSound;
            IncludingCreditTypeArtCheckBox.Checked = DefaultValues.CreditTypeArt;
            IncludingCreditTypeOtherCheckBox.Checked = DefaultValues.CreditTypeOther;
            IncludingCreditTypeSoundtrackCheckBox.Checked = DefaultValues.CreditTypeSoundtrack;
            DisableParsingCompleteMessageBoxCheckBox.Checked = DefaultValues.DisableParsingCompleteMessageBox;
            DisableParsingCompleteMessageBoxForGetBirthYearsCheckBox.Checked
                = DefaultValues.DisableParsingCompleteMessageBoxForGetBirthYears;
            DisableParsingCompleteMessageBoxForGetHeadshotsCheckBox.Checked
                = DefaultValues.DisableParsingCompleteMessageBoxForGetHeadshots;
            DisableCopyingSuccessfulMessageBoxCheckBox.Checked
                = DefaultValues.DisableCopyingSuccessfulMessageBox;
            DisableDuplicatesMessageBoxCheckBox.Checked
                = DefaultValues.DisableDuplicatesMessageBox;
            EpisodeFormatTextBox.Text = DefaultValues.EpisodeDividerFormat;
            UseDoubleDigitsEpisodeNumberCheckBox.Checked = DefaultValues.UseDoubleDigitsEpisodeNumber;
            DataPathTextBox.Text = Program.RootPath;
            GetCastHeadshotCheckBox.Checked = DefaultValues.GetCastHeadShots;
            GetCrewHeadshotCheckBox.Checked = DefaultValues.GetCrewHeadShots;
            AutoCopyHeadShotsCheckBox.Checked = DefaultValues.AutoCopyHeadShots;
            GetHeadshotsDirectlyAfterNameParsingCheckBox.Checked = DefaultValues.GetHeadShotsDirectlyAfterNameParsing;
            DownloadTriviaCheckBox.Checked = DefaultValues.DownloadTrivia;
            DownloadGoofsCheckBox.Checked = DefaultValues.DownloadGoofs;
            UseFakeBirthYearsCheckBox.Checked = DefaultValues.UseFakeBirthYears;
            SaveLogFileCheckBox.Checked = DefaultValues.SaveLogFile;
            StoreHeadshotsPerSessionCheckBox.Checked = DefaultValues.StoreHeadshotsPerSession;
            OnEpisodeFormatChanged(null, null);
            OnParseCastCheckBoxCheckedChanged(null, null);
            OnParseCrewCheckBoxCheckedChanged(null, null);
            OnTakeBirthYearFromLocalPersonCacheCheckBoxCheckedChanged(null, null);
        }

        private void OnCancelButtonClick(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnAcceptButtonClick(Object sender, EventArgs e)
        {
            SaveDataSettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveDataSettings()
        {
            DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName
                = ParseFirstNameInitialsIntoFirstAndMiddleNameCheckBox.Checked;
            DefaultValues.TakeBirthYearFromLocalCache = TakeBirthYearFromLocalPersonCacheCheckBox.Checked;
            DefaultValues.RetrieveBirthYearWhenLocalCacheEmpty
                = RetrieveBirthYearWhenLocalCacheEmptyCheckBox.Checked;
            DefaultValues.GetBirthYearsDirectlyAfterNameParsing
                = GetBirthYearsDirectlyAfterNameParsingCheckBox.Checked;
            DefaultValues.CheckPersonLinkForRedirect = CheckLinksCheckBox.Checked;
            DefaultValues.ParseCast = ParseCastCheckBox.Checked;
            DefaultValues.ParseRoleSlash = ParseRoleSlashCheckBox.Checked;
            DefaultValues.ParseVoiceOf = ParseVoiceOfCheckBox.Checked;
            DefaultValues.IgnoreUncredited = IgnoreUncreditedCheckBox.Checked;
            DefaultValues.IgnoreCreditOnly = IgnoreCreditOnlyCheckBox.Checked;
            DefaultValues.IgnoreScenesDeleted = IgnoreScenesDeletedCheckBox.Checked;
            DefaultValues.IgnoreArchiveFootage = IgnoreArchiveFootageCheckBox.Checked;
            DefaultValues.IgnoreLanguageVersion = IgnoreLanguageVersionCheckBox.Checked;
            DefaultValues.IgnoreUnconfirmed = IgnoreUnconfirmedCheckBox.Checked;
            DefaultValues.RetainCastCreditedAs = RetainCreditedAsOnCastCheckBox.Checked;
            DefaultValues.ParseCrew = ParseCrewCheckBox.Checked;
            DefaultValues.IncludeCustomCredits = IncludingCustomCredits.Checked;
            DefaultValues.RetainOriginalCredit = RetainOriginalCreditCheckbox.Checked;
            DefaultValues.IncludePrefixOnOtherCredits = IncludePrefixOnOtherCreditsCheckBox.Checked;
            DefaultValues.CapitalizeCustomRole = CapitalizeCustomRoleCheckBox.Checked;
            DefaultValues.RetainCrewCreditedAs = RetainCreditedAsOnCrewCheckBox.Checked;
            DefaultValues.CreditTypeDirection = IncludingCreditTypeDirectionCheckBox.Checked;
            DefaultValues.CreditTypeWriting = IncludingCreditTypeWritingCheckBox.Checked;
            DefaultValues.CreditTypeProduction = IncludingCreditTypeProductionCheckBox.Checked;
            DefaultValues.CreditTypeCinematography = IncludingCreditTypeCinematographyCheckBox.Checked;
            DefaultValues.CreditTypeFilmEditing = IncludingCreditTypeFilmEditingCheckBox.Checked;
            DefaultValues.CreditTypeMusic = IncludingCreditTypeMusicCheckBox.Checked;
            DefaultValues.CreditTypeSound = IncludingCreditTypeSoundCheckBox.Checked;
            DefaultValues.CreditTypeArt = IncludingCreditTypeArtCheckBox.Checked;
            DefaultValues.CreditTypeOther = IncludingCreditTypeOtherCheckBox.Checked;
            DefaultValues.CreditTypeSoundtrack = IncludingCreditTypeSoundtrackCheckBox.Checked;
            DefaultValues.DisableParsingCompleteMessageBox = DisableParsingCompleteMessageBoxCheckBox.Checked;
            DefaultValues.DisableParsingCompleteMessageBoxForGetBirthYears
                = DisableParsingCompleteMessageBoxForGetBirthYearsCheckBox.Checked;
            DefaultValues.DisableParsingCompleteMessageBoxForGetHeadshots
                = DisableParsingCompleteMessageBoxForGetHeadshotsCheckBox.Checked;
            DefaultValues.DisableCopyingSuccessfulMessageBox
                = DisableCopyingSuccessfulMessageBoxCheckBox.Checked;
            DefaultValues.DisableDuplicatesMessageBox
                = DisableDuplicatesMessageBoxCheckBox.Checked;
            DefaultValues.EpisodeDividerFormat = EpisodeFormatTextBox.Text;
            DefaultValues.UseDoubleDigitsEpisodeNumber = UseDoubleDigitsEpisodeNumberCheckBox.Checked;
            DefaultValues.GetCastHeadShots = GetCastHeadshotCheckBox.Checked;
            DefaultValues.GetCrewHeadShots = GetCrewHeadshotCheckBox.Checked;
            DefaultValues.AutoCopyHeadShots = AutoCopyHeadShotsCheckBox.Checked;
            DefaultValues.GetHeadShotsDirectlyAfterNameParsing = GetHeadshotsDirectlyAfterNameParsingCheckBox.Checked;
            DefaultValues.DownloadTrivia = DownloadTriviaCheckBox.Checked;
            DefaultValues.DownloadGoofs = DownloadGoofsCheckBox.Checked;
            DefaultValues.UseFakeBirthYears = UseFakeBirthYearsCheckBox.Checked;
            DefaultValues.SaveLogFile = SaveLogFileCheckBox.Checked;
            DefaultValues.StoreHeadshotsPerSession = StoreHeadshotsPerSessionCheckBox.Checked;
        }

        private void OnSettingsFormLoad(Object sender, EventArgs e)
        {
            Left = FormLeft;
            Top = FormTop;
            LoadDataSettings();
            if (ShowCastOptions == false)
            {
                SettingsTabControl.Controls.Remove(CastParsingTab);
            }
            if (ShowCrewOptions == false)
            {
                SettingsTabControl.Controls.Remove(CrewParsingTab);
            }
            if ((ShowCastOptions == false) || (ShowCrewOptions == false))
            {
                SettingsTabControl.Controls.Remove(ParsingTab);
            }
        }

        private void OnSettingsFormClosing(Object sender, FormClosingEventArgs e)
        {
            FormLeft = Left;
            FormTop = Top;
        }

        private void OnTakeBirthYearFromLocalPersonCacheCheckBoxCheckedChanged(Object sender, EventArgs e)
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

        private void OnEpisodeFormatChanged(Object sender, EventArgs e)
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

        private void OnSelectDataPathButtonClick(Object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = Program.RootPath;
                fbd.Description = Resources.Resources.SelectDataPath;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    String target;

                    target = fbd.SelectedPath;
                    if (target != Program.RootPath)
                    {
                        if (MessageBox.Show(MessageBoxTexts.FilesWillNowBeMoved, MessageBoxTexts.ContinueHeader
                            , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            try
                            {
                                String[] files;

                                if (Directory.Exists(target + @"\Data") == false)
                                {
                                    Directory.CreateDirectory(target + @"\Data");
                                }
                                if (Directory.Exists(target + @"\Images") == false)
                                {
                                    Directory.CreateDirectory(target + @"\Images");
                                }
                                if (Directory.Exists(target + @"\Images\CastCrewEdit2") == false)
                                {
                                    Directory.CreateDirectory(target + @"\Images\CastCrewEdit2");
                                }
                                if (Directory.Exists(target + @"\Images\CCViewer") == false)
                                {
                                    Directory.CreateDirectory(target + @"\Images\CCViewer");
                                }
                                if (Directory.Exists(target + @"\Images\DVD Profiler") == false)
                                {
                                    Directory.CreateDirectory(target + @"\Images\DVD Profiler");
                                }
                                files = Directory.GetFiles(Program.RootPath + @"\Data", "*.*"
                                    , SearchOption.AllDirectories);
                                foreach (String file in files)
                                {
                                    String newFile;

                                    newFile = file.Replace(Program.RootPath + @"\Data", target + @"\Data");
                                    File.Copy(file, newFile, true);
                                    File.Delete(file);
                                }
                                files = Directory.GetFiles(Program.RootPath + @"\Images", "*.*"
                                    , SearchOption.AllDirectories);
                                foreach (String file in files)
                                {
                                    String newFile;

                                    newFile = file.Replace(Program.RootPath + @"\Images", target + @"\Images");
                                    File.Copy(file, newFile, true);
                                    //File.Delete(file);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK
                                    , MessageBoxIcon.Exclamation);
                                Program.WriteError(ex);
                                return;
                            }
                            Directory.Delete(Program.RootPath + @"\Data", true);
                            Directory.Delete(Program.RootPath + @"\Images", true);
                            RegistryAccess.DataRootPath = target;
                            Program.InitPaths();
                            DataPathTextBox.Text = target;
                        }
                    }
                }
            }
        }

        private void OnParseCastCheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            Boolean enabled = ParseCastCheckBox.Checked;

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

        private void OnParseCrewCheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            Boolean enabled = ParseCrewCheckBox.Checked;

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
        }
    }
}