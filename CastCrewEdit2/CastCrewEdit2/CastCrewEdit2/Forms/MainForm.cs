using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Forms
{
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
        int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] Object pbc, [In, MarshalAs(UnmanagedType.LPWStr)] String pszDisplayName, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten, [Out, MarshalAs(UnmanagedType.LPArray)] Object[] ppmkOut);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumObjects([In, MarshalAs(UnmanagedType.U4)] uint grfFlags, [Out, MarshalAs(UnmanagedType.LPArray)] Object[] ppenum);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int LockContainer([In, MarshalAs(UnmanagedType.Bool)] Boolean fLock);
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

    [ComVisible(true), ComImport(), Guid("00000112-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleObject
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetClientSite([In, MarshalAs(UnmanagedType.Interface)]IOleClientSite
        pClientSite);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetClientSite([Out, MarshalAs(UnmanagedType.Interface)]out IOleClientSite site);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] String szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)]String szContainerObj);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Close([In, MarshalAs(UnmanagedType.U4)] uint dwSaveOption);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] Object pmk);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwAssign, [In, MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker, [Out, MarshalAs(UnmanagedType.Interface)] out Object moniker);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int InitFromData([In, MarshalAs(UnmanagedType.Interface)]Object pDataObject, [In, MarshalAs(UnmanagedType.Bool)] Boolean fCreation, [In, MarshalAs(UnmanagedType.U4)] uint dwReserved);
        int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] uint dwReserved, out Object data);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DoVerb([In, MarshalAs(UnmanagedType.I4)] int iVerb, [In]IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite, [In, MarshalAs(UnmanagedType.I4)] int lindex, [In] IntPtr hwndParent, [In] RECT lprcPosRect);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumVerbs(out Object e); // IEnumOLEVERB
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
        int GetUserType([In, MarshalAs(UnmanagedType.U4)] uint dwFormOfType, [Out, MarshalAs(UnmanagedType.LPWStr)] out String userType);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetExtent([In, MarshalAs(UnmanagedType.U4)] uint dwDrawAspect, [In] Object pSizel); // tagSIZEL
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetExtent([In, MarshalAs(UnmanagedType.U4)] uint dwDrawAspect, [Out] Object pSizel); // tagSIZEL
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Advise([In, MarshalAs(UnmanagedType.Interface)]System.Runtime.InteropServices.ComTypes.IAdviseSink pAdvSink, out int cookie);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumAdvise(out Object e);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] uint dwAspect, out int misc);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetColorScheme([In] Object pLogpal); // tagLOGPALETTE
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
        Int32 GetClassID([Out] out Guid pClassID);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        Int32 GetCurMoniker([Out, MarshalAs(UnmanagedType.Interface)] out System.Runtime.InteropServices.ComTypes.IMoniker ppimkName);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        Int32 IsDirty();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        Int32 Load([In] int bFullyAvailable, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IMoniker pimkName, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IBindCtx pibc, [In] int grfMode);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        Int32 Save([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IMoniker pimkName, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IBindCtx pibc, [In] bool fRemember);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        Int32 SaveCompleted([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IMoniker pimkName, [In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IBindCtx pibc);
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
    #endregion COM Interfaces

    [ComVisible(true)]
    public partial class MainForm : CastCrewEdit2ParseBaseForm, IOleClientSite, IDocHostShowUI
    {
        private String MovieTitle;
        private Boolean SkipVersionCheck;
        private static readonly Regex SeasonsBeginRegex;
        private static readonly Regex SeasonsEndRegex;
        private static readonly Regex SeasonRegex;
        private static readonly Regex EpisodeStartRegex;
        private static readonly Regex EpisodeLinkRegex;
        private static readonly Regex EpisodeNumberRegex;
        private static readonly Regex EpisodeNameRegex;
        private static readonly Regex EpisodeEndRegex;
        private String MovieTitleLink;
        private List<Match> CastMatches;
        private List<KeyValuePair<Match, List<Match>>> CrewMatches;
        private Dictionary<String, List<Match>> SoundtrackMatches;
        private List<CastInfo> CastList;
        private List<CrewInfo> CrewList;

        [Flags()]
        private enum EpisodeParts
        {
            None = 0,
            Link = 1,
            Name = 2,
            Number = 4
        }

        static MainForm()
        {
            SeasonsBeginRegex = new Regex("<label for=\"bySeason\">Season:</label>", RegexOptions.Compiled);
            SeasonsEndRegex = new Regex("</select>", RegexOptions.Compiled);
            SeasonRegex = new Regex("<option( +)(selected=\"selected\")?( +)value=\"(?'SeasonNumber'[0-9]+)\"", RegexOptions.Compiled);
            //SeasonRegex = new Regex("<option value=\"1\"", RegexOptions.Compiled);
            EpisodeStartRegex = new Regex("<div class=\"list_item (even|odd)\">", RegexOptions.Compiled);
            EpisodeLinkRegex = new Regex("href=\"/title/(?'EpisodeLink'[a-z0-9]+)/", RegexOptions.Compiled);
            EpisodeNumberRegex = new Regex("itemprop=\"episodeNumber\" content=\"(?'EpisodeNumber'[0-9]+)\"", RegexOptions.Compiled);
            EpisodeNameRegex = new Regex("itemprop=\"name\">(?'EpisodeName'.*?)</a>", RegexOptions.Compiled);
            EpisodeEndRegex = new Regex("<div class=\"clear\">&nbsp;</div>", RegexOptions.Compiled);
        }

        public MainForm(Boolean skipVersionCheck)
        {
            MovieTitle = String.Empty;

            SkipVersionCheck = skipVersionCheck;

            CastMatches = new List<Match>();

            CrewMatches = new List<KeyValuePair<Match, List<Match>>>();

            SoundtrackMatches = new Dictionary<String, List<Match>>();

            InitializeComponent();

            TheProgressBar = ProgressBar;
        }

        [DispId(-5512)]
        public virtual int IDispatch_Invoke_Handler()
        {
            System.Diagnostics.Debug.WriteLine("-5512");
            return (int)(BrowserOptions.Images |
            BrowserOptions.DontRunActiveX | BrowserOptions.NoJava |
            BrowserOptions.NoScripts | BrowserOptions.NoActiveXDownload);
        }

        #region IOleClientSite Members

        public int SaveObject()
        {
            return 0;
        }

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

        public int ShowObject()
        {
            return 0;
        }

        public int OnShowWindow(int fShow)
        {
            return 0;
        }

        public int RequestNewObjectLayout()
        {
            return 0;
        }

        #endregion

        private void OnMovieScanPageButtonClick(Object sender, EventArgs e)
        {
            Boolean failed = false;

            StartLongAction();

            try
            {
                MovieTitle = String.Empty;

                MovieCastDataGridView.Rows.Clear();

                MovieCrewDataGridView.Rows.Clear();

                if (MovieUrlTextBox.Text.Length == 0)
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsEmpty, MessageBoxTexts.UrlIsEmptyHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                Match match = IMDbParser.TitleUrlRegex.Match(MovieUrlTextBox.Text);

                if ((match.Success == false) || (match.Groups["TitleLink"].Success == false))
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsIncorrect, MessageBoxTexts.UrlIsIncorrectHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
                try
                {
                    //String url;

                    MovieTitleLink = match.Groups["TitleLink"].Value.ToString();

                    //url = IMDbParser.TitleUrl + MovieTitleLink + "/fullcredits";
                    ParseIMDb(MovieTitleLink);

                    if ((Program.Settings.DefaultValues.DisableParsingCompleteMessageBox == false)
                        && (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing == false)
                        && (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing == false))
                    {
                        ProcessMessageQueue();

                        MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    failed = true;

                    MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Program.WriteError(ex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Program.WriteError(ex);
            }
            finally
            {
                if ((failed == false) && (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing == false))
                {
                    Program.FlushPersonCache();
                }

                PersonsInLocalCacheLabel.Text = Program.PersonCacheCount.ToString();

                SetMovieFormText();

                EndLongAction();
            }

            if ((failed == false) && (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing))
            {
                GetBirthYears(Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing);
            }

            if ((failed == false) && (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing))
            {
                OnGetHeadshotsButtonClick(sender, e);
            }

            ProcessMessageQueue();

            if (failed == false)
            {
                DataGridViewHelper.CopyCastToClipboard(MovieCastDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
                DataGridViewHelper.CopyCrewToClipboard(MovieCrewDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
            }
        }

        private void SetMovieFormText()
        {
            if (String.IsNullOrEmpty(MovieTitle) == false)
            {
                Text = "Cast/Crew Edit 2 - " + MovieTitle;
            }
            else
            {
                Text = "Cast/Crew Edit 2";
            }
        }

        private void ParseIMDb(String key)
        {
            DefaultValues defaultValues;

            defaultValues = CopyDefaultValues();
            CastList = new List<CastInfo>();
            CrewList = new List<CrewInfo>();
            ParseTitle(key);
            ParseCastAndCrew(defaultValues, key, ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, ParseCrewCheckBox.Checked
                , false, ref CastMatches, ref CastList, ref CrewMatches, ref CrewList, ref SoundtrackMatches);
            try
            {
                Int32 progressMax = CastMatches.Count;

                foreach (KeyValuePair<Match, List<Match>> kvp in CrewMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                foreach (KeyValuePair<String, List<Match>> kvp in SoundtrackMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                StartProgress(progressMax, Color.LightBlue);

                ProcessLines(CastList, CastMatches, CrewList, CrewMatches, SoundtrackMatches, defaultValues);
            }
            finally
            {
                EndProgress();
            }
            UpdateUI();
            ParseTrivia();
            ParseGoofs();
        }

        private void ParseTitle(String key)
        {
            WebResponse webResponse;
            String webSite;
            String line;

            webResponse = null;
            try
            {
                webResponse = IMDbParser.GetWebResponse(IMDbParser.TitleUrl + key + "/fullcredits");
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream, IMDbParser.Encoding))
                    {
                        webSite = sr.ReadToEnd();
                    }
                }
            }
            finally
            {
                try
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                }
                catch
                {
                }
            }
            #region Parse for Title
            using (StringReader sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine();

                    if (String.IsNullOrEmpty(MovieTitle))
                    {
                        Match titleMatch;

                        titleMatch = IMDbParser.TitleRegex.Match(line);
                        if (titleMatch.Success)
                        {
                            MovieTitle = HttpUtility.HtmlDecode(titleMatch.Groups["Title"].Value);

                            MovieTitle = MovieTitle.Replace(" - IMDb", String.Empty).Replace(" - Full Cast & Crew", String.Empty).Trim();

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
            WebResponse webResponse;

            TriviaTextBox.Text = String.Empty;
            webResponse = null;
            try
            {
                if (Program.Settings.DefaultValues.DownloadTrivia)
                {
                    String triviaUrl;

                    triviaUrl = IMDbParser.TitleUrl + MovieTitleLink + "/trivia";
                    webResponse = IMDbParser.GetWebResponse(triviaUrl);
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream, IMDbParser.Encoding))
                        {
                            StringBuilder trivia;
                            Boolean triviaFound;

                            triviaFound = false;
                            trivia = new StringBuilder();
                            while (sr.EndOfStream == false)
                            {
                                String line;
                                Match beginMatch;
                                //Match endMatch;

                                line = sr.ReadLine();
                                if (triviaFound == false)
                                {
                                    beginMatch = IMDbParser.TriviaStartRegex.Match(line);
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
            }
            finally
            {
                try
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                }
                catch
                {
                }
            }
        }

        private void ParseTrivia(StringBuilder trivia, String triviaUrl)
        {
            MatchCollection matches;

            matches = IMDbParser.TriviaLiRegex.Matches(trivia.ToString());
            trivia = new StringBuilder();
            trivia.AppendLine("<div style=\"display:none\">");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups["Trivia"].Success)
                    {
                        String value;

                        value = match.Groups["Trivia"].Value.Trim();
                        if (String.IsNullOrEmpty(value) == false)
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
            WebResponse webResponse;

            GoofsTextBox.Text = String.Empty;
            webResponse = null;
            try
            {
                if (Program.Settings.DefaultValues.DownloadGoofs)
                {
                    String goofsUrl;

                    goofsUrl = IMDbParser.TitleUrl + MovieTitleLink + "/goofs";
                    webResponse = IMDbParser.GetWebResponse(goofsUrl);
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream, IMDbParser.Encoding))
                        {
                            StringBuilder goofs;
                            Boolean goofsFound;

                            goofsFound = false;
                            goofs = new StringBuilder();
                            while (sr.EndOfStream == false)
                            {
                                String line;
                                Match beginMatch;
                                //Match endMatch;

                                line = sr.ReadLine();
                                if (goofsFound == false)
                                {
                                    beginMatch = IMDbParser.GoofsStartRegex.Match(line);
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
            }
            finally
            {
                try
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                }
                catch
                {
                }
            }
        }

        private void ParseGoofs(StringBuilder goofs, String goofsUrl)
        {
            MatchCollection matches;

            matches = IMDbParser.GoofsLiRegex.Matches(goofs.ToString());
            goofs = new StringBuilder();
            goofs.AppendLine("<div style=\"display:none\">");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (match.Groups["Goof"].Success)
                    {
                        String value;
                        Match spoilerMatch;

                        value = match.Groups["Goof"].Value.Trim();
                        spoilerMatch = IMDbParser.GoofSpoilerRegex.Match(value);
                        if (spoilerMatch.Success)
                        {
                            value = spoilerMatch.Groups["Goof"].Value;
                        }
                        if (String.IsNullOrEmpty(value) == false)
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
                CastInfo title;

                title = new CastInfo(-1);
                title.FirstName = FirstNames.Title;
                title.MiddleName = String.Empty;
                title.LastName = MovieTitle;
                title.BirthYear = String.Empty;
                title.Role = String.Empty;
                title.Voice = "False";
                title.Uncredited = "False";
                title.CreditedAs = String.Empty;
                title.PersonLink = MovieTitleLink;
                CastList.Add(title);
            }
            if (ParseCrewCheckBox.Checked)
            {
                CrewInfo title;

                title = new CrewInfo();
                title.FirstName = FirstNames.Title;
                title.MiddleName = String.Empty;
                title.LastName = MovieTitle;
                title.BirthYear = String.Empty;
                title.CreditType = null;
                title.CreditSubtype = null;
                title.CreditedAs = String.Empty;
                title.CustomRole = String.Empty;
                title.PersonLink = MovieTitleLink;
                CrewList.Add(title);
            }
        }

        private void UpdateUI()
        {
            UpdateUI(CastList, CrewList, MovieCastDataGridView, MovieCrewDataGridView
                , ParseCastCheckBox.Checked, ParseCrewCheckBox.Checked, MovieTitleLink, MovieTitle);
            if (Log.Length > 0)
            {
                Log.Show(LogWebBrowser);
            }
        }

        private void OnMainFormLoad(Object sender, EventArgs e)
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
            PersonsInLocalCacheLabel.Text = Program.PersonCacheCount.ToString();
            RegisterEvents();
            if (Program.Settings.CurrentVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                OpenReadme();
                Program.Settings.CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            CheckForNewVersion(true);
            //WebBrowser.Navigate(BrowserUrlComboBox.Text);
            BrowserSearchTextBox.Focus();
        }

        private void CheckForNewVersion(Boolean silently)
        {
            if (silently)
            {
                if (SkipVersionCheck == false)
                {
                    OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "CastCrewEdit2"
                        , GetType().Assembly, silently);
                }
            }
            else
            {
                OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "CastCrewEdit2"
                        , GetType().Assembly, silently);
            }
        }

        private void CreateDataGridViewColumns()
        {
            DataGridViewTextBoxColumn seasonDataGridViewTextBoxColumn;

            DataGridViewHelper.CreateCastColumns(MovieCastDataGridView);
            DataGridViewHelper.CreateCrewColumns(MovieCrewDataGridView);
            seasonDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            seasonDataGridViewTextBoxColumn.Name = "Season Number";
            seasonDataGridViewTextBoxColumn.HeaderText = DataGridViewTexts.SeasonNumber;
            seasonDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            seasonDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
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
            IMDbToDVDProfilerTransformationDataToolStripMenuItem.Click
                += OnIMDbToDVDProfilerTransformationDataToolStripMenuItemClick;
            ReadmeToolStripMenuItem.Click += OnReadmeToolStripMenuItemClick;
            AboutToolStripMenuItem.Click += OnAboutToolStripMenuItemClick;
            BirthYearsInLocalCacheLabel.LinkClicked += OnBirthYearsInLocalCacheLabelLinkClicked;
            PersonsInLocalCacheLabel.LinkClicked += OnPersonsInLocalCacheLabelLinkClicked;
        }

        private void OnMainFormClosing(Object sender, FormClosingEventArgs e)
        {
            if ((Program.Settings.DefaultValues.SaveLogFile) && (Log.Length > 0))
            {
                using (StreamWriter sw = new StreamWriter(Program.LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine(Log.ToString());
                }
            }
            Program.Settings.MainForm.Left = Left;
            Program.Settings.MainForm.Top = Top;
            Program.Settings.MainForm.Width = Width;
            Program.Settings.MainForm.Height = Height;
            Program.Settings.MainForm.WindowState = WindowState;
            Program.Settings.MainForm.RestoreBounds = RestoreBounds;
        }

        private void OnGetBirthYearsButtonClick(Object sender
            , EventArgs e)
        {
            GetBirthYears(false);

            ProcessMessageQueue();
        }

        private void GetBirthYears(Boolean parseHeadshotsFollows)
        {
            GetBirthYears(parseHeadshotsFollows, MovieCastDataGridView, MovieCrewDataGridView
                , BirthYearsInLocalCacheLabel, GetBirthYearsButton, LogWebBrowser);
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

        private void OnMovieCrewDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewHelper.OnCrewDataGridViewCellValueChanged(sender, e);
        }

        private void OnMovieCastDataGridViewCellValueChanged(Object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewHelper.OnCastDataGridViewCellValueChanged(sender, e);
        }

        private void OnMovieCastGenerateButtonClick(Object sender
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

            DataGridViewHelper.CopyCastToClipboard(MovieCastDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, false);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CastDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnMovieCrewGenerateButtonClick(Object sender
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

            DataGridViewHelper.CopyCrewToClipboard(MovieCrewDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, false);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CrewDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnTVShowScanPageButtonClick(Object sender, EventArgs e)
        {
            StartLongAction();

            try
            {
                Match match;

                TVShowTitle = String.Empty;
                TVShowSeasonsDataGridView.Rows.Clear();
                if (TVShowUrlTextBox.Text.Length == 0)
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsEmpty, MessageBoxTexts.UrlIsEmptyHeader, MessageBoxButtons.OK
                        , MessageBoxIcon.Warning);
                    return;
                }
                match = IMDbParser.TitleUrlRegex.Match(TVShowUrlTextBox.Text);
                if ((match.Success == false) || (match.Groups["TitleLink"].Success == false))
                {
                    MessageBox.Show(this, MessageBoxTexts.UrlIsIncorrect, MessageBoxTexts.UrlIsIncorrectHeader, MessageBoxButtons.OK
                        , MessageBoxIcon.Warning);
                    return;
                }
                TVShowTitleLink = match.Groups["TitleLink"].Value.ToString();
                try
                {
                    ScanForSeasons();
                    if (Program.Settings.DefaultValues.DisableParsingCompleteMessageBox == false)
                    {
                        ProcessMessageQueue();
                        MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Program.WriteError(ex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Program.WriteError(ex);
            }
            finally
            {
                SetTVShowFormText();

                EndLongAction();
            }
        }

        private void SetTVShowFormText()
        {
            if (String.IsNullOrEmpty(TVShowTitle) == false)
            {
                Text = "Cast/Crew Edit 2 For TV Shows - " + Resources.Resources.Seasons + " - " + TVShowTitle;
            }
            else
            {
                Text = "Cast/Crew Edit 2 For TV Shows";
            }
        }

        private void OnScanForEpisodesButtonClick(Object sender, EventArgs e)
        {
            if ((TVShowSeasonsDataGridView.SelectedRows == null) || (TVShowSeasonsDataGridView.SelectedRows.Count == 0))
            {
                MessageBox.Show(this, MessageBoxTexts.NoSeasonSelected, MessageBoxTexts.NoSeasonSelectedHeader, MessageBoxButtons.OK
                    , MessageBoxIcon.Warning);
                return;
            }
            else
            {
                WebResponse webResponse;
                List<EpisodeInfo> episodes;

                StartLongAction();

                SuspendLayout();
                webResponse = null;
                episodes = new List<EpisodeInfo>();
                try
                {
                    List<Int32> seasons;

                    seasons = new List<Int32>(TVShowSeasonsDataGridView.SelectedRows.Count);
                    for (Int32 i = 0; i < TVShowSeasonsDataGridView.SelectedRows.Count; i++)
                    {
                        seasons.Add(Int32.Parse(TVShowSeasonsDataGridView.SelectedRows[i].Cells["Season Number"].Value.ToString()));
                    }
                    seasons.Sort();
                    //seasons.Sort(new Comparison<String>(delegate(String left, String right)
                    //    {
                    //        Int32 intLeft;
                    //        Int32 intRight;

                    //        intLeft = Int32.Parse(left);
                    //        intRight = Int32.Parse(right);
                    //        return (intLeft.CompareTo(intRight));
                    //    }));
                    foreach (Int32 season in seasons)
                    {
                        String targetUrl;

                        targetUrl = IMDbParser.TitleUrl + TVShowTitleLink + "/episodes?season=" + season;
                        webResponse = IMDbParser.GetWebResponse(targetUrl);
                        using (Stream stream = webResponse.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(stream, IMDbParser.Encoding))
                            {
                                while (sr.EndOfStream == false)
                                {
                                    Match episodeStartMatch;
                                    String line;

                                    line = sr.ReadLine();
                                    episodeStartMatch = EpisodeStartRegex.Match(line);
                                    if (episodeStartMatch.Success)
                                    {
                                        EpisodeInfo episodeInfo;
                                        EpisodeParts parts;
                                        Boolean episodeLinkFound;
                                        Boolean episodeNumberFound;
                                        Boolean episodeNameFound;

                                        episodeLinkFound = false;
                                        episodeNumberFound = false;
                                        episodeNameFound = false;
                                        episodeInfo = new EpisodeInfo();
                                        parts = EpisodeParts.None;
                                        while (EpisodeEndRegex.Match(line).Success == false)
                                        {
                                            Match match;

                                            line = sr.ReadLine();
                                            if (episodeLinkFound == false)
                                            {
                                                match = EpisodeLinkRegex.Match(line);
                                                if (match.Success)
                                                {
                                                    episodeInfo.Link = match.Groups["EpisodeLink"].Value.ToString();
                                                    parts |= EpisodeParts.Link;
                                                    episodeLinkFound = true;
                                                    continue;
                                                }
                                            }
                                            if (episodeNumberFound == false)
                                            {
                                                match = EpisodeNumberRegex.Match(line);
                                                if (match.Success)
                                                {
                                                    episodeInfo.EpisodeNumber = match.Groups["EpisodeNumber"].Value.ToString();
                                                    parts |= EpisodeParts.Number;
                                                    episodeNumberFound = true;
                                                    continue;
                                                }
                                            }
                                            if (episodeNameFound == false)
                                            {
                                                match = EpisodeNameRegex.Match(line);
                                                if (match.Success)
                                                {
                                                    episodeInfo.EpisodeName = HttpUtility.HtmlDecode(match.Groups["EpisodeName"].Value.ToString());
                                                    parts |= EpisodeParts.Name;
                                                    episodeNameFound = true;
                                                    continue;
                                                }
                                            }
                                        }
                                        episodeInfo.SeasonNumber = season.ToString();
                                        if (parts == (EpisodeParts.Link | EpisodeParts.Name | EpisodeParts.Number))
                                        {
                                            episodes.Add(episodeInfo);
                                        }
                                    }
                                }
                            }
                        }
                        webResponse.Close();
                        webResponse = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Program.WriteError(ex);
                }
                finally
                {
                    try
                    {
                        if (webResponse != null)
                        {
                            webResponse.Close();
                        }
                    }
                    catch
                    { }

                    ResumeLayout();

                    EndLongAction();
                }

                if (episodes.Count == 0)
                {
                    MessageBox.Show(this, MessageBoxTexts.NoEpisodesFound, MessageBoxTexts.NoEpisodesFoundHeader, MessageBoxButtons.OK
                        , MessageBoxIcon.Warning);

                    return;
                }
                else
                {
                    if (Program.Settings.DefaultValues.DisableParsingCompleteMessageBox == false)
                    {
                        ProcessMessageQueue();
                        MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
                    }
                    using (EpisodesForm episodesForm = new EpisodesForm(episodes))
                    {
                        SettingsHaveChanged = false;
                        episodesForm.ShowDialog(this);
                        if (SettingsHaveChanged)
                        {
                            SetCheckBoxes();
                            SettingsHaveChanged = false;
                        }
                        Log.Show(LogWebBrowser);
                    }
                }
                BirthYearsInLocalCacheLabel.Text = IMDbParser.PersonHashCount;
                PersonsInLocalCacheLabel.Text = Program.PersonCacheCount.ToString();
            }
        }

        private void ScanForSeasons()
        {
            WebResponse webResponse;

            webResponse = null;
            try
            {
                webResponse = IMDbParser.GetWebResponse(IMDbParser.TitleUrl + TVShowTitleLink + "/episodes");
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream, IMDbParser.Encoding))
                    {
                        while (sr.EndOfStream == false)
                        {
                            String line;
                            Match seasonsMatch;

                            line = sr.ReadLine();
                            if (String.IsNullOrEmpty(TVShowTitle))
                            {
                                Match titleMatch;

                                titleMatch = IMDbParser.TitleRegex.Match(line);
                                if (titleMatch.Success)
                                {
                                    TVShowTitle = HttpUtility.HtmlDecode(titleMatch.Groups["Title"].Value);

                                    TVShowTitle = TVShowTitle.Replace(" - IMDb", String.Empty).Replace(" - Episodes", String.Empty).Trim();

                                    continue;
                                }
                            }
                            seasonsMatch = SeasonsBeginRegex.Match(line);
                            if (seasonsMatch.Success)
                            {
                                while (sr.EndOfStream == false)
                                {
                                    Match seasonMatch;

                                    line = sr.ReadLine();
                                    seasonsMatch = SeasonsEndRegex.Match(line);
                                    if (seasonsMatch.Success)
                                    {
                                        return;
                                    }
                                    seasonMatch = SeasonRegex.Match(line);
                                    if (seasonMatch.Success)
                                    {
                                        if (seasonMatch.Success)
                                        {
                                            DataGridViewRow row;

                                            row = TVShowSeasonsDataGridView.Rows[TVShowSeasonsDataGridView.Rows.Add()];
                                            row.DefaultCellStyle.BackColor = Color.White;
                                            row.Cells["Season Number"].Value = seasonMatch.Groups["SeasonNumber"].Value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                }
                catch
                {
                }
            }
        }

        private void OnMovieTVShowTabControlSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (((TabControl)sender).SelectedTab == MovieTab)
            {
                SetMovieFormText();
            }
            else if (((TabControl)sender).SelectedTab == TVShowTab)
            {
                SetTVShowFormText();
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
                }
                settingsForm.GetValues(out Program.Settings.SettingsForm.Left, out Program.Settings.SettingsForm.Top);
            }
        }

        private void OnReApplySettingsAndFiltersButtonClick(Object sender
            , EventArgs e)
        {
            StartLongAction();

            MovieCastDataGridView.Rows.Clear();

            MovieCrewDataGridView.Rows.Clear();

            CastList = new List<CastInfo>();

            CrewList = new List<CrewInfo>();

            CreateTitleRow();

            DefaultValues defaultValues = CopyDefaultValues();

            try
            {
                Int32 progressMax = CastMatches.Count;

                foreach (KeyValuePair<Match, List<Match>> kvp in CrewMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                foreach (KeyValuePair<String, List<Match>> kvp in SoundtrackMatches)
                {
                    progressMax += kvp.Value.Count;
                }

                StartProgress(progressMax, Color.LightBlue);

                ProcessLines(CastList, CastMatches, CrewList, CrewMatches, SoundtrackMatches, defaultValues);
            }
            finally
            {
                EndProgress();
            }

            UpdateUI();

            EndLongAction();

            if ((Program.Settings.DefaultValues.DisableParsingCompleteMessageBox == false)
                && (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing == false)
                && (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing == false))
            {
                ProcessMessageQueue();

                MessageBox.Show(this, MessageBoxTexts.ParsingComplete, MessageBoxTexts.ParsingComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Program.Settings.DefaultValues.GetBirthYearsDirectlyAfterNameParsing)
            {
                GetBirthYears(Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing);
            }
            else
            {
                Program.FlushPersonCache();
            }

            if (Program.Settings.DefaultValues.GetHeadShotsDirectlyAfterNameParsing)
            {
                OnGetHeadshotsButtonClick(sender, e);
            }

            ProcessMessageQueue();

            DataGridViewHelper.CopyCastToClipboard(MovieCastDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
            DataGridViewHelper.CopyCrewToClipboard(MovieCrewDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage, true);
        }

        private DefaultValues CopyDefaultValues()
        {
            DefaultValues defaultValues;

            defaultValues = new DefaultValues();
            defaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName
                = Program.Settings.DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName;
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

        protected override void MoveRow(CastInfo castMember, Boolean up)
        {
            Int32 index;

            index = FindIndexOfCastMember(CastList, castMember);
            if (index != -1)
            {
                CastInfo temp;

                temp = CastList[index];
                if (up)
                {
                    CastList[index] = CastList[index - 1];
                    CastList[index - 1] = temp;
                }
                else
                {
                    CastList[index] = CastList[index + 1];
                    CastList[index + 1] = temp;
                }
                MovieCastDataGridView.Rows.Clear();
                UpdateUI(CastList, null, MovieCastDataGridView, null, true, false, MovieTitleLink, MovieTitle);
            }
            else
            {
                Debug.Assert(false, "Invalid Index");
            }
        }

        protected override void RemoveRow(CastInfo castMember)
        {
            Int32 index;

            index = FindIndexOfCastMember(CastList, castMember);
            if (index != -1)
            {
                CastList.RemoveAt(index);
                MovieCastDataGridView.Rows.Clear();
                UpdateUI(CastList, null, MovieCastDataGridView, null, true, false, MovieTitleLink, MovieTitle);
            }
            else
            {
                Debug.Assert(false, "Invalid Index");
            }
        }

        private void OnGetHeadshotsButtonClick(Object sender
            , EventArgs e)
        {
            GetHeadshots(MovieCastDataGridView, MovieCrewDataGridView, GetHeadshotsButton);
        }

        private void OnBrowseButtonClick(Object sender, EventArgs e)
        {
            WebBrowser.Navigate(BrowserUrlComboBox.Text);
        }

        private void OnWebBrowserNavigated(Object sender, WebBrowserNavigatedEventArgs e)
        {
            if (WebBrowser.Url != null)
            {
                BrowserUrlComboBox.Text = WebBrowser.Url.ToString();
                MovieUrlTextBox.Text = BrowserUrlComboBox.Text;
                TVShowUrlTextBox.Text = BrowserUrlComboBox.Text;
            }
        }

        private void OnBrowserUrlComboBoxSelectedIndexChanged(Object sender, EventArgs e)
        {
            if ((WebBrowser.Url != null) && (BrowserUrlComboBox.Text != WebBrowser.Url.ToString()))
            {
                OnBrowseButtonClick(sender, e);
            }
        }

        private void OnBrowserSearchButtonClick(Object sender, EventArgs e)
        {
            WebBrowser.Navigate("http://akas.imdb.com/find?s=tt&q="
                + System.Web.HttpUtility.UrlEncode(BrowserSearchTextBox.Text));
        }

        private void OnBrowserUrlComboBoxKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnBrowseButtonClick(this, null);
            }
        }

        private void OnBrowserSearchTextBoxKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnBrowserSearchButtonClick(this, null);
            }
        }

        private void OnCopyTriviaToClipboardButtonClick(Object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(TriviaTextBox.Text, true, 4, 250);
            }
            catch
            { }
        }

        private void OnWebBrowserNavigating(Object sender, WebBrowserNavigatingEventArgs e)
        {
            IOleObject obj;

            obj = (IOleObject)(WebBrowser.ActiveXInstance);
            obj.SetClientSite(this);
        }

        #region IDocHostShowUI Members

        //public void ShowMessage(int hwnd, ref int lpstrText, ref int lpstrCaption, uint dwType, ref int lpstrHelpFile, uint dwHelpContext, out int lpResult)
        //{
        //    lpResult = -1;
        //}

        #endregion

        #region IDocHostShowUI Members

        public uint ShowMessage(IntPtr hwnd, string lpstrText, string lpstrCaption, uint dwType, string lpstrHelpFile, uint dwHelpContext, out int lpResult)
        {
            if ((String.IsNullOrEmpty(lpstrText) == false) && (lpstrText.Contains("ActiveX")))
            {
                lpResult = 0;
            }
            else
            {
                MessageBox.Show(lpstrText, lpstrCaption);
                lpResult = 0;
            }
            return (0);
        }
        #endregion

        private void OnBackupToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "bin";
                sfd.Filter = "Binary Files|*.bin";
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.FileName = "cce2.bin";
                sfd.Title = "Select Session Data backup file";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SessionData.Serialize(sfd.FileName);
                }
            }
        }

        private void OnRestoreToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.DefaultExt = "bin";
                ofd.Filter = "Binary Files|*.bin";
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;
                ofd.Title = "Select Session Data backup file";
                ofd.FileName = "cce2.bin";
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BirthYearsInLocalCacheLabel.Text = SessionData.Deserialize(ofd.FileName);
                }
            }
        }

        private void OnCopyGoofsToClipboardButtonClick(Object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(GoofsTextBox.Text, true, 4, 250);
            }
            catch
            { }
        }

        private void OnCheckForUpdateToolStripMenuItemClick(Object sender, EventArgs e)
        {
            CheckForNewVersion(false);
        }

        private void OnLogWebBrowserNavigating(Object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.AbsoluteUri.StartsWith("http://akas.imdb.com/"))
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

            HasAgreed = true;

            DataGridViewHelper.CopyExtendedCastToClipboard(MovieCastDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage);

            Log.Show(LogWebBrowser);

            ProcessMessageQueue();

            if (Program.Settings.DefaultValues.DisableCopyingSuccessfulMessageBox == false)
            {
                MessageBox.Show(this, MessageBoxTexts.CastDataCopySuccessful, MessageBoxTexts.DataCopySuccessfulHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnCopyExtendedCrewToClipboardToolStripMenuItemClick(Object sender
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

            DataGridViewHelper.CopyExtendedCrewToClipboard(MovieCrewDataGridView, MovieTitle, Log, Program.Settings.DefaultValues.UseFakeBirthYears, AddMessage);

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