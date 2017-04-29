using System;
using System.Collections;
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
    internal partial class EpisodesForm : CastCrewEdit2BaseForm
    {
        //private static readonly Regex NotSeriesCrew;
        private List<EpisodeInfo> Episodes;

        static EpisodesForm()
        {
            //NotSeriesCrew
            //    = new Regex("(?'BeginOfLine'.+)<table border=\"0\" cellpadding=\"1\" cellspacing=\"1\"><tr><td align=\"left\"><b>Series Crew</b>"
            //    , RegexOptions.Compiled);
        }

        public EpisodesForm(List<EpisodeInfo> episodes)
        {
            Episodes = episodes;

            InitializeComponent();

            TheProgressBar = ProgressBar;
        }

        private void OnEpisodeFormLoad(Object sender, EventArgs e)
        {
            ComponentResourceManager resources;

            LayoutForm();
            CreateDataGridViewColumns();
            foreach (EpisodeInfo episode in Episodes)
            {
                DataGridViewRow row;

                row = DataGridView.Rows[DataGridView.Rows.Add()];
                row.DefaultCellStyle.BackColor = Color.White;
                row.Cells["Season Number"].Value = episode.SeasonNumber;
                row.Cells["Episode Number"].Value = episode.EpisodeNumber;
                row.Cells["Episode Name"].Value = episode.EpisodeName;
                row.Cells["Link"].Value = episode.Link;
                row.Tag = episode;
            }
            RegisterEvents();
            resources = new ComponentResourceManager(typeof(EpisodesForm));
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
            DataGridView.CellContentClick += OnDataGridViewCellContentClick;
        }

        private void CreateDataGridViewColumns()
        {
            DataGridViewTextBoxColumn seasonNumberDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn episodeNumberDataGridViewTextBoxColumn;
            DataGridViewTextBoxColumn episodeDataGridViewTextBoxColumn;
            DataGridViewLinkColumn episodeLinkColumn;

            seasonNumberDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            seasonNumberDataGridViewTextBoxColumn.Name = "Season Number";
            seasonNumberDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.SeasonNumber;
            seasonNumberDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            seasonNumberDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            DataGridView.Columns.Add(seasonNumberDataGridViewTextBoxColumn);

            episodeNumberDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            episodeNumberDataGridViewTextBoxColumn.Name = "Episode Number";
            episodeNumberDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.EpisodeNumber;
            episodeNumberDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            episodeNumberDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            DataGridView.Columns.Add(episodeNumberDataGridViewTextBoxColumn);

            episodeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            episodeDataGridViewTextBoxColumn.Name = "Episode Name";
            episodeDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.EpisodeName;
            episodeDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            episodeDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
            DataGridView.Columns.Add(episodeDataGridViewTextBoxColumn);

            episodeLinkColumn = new DataGridViewLinkColumn();
            episodeLinkColumn.Name = "Link";
            episodeLinkColumn.HeaderText = DataGridViewTexts.Link;
            episodeLinkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            episodeLinkColumn.Resizable = DataGridViewTriState.True;

            DataGridView.Columns.Add(episodeLinkColumn);
        }

        private void LayoutForm()
        {
            if (Program.Settings.EpisodesForm.WindowState == FormWindowState.Normal)
            {
                Left = Program.Settings.EpisodesForm.Left;
                Top = Program.Settings.EpisodesForm.Top;
                if (Program.Settings.EpisodesForm.Width > MinimumSize.Width)
                {
                    Width = Program.Settings.EpisodesForm.Width;
                }
                else
                {
                    Width = MinimumSize.Width;
                }
                if (Program.Settings.EpisodesForm.Height > MinimumSize.Height)
                {
                    Height = Program.Settings.EpisodesForm.Height;
                }
                else
                {
                    Height = MinimumSize.Height;
                }
            }
            else
            {
                Left = Program.Settings.EpisodesForm.RestoreBounds.X;
                Top = Program.Settings.EpisodesForm.RestoreBounds.Y;
                if (Program.Settings.EpisodesForm.RestoreBounds.Width > MinimumSize.Width)
                {
                    Width = Program.Settings.EpisodesForm.RestoreBounds.Width;
                }
                else
                {
                    Width = MinimumSize.Width;
                }
                if (Program.Settings.EpisodesForm.RestoreBounds.Height > MinimumSize.Height)
                {
                    Height = Program.Settings.EpisodesForm.RestoreBounds.Height;
                }
                else
                {
                    Height = MinimumSize.Height;
                }
            }
            if (Program.Settings.EpisodesForm.WindowState != FormWindowState.Minimized)
            {
                WindowState = Program.Settings.EpisodesForm.WindowState;
            }
        }

        private void OnDataGridViewCellContentClick(Object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                Process.Start(IMDbParser.TitleUrl
                    + DataGridView.Rows[e.RowIndex].Cells[ColumnNames.Link].Value.ToString());
            }
        }

        private void OnEpisodeFormClosing(Object sender, FormClosingEventArgs e)
        {
            Program.Settings.EpisodesForm.Left = Left;
            Program.Settings.EpisodesForm.Top = Top;
            Program.Settings.EpisodesForm.Width = Width;
            Program.Settings.EpisodesForm.Height = Height;
            Program.Settings.EpisodesForm.WindowState = WindowState;
            Program.Settings.EpisodesForm.RestoreBounds = RestoreBounds;
        }

        private void OnScanEpisodesButtonClick(Object sender, EventArgs e)
        {
            if ((DataGridView.SelectedRows == null) || (DataGridView.SelectedRows.Count == 0))
            {
                MessageBox.Show(this, MessageBoxTexts.NoEpisodeSelected, MessageBoxTexts.NoEpisodeSelectedHeader, MessageBoxButtons.OK
                    , MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {
                    ScanRows(DataGridView.SelectedRows, ScanEpisodesButton.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Program.WriteError(ex);
                }
            }
        }

        private void ScanRows(IEnumerable rows
            , String buttonText)
        {
            StartLongAction();

            SuspendLayout();

            List<EpisodeInfo> episodes = new List<EpisodeInfo>();

            try
            {
                foreach (DataGridViewRow row in rows)
                {
                    EpisodeInfo episode = (EpisodeInfo)(row.Tag);
                    episode.CastList = new List<CastInfo>();
                    episode.CrewList = new List<CrewInfo>();
                    episode.CastMatches = new List<Match>();
                    episode.CrewMatches = new List<KeyValuePair<Match, List<Match>>>();
                    episode.SoundtrackMatches = new Dictionary<String, List<Match>>();
                    episodes.Add(episode);
                }

                episodes.Sort(CompareEpisodes);

                try
                {
                    StartProgress(episodes.Count, Color.LightBlue);

                    foreach (EpisodeInfo episode in episodes)
                    {
                        ParseIMDb(episode);

                        SetProgress();
                    }
                }
                finally
                {
                    EndProgress();
                }
            }
            finally
            {
                Program.FlushPersonCache();

                EndLongAction();
            }

            using (EpisodeForm castForm = new EpisodeForm(episodes))
            {
                castForm.ShowDialog(this);
            }
        }

        private void ParseIMDb(EpisodeInfo episode)
        {
            DefaultValues defaultValues = CopyDefaultValues();

            ParseCastAndCrew(defaultValues, episode.Link, true, true, true, true, ref episode.CastMatches, ref episode.CastList, ref episode.CrewMatches
                , ref episode.CrewList, ref episode.SoundtrackMatches);

            SuppressProgress = true;

            try
            {
                ProcessLines(episode.CastList, episode.CastMatches, episode.CrewList, episode.CrewMatches, episode.SoundtrackMatches, defaultValues);
            }
            finally
            {
                SuppressProgress = false;
            }
        }

        private DefaultValues CopyDefaultValues()
        {
            DefaultValues defaultValues = new DefaultValues();

            defaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName = Program.Settings.DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName;
            defaultValues.ParseRoleSlash = Program.Settings.DefaultValues.ParseRoleSlash;
            defaultValues.ParseVoiceOf = Program.Settings.DefaultValues.ParseVoiceOf;
            defaultValues.IgnoreUncredited = Program.Settings.DefaultValues.IgnoreUncredited;
            defaultValues.IgnoreCreditOnly = Program.Settings.DefaultValues.IgnoreCreditOnly;
            defaultValues.IgnoreScenesDeleted = Program.Settings.DefaultValues.IgnoreScenesDeleted;
            defaultValues.IgnoreArchiveFootage = Program.Settings.DefaultValues.IgnoreArchiveFootage;
            defaultValues.IgnoreLanguageVersion = Program.Settings.DefaultValues.IgnoreLanguageVersion;
            defaultValues.IncludeCustomCredits = Program.Settings.DefaultValues.IncludeCustomCredits;
            defaultValues.RetainCastCreditedAs = Program.Settings.DefaultValues.RetainCastCreditedAs;
            defaultValues.RetainCrewCreditedAs = Program.Settings.DefaultValues.RetainCrewCreditedAs;
            defaultValues.RetainOriginalCredit = Program.Settings.DefaultValues.RetainOriginalCredit;
            defaultValues.IncludePrefixOnOtherCredits = Program.Settings.DefaultValues.IncludePrefixOnOtherCredits;
            defaultValues.CapitalizeCustomRole = Program.Settings.DefaultValues.CapitalizeCustomRole;
            defaultValues.RetainCrewCreditedAs = Program.Settings.DefaultValues.RetainCrewCreditedAs;
            defaultValues.CreditTypeDirection = Program.Settings.DefaultValues.CreditTypeDirection;
            defaultValues.CreditTypeWriting = Program.Settings.DefaultValues.CreditTypeWriting;
            defaultValues.CreditTypeProduction = Program.Settings.DefaultValues.CreditTypeProduction;
            defaultValues.CreditTypeCinematography = Program.Settings.DefaultValues.CreditTypeCinematography;
            defaultValues.CreditTypeFilmEditing = Program.Settings.DefaultValues.CreditTypeFilmEditing;
            defaultValues.CreditTypeMusic = Program.Settings.DefaultValues.CreditTypeMusic;
            defaultValues.CreditTypeSound = Program.Settings.DefaultValues.CreditTypeSound;
            defaultValues.CreditTypeArt = Program.Settings.DefaultValues.CreditTypeArt;
            defaultValues.CreditTypeOther = Program.Settings.DefaultValues.CreditTypeOther;
            defaultValues.CreditTypeSoundtrack = Program.Settings.DefaultValues.CreditTypeSoundtrack;
            defaultValues.CheckPersonLinkForRedirect = Program.Settings.DefaultValues.CheckPersonLinkForRedirect;

            return (defaultValues);
        }

        private static Int32 CompareEpisodes(EpisodeInfo left, EpisodeInfo right)
        {
            Int32 compare;

            compare = left.SeasonNumber.PadLeft(2, '0').CompareTo(right.SeasonNumber.PadLeft(2, '0'));
            if (compare == 0)
            {
                compare = left.EpisodeNumber.PadLeft(2, '0').CompareTo(right.EpisodeNumber.PadLeft(2, '0'));
            }
            return (compare);
        }

        private void OnCloseButtonClick(Object sender, EventArgs e)
        {
            Close();
        }

        private void OnSettingsToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm(true, true))
            {
                settingsForm.SetValues(Program.Settings.SettingsForm.Left, Program.Settings.SettingsForm.Top
                    , Program.Settings.DefaultValues);
                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    SettingsHaveChanged = true;
                }
                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnScanAllEpisodesButtonClick(Object sender, EventArgs e)
        {
            try
            {
                ScanRows(DataGridView.Rows, ScanAllEpisodesButton.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Program.WriteError(ex);
            }
        }
    }
}