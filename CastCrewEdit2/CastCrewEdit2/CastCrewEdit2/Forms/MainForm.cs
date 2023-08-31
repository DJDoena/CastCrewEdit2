namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using System.Windows.Forms;
    using DVDProfilerHelper;
    using Extended;
    using Helper;
    using Resources;

    [ComVisible(true)]
    public partial class MainForm : CastCrewEdit2ParseBaseForm, IOleClientSite, IDocHostShowUI
    {
#pragma warning disable CS0618 // Type or member is obsolete

        private readonly bool _skipVersionCheck;

        private readonly BrowserControlSelection _selectedBrowserControl;

        private static readonly Regex _seasonsBeginRegex;

        private static readonly Regex _seasonsEndRegex;

        private static readonly Regex _seasonRegexOldStyle;

        private static readonly Regex _seasonRegexNewStyle;

        private static readonly Regex _episodeStartRegexOldStyle;

        private static readonly Regex _episodeLinkRegexOldStyle;

        private static readonly Regex _episodeNumberRegexOldStyle;

        private static readonly Regex _episodeNameRegexOldStyle;

        private static readonly Regex _episodeEndRegexOldStyle;

        private static readonly Regex _episodeRegexNewStyle;

        private string _movieTitle;

        private string _movieTitleLink;

        private List<Match> _castMatches;

        private List<KeyValuePair<Match, List<Match>>> _crewMatches;

        private Dictionary<string, List<SoundtrackMatch>> _soundtrackMatches;

        private List<CastInfo> _castList;

        private List<CrewInfo> _crewList;

        private readonly Control WebBrowser;

        static MainForm()
        {
            _seasonsBeginRegex = new Regex("<label for=\"bySeason\">Season:</label>", RegexOptions.Compiled);

            _seasonsEndRegex = new Regex("</select>", RegexOptions.Compiled);

            _seasonRegexOldStyle = new Regex("<option( +)(selected=\"selected\")?( +)value=\"(?'SeasonNumber'[0-9]+)\"", RegexOptions.Compiled);

            _episodeStartRegexOldStyle = new Regex("<div class=\"list_item (even|odd)\">", RegexOptions.Compiled);

            _episodeLinkRegexOldStyle = new Regex("href=\"/title/(?'EpisodeLink'[a-z0-9]+)/", RegexOptions.Compiled);

            _episodeNumberRegexOldStyle = new Regex("itemprop=\"episodeNumber\" content=\"(?'EpisodeNumber'[0-9,]+)\"", RegexOptions.Compiled);

            _episodeNameRegexOldStyle = new Regex("itemprop=\"name\">(?'EpisodeName'.*?)</a>", RegexOptions.Compiled);

            _episodeEndRegexOldStyle = new Regex("<div class=\"clear\">&nbsp;</div>", RegexOptions.Compiled);

            _seasonRegexNewStyle = new Regex("\"id\":\"(.*?)\",\"value\":\"(?'SeasonNumber'[0-9,]+)\"", RegexOptions.Compiled);

            _episodeRegexNewStyle = new Regex("\"id\":\"(?'EpisodeLink'[a-z0-9]+)\",\"type\":\"tvEpisode\",\"season\":\"(?'SeasonNumber'[0-9,]+)\",\"episode\":\"(?'EpisodeNumber'[0-9,]+)\",\"titleText\":\"(?'EpisodeName'.*?)\"", RegexOptions.Compiled);
        }

        public MainForm(bool skipVersionCheck, BrowserControlSelection selectedBrowserControl)
        {
            _movieTitle = string.Empty;

            _skipVersionCheck = skipVersionCheck;

            _selectedBrowserControl = selectedBrowserControl;

            _castMatches = new List<Match>();

            _crewMatches = new List<KeyValuePair<Match, List<Match>>>();

            _soundtrackMatches = new Dictionary<string, List<SoundtrackMatch>>();

            InitializeComponent();

            WebBrowser = InitWebBrowser();

            _progressBar = ProgressBar;

            Icon = Properties.Resource.djdsoft;

            Shown += OnMainFormShown;
        }

        private Control InitWebBrowser()
        {
            Control webBrowser;
            switch (_selectedBrowserControl)
            {
                case BrowserControlSelection.FormsDefault:
                    {
                        webBrowser = InitWebBrowserFormsDefault();

                        break;
                    }
                case BrowserControlSelection.WebViewCompatible:
                    {
                        webBrowser = InitWebBrowserWebViewCompatible();

                        break;
                    }
                case BrowserControlSelection.WebView:
                    {
                        webBrowser = InitWebBrowserWebView();

                        break;
                    }
                case BrowserControlSelection.WebView2:
                    {
                        webBrowser = InitWebBrowserWebView2();

                        break;
                    }
                default:
                    {
                        webBrowser = null;

                        break;
                    }
            }

            if (webBrowser != null)
            {
                var supportInitialize = webBrowser as System.ComponentModel.ISupportInitialize;

                supportInitialize?.BeginInit();

                webBrowser.Name = "WebBrowser";
                webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                webBrowser.Location = new Point(9, 64);
                webBrowser.Size = new Size(BrowserTab.Size.Width - 15, BrowserTab.Size.Height - 40);

                supportInitialize?.EndInit();

                BrowserTab.Controls.Add(webBrowser);
            }

            return webBrowser;
        }

        private Control InitWebBrowserFormsDefault()
        {
            var webBrowser = new System.Windows.Forms.WebBrowser()
            {
                AllowWebBrowserDrop = false,
                ScriptErrorsSuppressed = true,
            };

            webBrowser.Navigating += OnWebBrowserNavigating;
            webBrowser.Navigated += OnWebBrowserNavigated;

            return webBrowser;
        }

        private Control InitWebBrowserWebViewCompatible()
        {
            var webBrowser = new Microsoft.Toolkit.Forms.UI.Controls.WebViewCompatible();

            webBrowser.NavigationStarting += OnWebViewNavigationStarting;
            webBrowser.NavigationCompleted += OnWebViewNavigationCompleted;

            return webBrowser;
        }

        private Control InitWebBrowserWebView()
        {
            var webBrowser = new Microsoft.Toolkit.Forms.UI.Controls.WebView();

            webBrowser.NavigationStarting += OnWebViewNavigationStarting;
            webBrowser.NavigationCompleted += OnWebViewNavigationCompleted;

            return webBrowser;
        }

        private Control InitWebBrowserWebView2()
        {
            var webBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();

            webBrowser.NavigationStarting += OnWebView2NavigationStarting;
            webBrowser.NavigationCompleted += OnWebView2NavigationCompleted;

            return webBrowser;
        }

        private void OnMovieScanPageButtonClick(object sender, EventArgs e)
        {
            var failed = false;

            StartLongAction();

            try
            {
                _movieTitle = string.Empty;

                MovieCastDataGridView.Rows.Clear();

                MovieCrewDataGridView.Rows.Clear();

                if (MovieUrlTextBox.Text.Length == 0)
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsEmpty, MessageBoxTexts.UrlIsEmptyHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                var match = IMDbParser.TitleUrlRegex.Match(MovieUrlTextBox.Text);

                if (!match.Success || !match.Groups["TitleLink"].Success)
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsIncorrect, MessageBoxTexts.UrlIsIncorrectHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
                try
                {
                    _movieTitleLink = match.Groups["TitleLink"].Value.ToString();

                    ParseIMDb(_movieTitleLink);

                    if (!Program.DefaultValues.DisableParsingCompleteMessageBox
                        && !Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing
                        && !Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
                    {
                        ProcessMessageQueue();

                        MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (AggregateException ex)
                {
                    failed = true;

                    MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Program.WriteError(ex);
                }
                catch (Exception ex)
                {
                    failed = true;

                    MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Program.WriteError(ex);
                }
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
            finally
            {
                if (!failed && !Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
                {
                    Program.FlushPersonCache();
                }

                PersonsInLocalCacheLabel.Text = Program.PersonCacheCountString;

                SetMovieFormText();

                EndLongActionWithGrids();
            }

            if (!failed && Program.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                GetBirthYears(Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing);
            }

            if (!failed && Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                OnGetHeadshotsButtonClick(sender, e);
            }

            ProcessMessageQueue();

            if (!failed)
            {
                DataGridViewHelper.CopyCastToClipboard(MovieCastDataGridView, _movieTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
                DataGridViewHelper.CopyCrewToClipboard(MovieCrewDataGridView, _movieTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
            }
        }

        private void EndLongActionWithGrids()
        {
            EndLongAction();

            MovieCastDataGridView.Refresh();
            MovieCrewDataGridView.Refresh();
        }

        private void SetMovieFormText()
        {
            if (!string.IsNullOrEmpty(_movieTitle))
            {
                Text = "Cast/Crew Edit 2 - " + _movieTitle;
            }
            else
            {
                Text = "Cast/Crew Edit 2";
            }
        }

        private void ParseIMDb(string key)
        {
            var defaultValues = GetDefaultValues();

            _castList = new List<CastInfo>();
            _crewList = new List<CrewInfo>();

            ParseTitle(key);

            ParseCastAndCrew(key, ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, ParseCrewCheckBox.Checked, false, ref _castMatches, ref _castList, ref _crewMatches, ref _crewList, ref _soundtrackMatches);

            try
            {
                var progressMax = _castMatches.Count;

                foreach (var kvp in _crewMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                foreach (var kvp in _soundtrackMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                StartProgress(progressMax, Color.LightBlue);

                ProcessLines(_castList, _castMatches, _crewList, _crewMatches, _soundtrackMatches, defaultValues);
            }
            finally
            {
                EndProgress();
            }

            UpdateUI();

            ParseTrivia();

            ParseGoofs();
        }

        private void ParseTitle(string key)
        {
            var targetUrl = $"{IMDbParser.TitleUrl}{key}/fullcredits";

            var webSite = IMDbParser.GetWebSite(targetUrl);

            #region Parse for Title

            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    if (string.IsNullOrEmpty(_movieTitle))
                    {
                        var titleMatch = IMDbParser.TitleRegex.Match(line);

                        if (titleMatch.Success)
                        {
                            _movieTitle = HttpUtility.HtmlDecode(titleMatch.Groups["Title"].Value);

                            _movieTitle = _movieTitle.Replace(" - IMDb", string.Empty).Replace(" - Full Cast & Crew", string.Empty).Trim();

                            CreateTitleRow();

                            break;
                        }
                    }
                }
            }

            #endregion
        }

        private void ParseTrivia()
        {
            TriviaTextBox.Text = string.Empty;

            if (Program.DefaultValues.DownloadTrivia)
            {
                var triviaUrl = $"{IMDbParser.TitleUrl}{_movieTitleLink}/trivia";

                var webSite = IMDbParser.GetWebSite(triviaUrl);

                using (var sr = new StringReader(webSite))
                {
                    var triviaFound = false;

                    var trivia = new StringBuilder();

                    while (sr.Peek() != -1)
                    {
                        var line = sr.ReadLine();

                        if (!triviaFound)
                        {
                            var beginMatch = IMDbParser.TriviaStartRegex.Match(line);

                            if (beginMatch.Success)
                            {
                                triviaFound = true;

                                continue;
                            }
                        }

                        if (triviaFound)
                        {
                            trivia.AppendLine(line);
                        }
                    }

                    if (trivia.Length > 0)
                    {
                        ParseTrivia(trivia, triviaUrl);
                    }
                }
            }
        }

        private void ParseTrivia(StringBuilder trivia, string triviaUrl)
        {
            var matches = IMDbParser.TriviaLiRegex.Matches(trivia.ToString());

            trivia = new StringBuilder();

            trivia.AppendLine("<div style=\"display:none\">");

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups["Trivia"].Success)
                    {
                        var value = match.Groups["Trivia"].Value.Trim();

                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Replace("href=\"#", "href=\"" + triviaUrl + "#");
                            value = value.Replace("href=\"/", "href=\"" + IMDbParser.BaseUrl + "/");
                            value = value.Replace("href=\"?", "href=\"" + IMDbParser.BaseUrl + "?");
                            value = value.Replace(" />", ">");
                            value = value.Replace("/>", ">");
                            value = value.Trim();

                            while (value.EndsWith("<br>"))
                            {
                                value = value.Substring(0, value.Length - 4).TrimEnd();
                            }

                            trivia.AppendLine("<trivia=" + value + " />");
                            trivia.AppendLine();
                        }
                    }
                }
            }

            trivia.AppendLine("</div>");

            TriviaTextBox.Text = trivia.ToString();
        }

        private void ParseGoofs()
        {
            GoofsTextBox.Text = string.Empty;

            if (Program.DefaultValues.DownloadGoofs)
            {
                var goofsUrl = $"{IMDbParser.TitleUrl}{_movieTitleLink}/goofs";

                var webSite = IMDbParser.GetWebSite(goofsUrl);

                using (var sr = new StringReader(webSite))
                {
                    var goofsFound = false;

                    var goofs = new StringBuilder();

                    while (sr.Peek() != -1)
                    {
                        var line = sr.ReadLine();

                        if (!goofsFound)
                        {
                            var beginMatch = IMDbParser.GoofsStartRegex.Match(line);

                            if (beginMatch.Success)
                            {
                                goofsFound = true;

                                continue;
                            }
                        }

                        if (goofsFound)
                        {
                            goofs.AppendLine(line);
                        }
                    }

                    if (goofs.Length > 0)
                    {
                        ParseGoofs(goofs, goofsUrl);
                    }
                }
            }
        }

        private void ParseGoofs(StringBuilder goofs, string goofsUrl)
        {
            var matches = IMDbParser.GoofsLiRegex.Matches(goofs.ToString());

            goofs = new StringBuilder();

            goofs.AppendLine("<div style=\"display:none\">");

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups["Goof"].Success)
                    {
                        var value = match.Groups["Goof"].Value.Trim();

                        var spoilerMatch = IMDbParser.GoofSpoilerRegex.Match(value);

                        if (spoilerMatch.Success)
                        {
                            value = spoilerMatch.Groups["Goof"].Value;
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Replace("&nbsp;", " ");
                            value = value.Replace("href=\"#", "href=\"" + goofsUrl + "#");
                            value = value.Replace("href=\"/", "href=\"" + IMDbParser.BaseUrl + "/");
                            value = value.Replace("href=\"?", "href=\"" + IMDbParser.BaseUrl + "?");
                            value = value.Replace(" />", ">");
                            value = value.Replace("/>", ">");
                            value = value.Trim();

                            while (value.EndsWith("<br>"))
                            {
                                value = value.Substring(0, value.Length - 4).TrimEnd();
                            }

                            goofs.AppendLine("<goof=" + value + " />");
                            goofs.AppendLine();
                        }
                    }
                }
            }

            goofs.AppendLine("</div>");

            GoofsTextBox.Text = goofs.ToString();
        }

        private void CreateTitleRow()
        {
            if (ParseCastCheckBox.Checked)
            {
                var title = new CastInfo(-1)
                {
                    FirstName = FirstNames.Title,
                    MiddleName = string.Empty,
                    LastName = _movieTitle,
                    BirthYear = string.Empty,
                    Role = string.Empty,
                    Voice = "False",
                    Uncredited = "False",
                    CreditedAs = string.Empty,
                    PersonLink = _movieTitleLink,
                };

                _castList.Add(title);
            }

            if (ParseCrewCheckBox.Checked)
            {
                var title = new CrewInfo()
                {
                    FirstName = FirstNames.Title,
                    MiddleName = string.Empty,
                    LastName = _movieTitle,
                    BirthYear = string.Empty,
                    CreditType = null,
                    CreditSubtype = null,
                    CreditedAs = string.Empty,
                    CustomRole = string.Empty,
                    PersonLink = _movieTitleLink,
                };

                _crewList.Add(title);
            }
        }

        private void UpdateUI()
        {
            UpdateUI(_castList, _crewList, MovieCastDataGridView, MovieCrewDataGridView, ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, _movieTitleLink, _movieTitle);

            if (_log.Length > 0)
            {
                _log.Show(LogWebBrowser);
            }
        }

        private async void OnMainFormShown(object sender, EventArgs e)
        {
            SuspendLayout();

            LayoutForm();

            CreateDataGridViewColumns();

            SetCheckBoxes();

            if (ItsMe)
            {
                MenuStrip.Items.Add(sessionDataToolStripMenuItem);
            }

            ResumeLayout();

            BirthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;

            PersonsInLocalCacheLabel.Text = Program.PersonCacheCountString;

            RegisterEvents();

            if (Program.Settings.CurrentVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                OpenReadme();

                Program.Settings.CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            CheckForNewVersion(true);

            await InitialNavigate();

            BrowserSearchTextBox.Focus();
        }

        private async Task InitialNavigate()
        {
            if (_selectedBrowserControl == BrowserControlSelection.WebView2)
            {
                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, Path.Combine(Path.GetTempPath(), "CCE2browser"));

                await ((Microsoft.Web.WebView2.WinForms.WebView2)WebBrowser).EnsureCoreWebView2Async(environment);
            }

            NavigateTo("https://www.imdb.com/find?s=tt&q=");
        }

        private void CheckForNewVersion(bool silently)
        {
            if (silently)
            {
                if (!_skipVersionCheck)
                {
                    OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "CastCrewEdit2", GetType().Assembly, silently);
                }
            }
            else
            {
                OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "CastCrewEdit2", GetType().Assembly, silently);
            }
        }

        private void CreateDataGridViewColumns()
        {
            DataGridViewHelper.CreateCastColumns(MovieCastDataGridView);
            DataGridViewHelper.CreateCrewColumns(MovieCrewDataGridView);

            var seasonDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn()
            {
                Name = "Season Number",
                HeaderText = DataGridViewTexts.SeasonNumber,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Resizable = DataGridViewTriState.True,
            };

            TVShowSeasonsDataGridView.Columns.Add(seasonDataGridViewTextBoxColumn);
        }

        private void LayoutForm()
        {
            if (Program.Settings.MainForm.WindowState == FormWindowState.Normal)
            {
                Left = Program.Settings.MainForm.Left;

                Top = Program.Settings.MainForm.Top;

                if (Program.Settings.MainForm.Width > MinimumSize.Width)
                {
                    Width = Program.Settings.MainForm.Width;
                }
                else
                {
                    Width = MinimumSize.Width;
                }

                if (Program.Settings.MainForm.Height > MinimumSize.Height)
                {
                    Height = Program.Settings.MainForm.Height;
                }
                else
                {
                    Height = MinimumSize.Height;
                }
            }
            else
            {
                Left = Program.Settings.MainForm.RestoreBounds.X;

                Top = Program.Settings.MainForm.RestoreBounds.Y;

                if (Program.Settings.MainForm.RestoreBounds.Width > MinimumSize.Width)
                {
                    Width = Program.Settings.MainForm.RestoreBounds.Width;
                }
                else
                {
                    Width = MinimumSize.Width;
                }

                if (Program.Settings.MainForm.RestoreBounds.Height > MinimumSize.Height)
                {
                    Height = Program.Settings.MainForm.RestoreBounds.Height;
                }
                else
                {
                    Height = MinimumSize.Height;
                }
            }

            if (Program.Settings.MainForm.WindowState != FormWindowState.Minimized)
            {
                WindowState = Program.Settings.MainForm.WindowState;
            }
        }

        private void RegisterEvents()
        {
            MovieCastDataGridView.CellValueChanged += OnMovieCastDataGridViewCellValueChanged;

            MovieCrewDataGridView.CellValueChanged += OnMovieCrewDataGridViewCellValueChanged;

            MovieCastDataGridView.CellContentClick += OnDataGridViewCellContentClick;

            MovieCrewDataGridView.CellContentClick += OnDataGridViewCellContentClick;

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

            FormClosing += OnMainFormClosing;

            KeyPreview = true;

            KeyDown += OnMainFormKeyDown;
        }

        private void OnMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.DefaultValues.SaveLogFile && _log.Length > 0)
            {
                using (var sw = new StreamWriter(Program.LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine(_log.ToString());
                }
            }

            Program.Settings.MainForm.Left = Left;
            Program.Settings.MainForm.Top = Top;
            Program.Settings.MainForm.Width = Width;
            Program.Settings.MainForm.Height = Height;
            Program.Settings.MainForm.WindowState = WindowState;
            Program.Settings.MainForm.RestoreBounds = RestoreBounds;
        }

        private void OnGetBirthYearsButtonClick(object sender, EventArgs e)
        {
            GetBirthYears(false);

            ProcessMessageQueue();
        }

        private void GetBirthYears(bool parseHeadshotsFollows) => GetBirthYears(parseHeadshotsFollows, MovieCastDataGridView, MovieCrewDataGridView, BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);

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

        private void OnMovieCrewDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e) => DataGridViewHelper.OnCrewDataGridViewCellValueChanged(sender, e);

        private void OnMovieCastDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e) => DataGridViewHelper.OnCastDataGridViewCellValueChanged(sender, e);

        private void OnMovieCastGenerateButtonClick(object sender, EventArgs e) => GenerateCastXml(true);

        private string GenerateCastXml(bool showMessageBox) => GenerateCastXml(MovieCastDataGridView, _movieTitle, showMessageBox, LogWebBrowser);

        private void OnMovieCrewGenerateButtonClick(object sender, EventArgs e) => GenerateCrewXml(true);

        private string GenerateCrewXml(bool showMessageBox) => GenerateCrewXml(MovieCrewDataGridView, _movieTitle, showMessageBox, LogWebBrowser);

        private void OnTVShowScanPageButtonClick(object sender, EventArgs e)
        {
            StartLongAction();

            try
            {
                _tvShowTitle = string.Empty;

                TVShowSeasonsDataGridView.Rows.Clear();

                if (TVShowUrlTextBox.Text.Length == 0)
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsEmpty, MessageBoxTexts.UrlIsEmptyHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                var match = IMDbParser.TitleUrlRegex.Match(TVShowUrlTextBox.Text);

                if (!match.Success || !match.Groups["TitleLink"].Success)
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsIncorrect, MessageBoxTexts.UrlIsIncorrectHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                _tvShowTitleLink = match.Groups["TitleLink"].Value.ToString();

                try
                {
                    ScanForSeasons();

                    if (!Program.DefaultValues.DisableParsingCompleteMessageBox)
                    {
                        ProcessMessageQueue();

                        MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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
            finally
            {
                SetTVShowFormText();

                EndLongActionWithGrids();
            }
        }

        private void SetTVShowFormText()
        {
            if (!string.IsNullOrEmpty(_tvShowTitle))
            {
                Text = "Cast/Crew Edit 2 For TV Shows - " + Resources.Seasons + " - " + _tvShowTitle;
            }
            else
            {
                Text = "Cast/Crew Edit 2 For TV Shows";
            }
        }

        private void OnScanForEpisodesButtonClick(object sender, EventArgs e)
        {
            if (TVShowSeasonsDataGridView.SelectedRows == null || TVShowSeasonsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, MessageBoxTexts.NoSeasonSelected, MessageBoxTexts.NoSeasonSelectedHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            else
            {
                StartLongAction();

                SuspendLayout();

                var episodes = new List<EpisodeInfo>();

                try
                {
                    var seasons = new List<int>(TVShowSeasonsDataGridView.SelectedRows.Count);

                    for (var rowIndex = 0; rowIndex < TVShowSeasonsDataGridView.SelectedRows.Count; rowIndex++)
                    {
                        seasons.Add(int.Parse(TVShowSeasonsDataGridView.SelectedRows[rowIndex].Cells["Season Number"].Value.ToString()));
                    }

                    seasons.Sort();

                    foreach (var season in seasons)
                    {
                        var targetUrl = $"{IMDbParser.TitleUrl}{_tvShowTitleLink}/episodes?season={season}";

                        var webSite = IMDbParser.GetWebSite(targetUrl);

                        var seasonEpisodes = ScanForEpisodesNewStyle(webSite);

                        if (seasonEpisodes.Count == 0)
                        {
                            seasonEpisodes = ScanForEpisodesOldStyle(webSite, season.ToString());
                        }

                        episodes.AddRange(seasonEpisodes);
                    }
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
                finally
                {
                    ResumeLayout();

                    EndLongActionWithGrids();
                }

                if (episodes.Count == 0)
                {
                    MessageBox.Show(this, MessageBoxTexts.NoEpisodesFound, MessageBoxTexts.NoEpisodesFoundHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
                else
                {
                    if (!Program.DefaultValues.DisableParsingCompleteMessageBox)
                    {
                        ProcessMessageQueue();

                        MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    using (var episodesForm = new EpisodesForm(episodes))
                    {
                        _settingsHaveChanged = false;

                        episodesForm.ShowDialog(this);

                        if (_settingsHaveChanged)
                        {
                            SetCheckBoxes();

                            _settingsHaveChanged = false;
                        }

                        _log.Show(LogWebBrowser);
                    }
                }

                BirthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;

                PersonsInLocalCacheLabel.Text = Program.PersonCacheCountString;
            }
        }

        private static List<EpisodeInfo> ScanForEpisodesNewStyle(string webSite)
        {
            var episodes = new List<EpisodeInfo>();

            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    //var episodeStartMatch = _episodeStartRegex.Match(line);

                    var indexOfEpisodes = line.IndexOf("\"episodes\":", StringComparison.InvariantCultureIgnoreCase);

                    if (indexOfEpisodes != -1)
                    {
                        line = line.Substring(indexOfEpisodes);

                        while (!line.Contains("</script>"))
                        {
                            line += sr.ReadLine();
                        }

                        var indexOfScript = line.IndexOf("</script>", StringComparison.InvariantCultureIgnoreCase);

                        if (indexOfScript != -1)
                        {
                            line = line.Substring(0, indexOfScript);
                        }

                        var indexOfTotal = line.IndexOf("\"total\":", StringComparison.InvariantCultureIgnoreCase);

                        if (indexOfTotal != -1)
                        {
                            line = line.Substring(0, indexOfTotal);
                        }

                        var matches = _episodeRegexNewStyle.Matches(line);

                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                var episodeInfo = new EpisodeInfo()
                                {
                                    Link = match.Groups["EpisodeLink"].Value.ToString(),
                                    SeasonNumber = match.Groups["SeasonNumber"].Value.ToString(),
                                    EpisodeNumber = match.Groups["EpisodeNumber"].Value.ToString(),
                                    EpisodeName = HttpUtility.HtmlDecode(match.Groups["EpisodeName"].Value.ToString()),
                                };

                                episodes.Add(episodeInfo);
                            }

                            break;
                        }
                    }
                }
            }

            return episodes;
        }

        private static List<EpisodeInfo> ScanForEpisodesOldStyle(string webSite, string season)
        {
            var episodes = new List<EpisodeInfo>();

            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    var episodeStartMatch = _episodeStartRegexOldStyle.Match(line);

                    if (episodeStartMatch.Success)
                    {
                        var episodeLinkFound = false;

                        var episodeNumberFound = false;

                        var episodeNameFound = false;

                        var episodeInfo = new EpisodeInfo();

                        var parts = EpisodeParts.None;

                        while (!_episodeEndRegexOldStyle.Match(line).Success)
                        {
                            line = sr.ReadLine();

                            if (!episodeLinkFound)
                            {
                                var match = _episodeLinkRegexOldStyle.Match(line);

                                if (match.Success)
                                {
                                    episodeInfo.Link = match.Groups["EpisodeLink"].Value.ToString();

                                    parts |= EpisodeParts.Link;

                                    episodeLinkFound = true;

                                    continue;
                                }
                            }

                            if (!episodeNumberFound)
                            {
                                var match = _episodeNumberRegexOldStyle.Match(line);

                                if (match.Success)
                                {
                                    episodeInfo.EpisodeNumber = match.Groups["EpisodeNumber"].Value.ToString();

                                    parts |= EpisodeParts.Number;

                                    episodeNumberFound = true;

                                    continue;
                                }
                            }

                            if (!episodeNameFound)
                            {
                                var match = _episodeNameRegexOldStyle.Match(line);

                                if (match.Success)
                                {
                                    episodeInfo.EpisodeName = HttpUtility.HtmlDecode(match.Groups["EpisodeName"].Value.ToString());

                                    parts |= EpisodeParts.Name;

                                    episodeNameFound = true;

                                    continue;
                                }
                            }
                        }

                        episodeInfo.SeasonNumber = season;

                        if (parts == (EpisodeParts.Link | EpisodeParts.Name | EpisodeParts.Number))
                        {
                            episodes.Add(episodeInfo);
                        }
                    }
                }
            }

            return episodes;
        }

        private void ScanForSeasons()
        {
            var targetUrl = $"{IMDbParser.TitleUrl}{_tvShowTitleLink}/episodes";

            var webSite = IMDbParser.GetWebSite(targetUrl);

            if (!ScanForSeasonsNewStyle(webSite))
            {
                ScanForSeasonsOldStyle(webSite);
            }
        }

        private bool ScanForSeasonsNewStyle(string webSite)
        {
            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    if (string.IsNullOrEmpty(_tvShowTitle))
                    {
                        var titleMatch = IMDbParser.TitleRegex.Match(line);

                        if (titleMatch.Success)
                        {
                            _tvShowTitle = HttpUtility.HtmlDecode(titleMatch.Groups["Title"].Value);

                            _tvShowTitle = _tvShowTitle.Replace(" - IMDb", string.Empty).Replace(" - Episodes", string.Empty).Trim();

                            if (_tvShowTitle.EndsWith(" list"))
                            {
                                _tvShowTitle = _tvShowTitle.Substring(0, _tvShowTitle.Length - 5);
                            }

                            continue;
                        }
                    }

                    var indexOfSeasons = line.IndexOf("\"seasons\":", StringComparison.InvariantCultureIgnoreCase);

                    if (indexOfSeasons != -1)
                    {
                        line = line.Substring(indexOfSeasons);

                        while (!line.Contains("</script>"))
                        {
                            line += sr.ReadLine();
                        }

                        var indexOfScript = line.IndexOf("</script>", StringComparison.InvariantCultureIgnoreCase);

                        if (indexOfScript != -1)
                        {
                            line = line.Substring(0, indexOfScript);
                        }

                        var indexOfYears = line.IndexOf("\"years\":", StringComparison.InvariantCultureIgnoreCase);

                        if (indexOfYears != -1)
                        {
                            line = line.Substring(0, indexOfYears);
                        }

                        var matches = _seasonRegexNewStyle.Matches(line);

                        if (matches.Count > 0)
                        {
                            var seasons = new HashSet<string>();

                            foreach (Match match in matches)
                            {
                                seasons.Add(match.Groups["SeasonNumber"].Value.ToString());
                            }

                            foreach (var season in seasons)
                            {
                                var row = TVShowSeasonsDataGridView.Rows[TVShowSeasonsDataGridView.Rows.Add()];

                                row.DefaultCellStyle.BackColor = Color.White;

                                row.Cells["Season Number"].Value = season;
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool ScanForSeasonsOldStyle(string webSite)
        {
            var hasSeasons = false;

            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    if (string.IsNullOrEmpty(_tvShowTitle))
                    {
                        var titleMatch = IMDbParser.TitleRegex.Match(line);

                        if (titleMatch.Success)
                        {
                            _tvShowTitle = HttpUtility.HtmlDecode(titleMatch.Groups["Title"].Value);

                            _tvShowTitle = _tvShowTitle.Replace(" - IMDb", string.Empty).Replace(" - Episodes", string.Empty).Trim();

                            continue;
                        }
                    }

                    var seasonsMatch = _seasonsBeginRegex.Match(line);

                    if (seasonsMatch.Success)
                    {
                        while (sr.Peek() != -1)
                        {
                            line = sr.ReadLine();

                            seasonsMatch = _seasonsEndRegex.Match(line);

                            if (seasonsMatch.Success)
                            {
                                return hasSeasons;
                            }

                            var seasonMatch = _seasonRegexOldStyle.Match(line);

                            if (seasonMatch.Success)
                            {
                                if (seasonMatch.Success)
                                {
                                    var row = TVShowSeasonsDataGridView.Rows[TVShowSeasonsDataGridView.Rows.Add()];

                                    row.DefaultCellStyle.BackColor = Color.White;

                                    row.Cells["Season Number"].Value = seasonMatch.Groups["SeasonNumber"].Value.ToString();

                                    hasSeasons = true;
                                }
                            }
                        }
                    }
                }
            }

            return hasSeasons;
        }

        private void OnMovieTVShowTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            var tabControl = (TabControl)sender;

            if (tabControl.SelectedTab == MovieTab)
            {
                SetMovieFormText();
            }
            else if (tabControl.SelectedTab == TVShowTab)
            {
                SetTVShowFormText();
            }
        }

        private void OnSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(true, true))
            {
                settingsForm.SetValues(Program.Settings.SettingsForm.Left, Program.Settings.SettingsForm.Top, Program.DefaultValues);

                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    SetCheckBoxes();
                }

                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnReApplySettingsAndFiltersButtonClick(object sender, EventArgs e)
        {
            StartLongAction();

            MovieCastDataGridView.Rows.Clear();

            MovieCrewDataGridView.Rows.Clear();

            _castList = new List<CastInfo>();

            _crewList = new List<CrewInfo>();

            CreateTitleRow();

            var defaultValues = GetDefaultValues();

            try
            {
                var progressMax = _castMatches.Count;

                foreach (var kvp in _crewMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                foreach (var kvp in _soundtrackMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                StartProgress(progressMax, Color.LightBlue);

                ProcessLines(_castList, _castMatches, _crewList, _crewMatches, _soundtrackMatches, defaultValues);
            }
            finally
            {
                EndProgress();
            }

            UpdateUI();

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
                GetBirthYears(Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing);
            }
            else
            {
                Program.FlushPersonCache();
            }

            if (Program.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                OnGetHeadshotsButtonClick(sender, e);
            }

            ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(MovieCastDataGridView, _movieTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(MovieCrewDataGridView, _movieTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage, true);
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
            defaultValues.IncludeCustomCredits = CustomCreditsCheckBox.Checked;
            defaultValues.RetainCastCreditedAs = RetainCreditedAsOnCastCheckBox.Checked;
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

        protected override void MoveRow(CastInfo castMember, bool up)
        {
            var index = FindIndexOfCastMember(_castList, castMember);

            if (index != -1)
            {
                var temp = _castList[index];

                if (up)
                {
                    _castList[index] = _castList[index - 1];
                    _castList[index - 1] = temp;
                }
                else
                {
                    _castList[index] = _castList[index + 1];
                    _castList[index + 1] = temp;
                }

                MovieCastDataGridView.Rows.Clear();

                UpdateUI(_castList, null, MovieCastDataGridView, null, true, false, _movieTitleLink, _movieTitle);
            }
            else
            {
                Debug.Assert(false, "Invalid Index");
            }
        }

        protected override void RemoveRow(CastInfo castMember)
        {
            var index = FindIndexOfCastMember(_castList, castMember);

            if (index != -1)
            {
                _castList.RemoveAt(index);

                MovieCastDataGridView.Rows.Clear();

                UpdateUI(_castList, null, MovieCastDataGridView, null, true, false, _movieTitleLink, _movieTitle);
            }
            else
            {
                Debug.Assert(false, "Invalid Index");
            }
        }

        private void OnGetHeadshotsButtonClick(object sender, EventArgs e) => GetHeadshots(MovieCastDataGridView, MovieCrewDataGridView, GetHeadshotsButton);

        private void OnBrowseButtonClick(object sender, EventArgs e) => NavigateTo(BrowserUrlComboBox.Text);

        private void OnBrowserSearchButtonClick(object sender, EventArgs e)
        {
            const string BaseUrl = "https://www.imdb.com/find?s=tt&q=";

            var url = BaseUrl + System.Web.HttpUtility.UrlEncode(BrowserSearchTextBox.Text);

            NavigateTo(url);
        }

        private void NavigateTo(string url)
        {
            switch (_selectedBrowserControl)
            {
                case BrowserControlSelection.FormsDefault:
                    {
                        ((System.Windows.Forms.WebBrowser)WebBrowser).Navigate(url);

                        break;
                    }
                case BrowserControlSelection.WebViewCompatible:
                    {
                        ((Microsoft.Toolkit.Forms.UI.Controls.WebViewCompatible)WebBrowser).Navigate(url);

                        break;
                    }
                case BrowserControlSelection.WebView:
                    {
                        ((Microsoft.Toolkit.Forms.UI.Controls.WebView)WebBrowser).Navigate(url);

                        break;
                    }
                case BrowserControlSelection.WebView2:
                    {
                        ((Microsoft.Web.WebView2.WinForms.WebView2)WebBrowser).Source = new Uri(url);

                        break;
                    }
            }
        }

        private void OnBrowserUrlComboBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnBrowseButtonClick(this, null);
            }
        }

        private void OnBrowserSearchTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnBrowserSearchButtonClick(this, null);
            }
        }

        private void OnCopyTriviaToClipboardButtonClick(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(TriviaTextBox.Text, true, 4, 250);
            }
            catch
            { }
        }

        private void OnBackupToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                DefaultExt = "bin",
                Filter = "Binary Files|*.bin",
                OverwritePrompt = true,
                RestoreDirectory = true,
                FileName = "cce2.bin",
                Title = "Select Session Data backup file",
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SessionData.Serialize(sfd.FileName);
                }
            }
        }

        private void OnRestoreToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()
            {
                DefaultExt = "bin",
                Filter = "Binary Files|*.bin",
                Multiselect = false,
                RestoreDirectory = true,
                Title = "Select Session Data backup file",
                FileName = "cce2.bin",
                CheckFileExists = true,
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BirthYearsInLocalCacheLabel.Text = SessionData.Deserialize(ofd.FileName);
                }
            }
        }

        private void OnCopyGoofsToClipboardButtonClick(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(GoofsTextBox.Text, true, 4, 250);
            }
            catch
            { }
        }

        private void OnCheckForUpdateToolStripMenuItemClick(object sender, EventArgs e) => CheckForNewVersion(false);

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

            HasAgreed = true;

            DataGridViewHelper.CopyExtendedCastToClipboard(MovieCastDataGridView, _movieTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage);

            _log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (!Program.DefaultValues.DisableCopyingSuccessfulMessageBox)
            {
                MessageBox.Show(this, MessageBoxTexts.CastDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnCopyExtendedCrewToClipboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!HasAgreed)
            {
                if (MessageBox.Show(this, MessageBoxTexts.DontContributeIMDbData, MessageBoxTexts.DontContributeIMDbDataHeader, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            HasAgreed = true;

            DataGridViewHelper.CopyExtendedCrewToClipboard(MovieCrewDataGridView, _movieTitle, _log, Program.DefaultValues.UseFakeBirthYears, AddMessage);

            _log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (!Program.DefaultValues.DisableCopyingSuccessfulMessageBox)
            {
                MessageBox.Show(this, MessageBoxTexts.CrewDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnWebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var obj = (IOleObject)((System.Windows.Forms.WebBrowser)WebBrowser).ActiveXInstance;

            obj.SetClientSite(this);
        }

        private void OnWebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e) => UpdateUri(e.Url);

        private void OnWebViewNavigationStarting(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationStartingEventArgs e) => UpdateUri(e.Uri);

        private void OnWebViewNavigationCompleted(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.WebViewControlNavigationCompletedEventArgs e) => UpdateUri(e.Uri);

        private void OnWebView2NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e) => UpdateUriFromWebView2();

        private void OnWebView2NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e) => UpdateUriFromWebView2();

        private void UpdateUriFromWebView2() => UpdateUri(((Microsoft.Web.WebView2.WinForms.WebView2)WebBrowser).Source);

        private void UpdateUri(Uri uri)
        {
            if (uri?.AbsoluteUri != null)
            {
                BrowserUrlComboBox.Text = uri.AbsoluteUri;

                MovieUrlTextBox.Text = BrowserUrlComboBox.Text;

                TVShowUrlTextBox.Text = BrowserUrlComboBox.Text;
            }
        }

        private async void OnMainFormKeyDown(object sender, KeyEventArgs e)
        {
            if (MovieCastCrewTabControl.Enabled)
            {
                if (MovieTVShowTabControl.SelectedIndex != 1)
                {
                    return;
                }
                else if (MovieTVShowTabControl.SelectedIndex == 1 && MovieUrlTextBox.Focused)
                {
                    return;
                }
                else if (CtrlSWasPressed(e))
                {
                    if (Program.DefaultValues.SendToCastCrewCopyPaste
                        && (MovieCastCrewTabControl.SelectedIndex == 0 || MovieCastCrewTabControl.SelectedIndex == 1))
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
                    if (MovieCastCrewTabControl.SelectedIndex == 0)
                    {
                        OnMovieCastGenerateButtonClick(this, EventArgs.Empty);
                    }
                    else if (MovieCastCrewTabControl.SelectedIndex == 1)
                    {
                        OnMovieCrewGenerateButtonClick(this, EventArgs.Empty);
                    }
                }
            }
        }

#pragma warning restore CS0618 // Type or member is obsolete

        [DispId(-5512)]
        public virtual int IDispatch_Invoke_Handler()
        {
            System.Diagnostics.Debug.WriteLine("-5512");

            return (int)(BrowserOptions.Images | BrowserOptions.DontRunActiveX | BrowserOptions.NoJava | BrowserOptions.NoScripts | BrowserOptions.NoActiveXDownload);
        }

        #region IOleClientSite Members

        public int SaveObject() => 0;

        public int GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker)
        {
            moniker = this;

            return 0;
        }

        public int GetContainer(out object container)
        {
            container = this;

            return 0;
        }

        public int ShowObject() => 0;

        public int OnShowWindow(int fShow) => 0;

        public int RequestNewObjectLayout() => 0;

        #endregion

        #region IDocHostShowUI Members

        //public void ShowMessage(int hwnd, ref int lpstrText, ref int lpstrCaption, uint dwType, ref int lpstrHelpFile, uint dwHelpContext, out int lpResult)
        //{
        //    lpResult = -1;
        //}

        #endregion

        #region IDocHostShowUI Members

        public uint ShowMessage(IntPtr hwnd, string lpstrText, string lpstrCaption, uint dwType, string lpstrHelpFile, uint dwHelpContext, out int lpResult)
        {
            if (!string.IsNullOrEmpty(lpstrText) && lpstrText.Contains("ActiveX"))
            {
                lpResult = 0;
            }
            else
            {
                MessageBox.Show(lpstrText, lpstrCaption);

                lpResult = 0;
            }

            return 0;
        }

        #endregion

        [Flags]
        private enum EpisodeParts
        {
            None = 0,

            Link = 1,

            Name = 2,

            Number = 4
        }
    }

    #region COM Interfaces
    public enum BrowserOptions : uint
    {
        /// <summary>
        /// No flags are set.
        /// </summary>
        None = 0,
        /// <summary>
        /// The browser will operate in offline mode. Equivalent to DLCTL_FORCEOFFLINE.
        /// </summary>
        AlwaysOffline = 0x10000000,
        /// <summary>
        /// The browser will play background sounds. Equivalent to DLCTL_BGSOUNDS.
        /// </summary>
        BackgroundSounds = 0x00000040,
        /// <summary>
        /// Specifies that the browser will not run Active-X controls. Use this setting to disable Flash movies. Equivalent to DLCTL_NO_RUNACTIVEXCTLS.
        /// </summary>
        DontRunActiveX = 0x00000200,

        DownloadOnly = 0x00000800,
        /// <summary>
        /// Specifies that the browser should fetch the content from the server. If the server's content is the same as the cache, the cache is used.Equivalent to DLCTL_RESYNCHRONIZE.
        /// </summary>
        IgnoreCache = 0x00002000,
        /// <summary>
        /// The browser will force the request from the server, and ignore the proxy, even if the proxy indicates the content is up to date.Equivalent to DLCTL_PRAGMA_NO_CACHE.
        /// </summary>
        IgnoreProxy = 0x00004000,
        /// <summary>
        /// Specifies that the browser should download and display images. This is set by default.
        /// Equivalent to DLCTL_DLIMAGES.
        /// </summary>
        Images = 0x00000010,
        /// <summary>
        /// Disables downloading and installing of Active-X controls.Equivalent to DLCTL_NO_DLACTIVEXCTLS.
        /// </summary>
        NoActiveXDownload = 0x00000400,
        /// <summary>
        /// Disables web behaviours.Equivalent to DLCTL_NO_BEHAVIORS.
        /// </summary>
        NoBehaviours = 0x00008000,
        /// <summary>
        /// The browser suppresses any HTML charset specified.Equivalent to DLCTL_NO_METACHARSET.
        /// </summary>
        NoCharSets = 0x00010000,
        /// <summary>
        /// Indicates the browser will ignore client pulls.Equivalent to DLCTL_NO_CLIENTPULL.
        /// </summary>
        NoClientPull = 0x20000000,
        /// <summary>
        /// The browser will not download or display Java applets.Equivalent to DLCTL_NO_JAVA.
        /// </summary>
        NoJava = 0x00000100,
        /// <summary>
        /// The browser will download framesets and parse them, but will not download the frames contained inside those framesets.Equivalent to DLCTL_NO_FRAMEDOWNLOAD.
        /// </summary>
        NoFrameDownload = 0x00080000,
        /// <summary>
        /// The browser will not execute any scripts.Equivalent to DLCTL_NO_SCRIPTS.
        /// </summary>
        NoScripts = 0x00000080,
        /// <summary>
        /// If the browser cannot detect any internet connection, this causes it to default to offline mode.Equivalent to DLCTL_OFFLINEIFNOTCONNECTED.
        /// </summary>
        OfflineIfNotConnected = 0x80000000,
        /// <summary>
        /// Specifies that UTF8 should be used.Equivalent to DLCTL_URL_ENCODING_ENABLE_UTF8.
        /// </summary>
        UTF8 = 0x00040000,
        /// <summary>
        /// The browser will download and display video media.Equivalent to DLCTL_VIDEOS.
        /// </summary>
        Videos = 0x00000020
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }

    [ComVisible(true), Guid("0000011B-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleContainer
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] object pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten, [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumObjects([In, MarshalAs(UnmanagedType.U4)] uint grfFlags, [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppenum);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int LockContainer([In, MarshalAs(UnmanagedType.Bool)] bool fLock);
    }

    [ComVisible(true), Guid("00000118-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleClientSite
    {
        [PreserveSig]
        int SaveObject();
        [PreserveSig]
        int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);
        [PreserveSig]
        int GetContainer(out object container);
        [PreserveSig]
        int ShowObject();
        [PreserveSig]
        int OnShowWindow(int fShow);
        [PreserveSig]
        int RequestNewObjectLayout();
    }

    [ComVisible(true), ComImport, Guid("00000112-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleObject
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetClientSite([In, MarshalAs(UnmanagedType.Interface)]IOleClientSite
        pClientSite);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetClientSite([Out, MarshalAs(UnmanagedType.Interface)] out IOleClientSite site);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Close([In, MarshalAs(UnmanagedType.U4)] uint dwSaveOption);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] object pmk);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwAssign, [In, MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker, [Out, MarshalAs(UnmanagedType.Interface)] out object moniker);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int InitFromData([In, MarshalAs(UnmanagedType.Interface)] object pDataObject, [In, MarshalAs(UnmanagedType.Bool)] bool fCreation, [In, MarshalAs(UnmanagedType.U4)] uint dwReserved);
        int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] uint dwReserved, out object data);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DoVerb([In, MarshalAs(UnmanagedType.I4)] int iVerb, [In] IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite, [In, MarshalAs(UnmanagedType.I4)] int lindex, [In] IntPtr hwndParent, [In] RECT lprcPosRect);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumVerbs(out object e); // IEnumOLEVERB
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OleUpdate();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsUpToDate();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetUserClassID([In, Out] ref Guid pClsid);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetUserType([In, MarshalAs(UnmanagedType.U4)] uint dwFormOfType, [Out, MarshalAs(UnmanagedType.LPWStr)] out string userType);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetExtent([In, MarshalAs(UnmanagedType.U4)] uint dwDrawAspect, [In] object pSizel); // tagSIZEL
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetExtent([In, MarshalAs(UnmanagedType.U4)] uint dwDrawAspect, [Out] object pSizel); // tagSIZEL
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Advise([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IAdviseSink pAdvSink, out int cookie);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumAdvise(out object e);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] uint dwAspect, out int misc);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetColorScheme([In] object pLogpal); // tagLOGPALETTE
    }

    [ComImport, Guid("B196B288-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleControl
    {
        [PreserveSig]
        int GetControlInfo([Out] object pCI);
        [PreserveSig]
        int OnMnemonic([In] ref MSG pMsg);
        [PreserveSig]
        int OnAmbientPropertyChange(int dispID);
        [PreserveSig]
        int FreezeEvents(int bFreeze);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("79eac9c9-baf9-11ce-8c82-00aa004ba90b")]
    public interface IPersistMoniker
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetClassID([Out] out Guid pClassID);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetCurMoniker([Out, MarshalAs(UnmanagedType.Interface)] out System.Runtime.InteropServices.ComTypes.IMoniker ppimkName);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsDirty();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Load([In] int bFullyAvailable, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IMoniker pimkName, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IBindCtx pibc, [In] int grfMode);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Save([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IMoniker pimkName, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IBindCtx pibc, [In] bool fRemember);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SaveCompleted([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IMoniker pimkName, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IBindCtx pibc);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07")]
    public interface IPropertyNotifySink
    {
        void OnChanged(int dispID);
        [PreserveSig]
        int OnRequestEdit(int dispID);
    }

    [ComImport, Guid("C4D244B0-D43E-11CF-893B-00AA00BDCE1A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDocHostShowUI
    {
        [PreserveSig]
        uint ShowMessage(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string lpstrText, [MarshalAs(UnmanagedType.LPWStr)] string lpstrCaption, uint dwType, [MarshalAs(UnmanagedType.LPWStr)] string lpstrHelpFile, uint dwHelpContext, out int lpResult);
    }

    #endregion
}