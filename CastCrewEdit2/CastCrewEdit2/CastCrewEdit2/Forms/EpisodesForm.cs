namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Extended;
    using Helper;
    using Resources;

    internal partial class EpisodesForm : CastCrewEdit2BaseForm
    {
        private readonly List<EpisodeInfo> _episodes;

        static EpisodesForm()
        {
            //NotSeriesCrew
            //    = new Regex("(?'BeginOfLine'.+)<table border=\"0\" cellpadding=\"1\" cellspacing=\"1\"><tr><td align=\"left\"><b>Series Crew</b>"
            //    , RegexOptions.Compiled);
        }

        public EpisodesForm(List<EpisodeInfo> episodes)
        {
            _episodes = episodes;

            InitializeComponent();

            _progressBar = ProgressBar;

            Icon = Properties.Resource.djdsoft;
        }

        private void OnEpisodesFormLoad(object sender, EventArgs e)
        {
            LayoutForm();

            CreateDataGridViewColumns();

            foreach (var episode in _episodes)
            {
                var row = DataGridView.Rows[DataGridView.Rows.Add()];

                row.DefaultCellStyle.BackColor = Color.White;
                row.Cells["Season Number"].Value = episode.SeasonNumber;
                row.Cells["Episode Number"].Value = episode.EpisodeNumber;
                row.Cells["Episode Name"].Value = episode.EpisodeName;
                row.Cells["Link"].Value = episode.Link;
                row.Tag = episode;
            }

            RegisterEvents();

            var resources = new ComponentResourceManager(typeof(EpisodesForm));

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

            DataGridView.CellContentClick += OnDataGridViewCellContentClick;
        }

        private void CreateDataGridViewColumns()
        {
            var seasonNumberDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = "Season Number",
                HeaderText = DataGridViewTexts.SeasonNumber,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
            };

            var episodeNumberDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = "Episode Number",
                HeaderText = DataGridViewTexts.EpisodeNumber,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
            };

            var episodeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = "Episode Name",
                HeaderText = DataGridViewTexts.EpisodeName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
            };


            var episodeLinkColumn = new DataGridViewLinkColumn
            {
                Name = "Link",
                HeaderText = DataGridViewTexts.Link,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
            };

            DataGridView.Columns.AddRange(seasonNumberDataGridViewTextBoxColumn, episodeNumberDataGridViewTextBoxColumn, episodeDataGridViewTextBoxColumn, episodeLinkColumn);
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

        private void OnDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                Process.Start(IMDbParser.TitleUrl + DataGridView.Rows[e.RowIndex].Cells[ColumnNames.Link].Value.ToString());
            }
        }

        private void OnEpisodeFormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Settings.EpisodesForm.Left = Left;
            Program.Settings.EpisodesForm.Top = Top;
            Program.Settings.EpisodesForm.Width = Width;
            Program.Settings.EpisodesForm.Height = Height;
            Program.Settings.EpisodesForm.WindowState = WindowState;
            Program.Settings.EpisodesForm.RestoreBounds = RestoreBounds;
        }

        private void OnScanEpisodesButtonClick(object sender, EventArgs e)
        {
            if (DataGridView.SelectedRows == null || DataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, MessageBoxTexts.NoEpisodeSelected, MessageBoxTexts.NoEpisodeSelectedHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            else
            {
                try
                {
                    ScanRows(DataGridView.SelectedRows);
                }
                catch (AggregateException ex)
                {
                    MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Program.WriteError(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Program.WriteError(ex);
                }
            }
        }

        private void ScanRows(IEnumerable rows)
        {
            StartLongAction();

            SuspendLayout();

            var episodes = new List<EpisodeInfo>();

            try
            {
                foreach (DataGridViewRow row in rows)
                {
                    var episode = (EpisodeInfo)row.Tag;

                    episode.CastList = new List<CastInfo>();
                    episode.CrewList = new List<CrewInfo>();
                    episode.CastMatches = new List<Match>();
                    episode.CrewMatches = new List<KeyValuePair<Match, List<Match>>>();
                    episode.SoundtrackMatches = new Dictionary<string, List<SoundtrackMatch>>();

                    episodes.Add(episode);
                }

                episodes.Sort(CompareEpisodes);

                try
                {
                    StartProgress(episodes.Count, Color.LightBlue);

                    foreach (var episode in episodes)
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

            using (var castForm = new EpisodeForm(episodes))
            {
                castForm.ShowDialog(this);
            }
        }

        private void ParseIMDb(EpisodeInfo episode)
        {
            var defaultValues = GetDefaultValues();

            ParseCastAndCrew(episode.Link, true, true, true, true, ref episode.CastMatches, ref episode.CastList, ref episode.CrewMatches, ref episode.CrewList, ref episode.SoundtrackMatches);

            _suppressProgress = true;

            try
            {
                ProcessLines(episode.CastList, episode.CastMatches, episode.CrewList, episode.CrewMatches, episode.SoundtrackMatches, defaultValues);
            }
            finally
            {
                _suppressProgress = false;
            }
        }

        private DefaultValues GetDefaultValues() => DefaultValues.GetFromProgramSettings();

        private static int CompareEpisodes(EpisodeInfo left, EpisodeInfo right)
        {
            var compare = left.SeasonNumber.PadLeft(3, '0').CompareTo(right.SeasonNumber.PadLeft(3, '0'));

            if (compare == 0)
            {
                compare = left.EpisodeNumber.PadLeft(5, '0').CompareTo(right.EpisodeNumber.PadLeft(5, '0'));
            }

            return compare;
        }

        private void OnCloseButtonClick(object sender, EventArgs e) => Close();

        private void OnSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(true, true))
            {
                settingsForm.SetValues(Program.Settings.SettingsForm.Left, Program.Settings.SettingsForm.Top, Program.DefaultValues);

                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    _settingsHaveChanged = true;
                }

                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnScanAllEpisodesButtonClick(object sender, EventArgs e)
        {
            try
            {
                ScanRows(DataGridView.Rows);
            }
            catch (AggregateException ex)
            {
                MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
        }
    }
}