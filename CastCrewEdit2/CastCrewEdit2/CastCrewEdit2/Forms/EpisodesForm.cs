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

            this.InitializeComponent();

            _progressBar = ProgressBar;

            this.Icon = Properties.Resource.djdsoft;
        }

        private void OnEpisodeFormLoad(object sender, EventArgs e)
        {
            this.LayoutForm();

            this.CreateDataGridViewColumns();

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

            this.RegisterEvents();

            var resources = new ComponentResourceManager(typeof(EpisodesForm));

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

            DataGridView.CellContentClick += this.OnDataGridViewCellContentClick;
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
                this.Left = Program.Settings.EpisodesForm.Left;
                this.Top = Program.Settings.EpisodesForm.Top;

                if (Program.Settings.EpisodesForm.Width > this.MinimumSize.Width)
                {
                    this.Width = Program.Settings.EpisodesForm.Width;
                }
                else
                {
                    this.Width = this.MinimumSize.Width;
                }

                if (Program.Settings.EpisodesForm.Height > this.MinimumSize.Height)
                {
                    this.Height = Program.Settings.EpisodesForm.Height;
                }
                else
                {
                    this.Height = this.MinimumSize.Height;
                }
            }
            else
            {
                this.Left = Program.Settings.EpisodesForm.RestoreBounds.X;
                this.Top = Program.Settings.EpisodesForm.RestoreBounds.Y;

                if (Program.Settings.EpisodesForm.RestoreBounds.Width > this.MinimumSize.Width)
                {
                    this.Width = Program.Settings.EpisodesForm.RestoreBounds.Width;
                }
                else
                {
                    this.Width = this.MinimumSize.Width;
                }

                if (Program.Settings.EpisodesForm.RestoreBounds.Height > this.MinimumSize.Height)
                {
                    this.Height = Program.Settings.EpisodesForm.RestoreBounds.Height;
                }
                else
                {
                    this.Height = this.MinimumSize.Height;
                }
            }

            if (Program.Settings.EpisodesForm.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Program.Settings.EpisodesForm.WindowState;
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
            Program.Settings.EpisodesForm.Left = this.Left;
            Program.Settings.EpisodesForm.Top = this.Top;
            Program.Settings.EpisodesForm.Width = this.Width;
            Program.Settings.EpisodesForm.Height = this.Height;
            Program.Settings.EpisodesForm.WindowState = this.WindowState;
            Program.Settings.EpisodesForm.RestoreBounds = this.RestoreBounds;
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
                    this.ScanRows(DataGridView.SelectedRows);
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
            this.StartLongAction();

            this.SuspendLayout();

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
                    this.StartProgress(episodes.Count, Color.LightBlue);

                    foreach (var episode in episodes)
                    {
                        this.ParseIMDb(episode);

                        this.SetProgress();
                    }
                }
                finally
                {
                    this.EndProgress();
                }
            }
            finally
            {
                Program.FlushPersonCache();

                this.EndLongAction();
            }

            using (var castForm = new EpisodeForm(episodes))
            {
                castForm.ShowDialog(this);
            }
        }

        private void ParseIMDb(EpisodeInfo episode)
        {
            var defaultValues = this.GetDefaultValues();

            ParseCastAndCrew(episode.Link, true, true, true, true, ref episode.CastMatches, ref episode.CastList, ref episode.CrewMatches, ref episode.CrewList, ref episode.SoundtrackMatches);

            _suppressProgress = true;

            try
            {
                this.ProcessLines(episode.CastList, episode.CastMatches, episode.CrewList, episode.CrewMatches, episode.SoundtrackMatches, defaultValues);
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

        private void OnCloseButtonClick(object sender, EventArgs e) => this.Close();

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
                this.ScanRows(DataGridView.Rows);
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