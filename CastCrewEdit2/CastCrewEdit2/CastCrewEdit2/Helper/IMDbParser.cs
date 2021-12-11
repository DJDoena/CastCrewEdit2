namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using System.Windows.Forms;
    using DVDProfilerHelper;
    using Resources;

    public delegate void SetProgress();

    internal static class IMDbParser
    {
        public const int MaxTasks = 4;

        public const string BaseUrl = @"https://www.imdb.com";

        public const string PersonUrl = BaseUrl + @"/name/";

        public const string TitleUrl = BaseUrl + @"/title/";

        public static Regex TitleUrlRegex { get; }

        public static Regex TitleRegex { get; }

        public static Regex TriviaStartRegex { get; }

        public static Regex TriviaLiRegex { get; }

        public static Regex GoofsStartRegex { get; }

        public static Regex GoofsLiRegex { get; }

        public static Regex GoofSpoilerRegex { get; }

#if UnitTest

        public static Encoding Encoding { get; }

#else

        private static Encoding Encoding { get; }

#endif

#if UnitTest

        public static Dictionary<string, WebResponse> WebResponses { get; }

#endif

        internal static Regex UncreditedRegex { get; }

        internal static Regex CreditedAsRegex { get; }

        internal static Regex SoundtrackStartRegex { get; }

        internal static Dictionary<PersonInfo, string> BirthYearCache { get; set; }

        internal static Dictionary<PersonInfo, FileInfo> HeadshotCache { get; set; }

        internal static Dictionary<string, ushort> ForcedFakeBirthYears { get; private set; }

        internal static List<string> IgnoreCustomInIMDbCreditType => _ignoreCustomInIMDbCreditType;

        internal static List<string> IgnoreIMDbCreditTypeInOther => _ignoreIMDbCreditTypeInOther;

        internal static IMDbToDVDProfilerCrewRoleTransformation TransformationData { get; private set; }

        private static readonly Regex _castRegex;

        private static readonly Regex _creditTypeRegex;

        private static readonly Regex _crewRegex;

        private static readonly Regex _photoRegex;

        private static readonly Regex _photoUrlRegex;

        private static readonly Regex _soundtrackLiRegex;

        private static readonly Regex _soundtrackTitleRegex;

        private static readonly Regex _soundtrackPersonRegex;

        private static readonly Regex _personUrlRegex;

        private static readonly object _updatedPersonLock;

        private static readonly object _getBirthYearLock;

        private static readonly object _getWebsiteLock;

        private static List<string> _ignoreCustomInIMDbCreditType;

        private static List<string> _ignoreIMDbCreditTypeInOther;

        private static readonly Dictionary<string, string> UpdatedPersonLinks;

        private static bool IsInitialized;

        private static readonly Dictionary<string, string> WebSites;

        static IMDbParser()
        {
            const string DomainPrefix = "https?://((akas.)*|(www.)*|(us.)*|(german.)*)imdb.(com|de)/";

            _updatedPersonLock = new object();

            _getBirthYearLock = new object();

            _getWebsiteLock = new object();

            Encoding = Encoding.GetEncoding("UTF-8");

            TitleUrlRegex = new Regex(DomainPrefix + "title/(?'TitleLink'tt[0-9]+)/.*$", RegexOptions.Compiled);

            TitleRegex = new Regex(@"<title>(?'Title'.+?)</title>", RegexOptions.Compiled);

            UncreditedRegex = new Regex(@"\(?uncredited\)?", RegexOptions.Compiled);

            CreditedAsRegex = new Regex(@"\(as (?'CreditedAs'.+?)\)", RegexOptions.Compiled);

            _castRegex = new Regex("<td class=\"primary_photo\">.*?<td><a href=\"/name/(?'PersonLink'[a-z0-9]+)/.*?>(?'PersonName'.+?)</a>          </td><td class=\"ellipsis\">(\\.\\.\\.)?</td><td class=\"character\">(<div>)?(?'Role'.*?)(</div>)?</td>", RegexOptions.Compiled);

            _creditTypeRegex = new Regex("<h4 (.*?)class=\"dataHeaderWithBorder\"(.*?)>(?'CreditType'.+?)(<span.+?)?(&nbsp;)?</h4>", RegexOptions.Compiled | RegexOptions.Multiline);

            _crewRegex = new Regex("<td (.*?)class=\"name\"(.*?)><a (.*?)href=\"/name/(?'PersonLink'[a-z0-9]+)/(.*?)>(?'PersonName'.+?)</a>.*?</td>.*?(\\.\\.\\.)?.*?((<td colspan=\"(2|3)\">)|(<td (.*?)class=\"credit\"(.*?)>))(?'Credit'.*?)</td>", RegexOptions.Compiled | RegexOptions.Multiline);

            _photoRegex = new Regex("<td.+?id=\"img_primary\".*?>", RegexOptions.Compiled);

            _photoUrlRegex = new Regex("<img (.*?)src=\"(?'PhotoUrl'.+?)\"", RegexOptions.Compiled);

            TriviaStartRegex = new Regex("class=\"soda (even|odd) sodavote\".*?>", RegexOptions.Compiled);

            TriviaLiRegex = new Regex("<div class=\"sodatext\">(?'Trivia'.+?)</div>", RegexOptions.Singleline | RegexOptions.Compiled);

            GoofsStartRegex = new Regex("<h4 class=\"li_group\">", RegexOptions.Compiled);

            GoofsLiRegex = new Regex("<div class=\"sodatext\">(?'Goof'.+?)</div>", RegexOptions.Singleline);

            GoofSpoilerRegex = new Regex("<h4 class=\"inline\">.+?</h4>(?'Goof'.*)", RegexOptions.Singleline | RegexOptions.Compiled);

            SoundtrackStartRegex = new Regex("<div id=\"soundtracks_content\"", RegexOptions.Compiled);

            _soundtrackLiRegex = new Regex("<div id=\"(?'Soundtrack'.+?)</div>", RegexOptions.Singleline | RegexOptions.Compiled);

            _soundtrackTitleRegex = new Regex("\"(?'Title'.+?)\"", RegexOptions.Compiled);

            _soundtrackPersonRegex = new Regex("<a href=\"/name/(?'PersonLink'[a-z0-9]+)(.+?)\">(?'PersonName'.+?)</a>", RegexOptions.Compiled);

            BirthYearCache = new Dictionary<PersonInfo, string>(1000);

            HeadshotCache = new Dictionary<PersonInfo, FileInfo>(1000);

            _personUrlRegex = new Regex(DomainPrefix + "name/(?'PersonLink'nm[0-9]+)/.*$", RegexOptions.Compiled);

            UpdatedPersonLinks = new Dictionary<string, string>();

            IsInitialized = false;

            WebSites = new Dictionary<string, string>();

#if UnitTest

            WebResponses = new Dictionary<string, WebResponse>();

#endif

        }

        public static WebResponse GetWebResponse(string targetUrl)
        {

#if UnitTest

            return WebResponses[targetUrl];

#else

            var wr = OnlineAccess.CreateSystemSettingsWebRequest(targetUrl, CultureInfo.GetCultureInfo("en-US"));

            return wr;

#endif

        }

        public static string GetWebSite(string targetUrl)
        {
            lock (_getWebsiteLock)
            {
                if (WebSites.TryGetValue(targetUrl, out var webSite))
                {
                    return webSite;
                }

                using (var webResponse = GetWebResponse(targetUrl))
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream, Encoding))
                        {
                            webSite = sr.ReadToEnd();

                            WebSites.Add(targetUrl, webSite);

                            return webSite;
                        }
                    }
                }
            }
        }

        public static string PersonHashCount => BirthYearCache.Count.ToString();

        public static Dictionary<PersonInfo, string> PersonHash => BirthYearCache;

        public static void Initialize(IWin32Window windowHandle)
        {
            var path = Program.RootPath + @"\Data\";

            NameParser.Initialize(windowHandle, path);

            InitList(path + @"IgnoreCustomInIMDbCategory.txt", ref _ignoreCustomInIMDbCreditType, false);
            InitList(path + @"IgnoreIMDbCategoryInOther.txt", ref _ignoreIMDbCreditTypeInOther, false);

            List<string> temp = null;

            InitList(path + @"ForcedFakeBirthYears.txt", ref temp, false);

            ProcessForcedFakeBirthYears(temp);

            InitIMDbToDVDProfilerCrewRoleTransformation(windowHandle);

            IsInitialized = true;
        }

        private static void ProcessForcedFakeBirthYears(List<string> forcedFakeBirthYears)
        {
            ForcedFakeBirthYears = new Dictionary<string, ushort>(forcedFakeBirthYears.Count);

            foreach (var forcedFakeBirthYear in forcedFakeBirthYears)
            {
                var split = forcedFakeBirthYear.Split(';');

                if (split.Length == 2)
                {
                    if (!NameParser.IsKnownName(split[0]) && !string.IsNullOrEmpty(split[1]))
                    {
                        if (ushort.TryParse(split[1], out ushort birthYear))
                        {
                            ForcedFakeBirthYears.Add(split[0], birthYear);
                        }
                    }
                }
            }
        }

        public static void InitIMDbToDVDProfilerCrewRoleTransformation(IWin32Window windowHandle)
        {
            var path = Program.RootPath + @"\Data\";

            var fileName = path + @"IMDbToDVDProfilerCrewRoleTransformation.xml";

            if (File.Exists(fileName))
            {
                try
                {
                    TransformationData = DVDProfilerSerializer<IMDbToDVDProfilerCrewRoleTransformation>.Deserialize(fileName);
                }
                catch
                {
                    TransformationData = new IMDbToDVDProfilerCrewRoleTransformation()
                    {
                        CreditTypeList = new CreditType[0],
                    };

                    MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileCantBeRead, fileName), MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                TransformationData = new IMDbToDVDProfilerCrewRoleTransformation()
                {
                    CreditTypeList = new CreditType[0],
                };

                MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void InitList(string fileName, FileNameType fileNameType, IWin32Window windowHandle)
        {
            switch (fileNameType)
            {
                case FileNameType.FirstnamePrefixes:
                case FileNameType.LastnamePrefixes:
                case FileNameType.LastnameSuffixes:
                case FileNameType.KnownNames:
                    {
                        NameParser.InitList(fileName, fileNameType, windowHandle);

                        break;
                    }
                case FileNameType.IgnoreCustomInIMDbCreditType:
                    {
                        InitList(fileName, ref _ignoreCustomInIMDbCreditType, windowHandle, false);

                        break;
                    }
                case FileNameType.IgnoreIMDbCreditTypeInOther:
                    {
                        InitList(fileName, ref _ignoreIMDbCreditTypeInOther, windowHandle, false);

                        break;
                    }
                case FileNameType.ForcedFakeBirthYears:
                    {
                        List<string> forcedFakeBirthYears = null;

                        InitList(fileName, ref forcedFakeBirthYears, windowHandle, false);

                        ProcessForcedFakeBirthYears(forcedFakeBirthYears);

                        break;
                    }
            }
        }

        internal static void InitList(string fileName, ref List<string> list, IWin32Window windowHandle, bool toLower)
        {
            if (!InitList(fileName, ref list, toLower))
            {
                MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static bool InitList(string fileName, ref List<string> list, bool toLower)
        {
            list = new List<string>(50);

            if (!File.Exists(fileName))
            {
                return false;
            }

            using (var sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        if (toLower)
                        {
                            list.Add(line.ToLower());
                        }
                        else
                        {
                            list.Add(line);
                        }
                    }
                }
            }

            return true;
        }

        public static FileInfo GetHeadshot(PersonInfo person)
        {
            if (HeadshotCache.ContainsKey(person))
            {
                return HeadshotCache[person];
            }

            var jpg = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + person.PersonLink + ".jpg");

            var gif = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + person.PersonLink + ".gif");

            var png = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + person.PersonLink + ".png");

            var targetUrl = PersonUrl + person.PersonLink;

            string webSite;
            try
            {
                webSite = GetWebSite(targetUrl);
            }
            catch (WebException webEx)
            {
                if (webEx.Message.Contains("404"))
                {
                    return CheckExistingFile(person, jpg, gif, png);
                }
                else
                {
                    throw;
                }
            }

            using (var sr = new StringReader(webSite))
            {
                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();

                    if (_photoRegex.Match(line).Success)
                    {
                        line = string.Empty;

                        while (!line.Contains("</td>"))
                        {
                            line += sr.ReadLine();
                        }

                        var match = _photoUrlRegex.Match(line);

                        if (match.Success)
                        {
                            return GetHeadshot(match, person, jpg, gif, png);
                        }
                    }
                }

                var headshot = CheckExistingFile(person, jpg, gif, png);

                return headshot;
            }
        }

        private static FileInfo GetHeadshot(Match match, PersonInfo person, FileInfo jpg, FileInfo gif, FileInfo png)
        {
            var remoteFile = match.Groups["PhotoUrl"].Value.ToString();

            try
            {
                if (remoteFile.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase))
                {
                    DownloadFile(jpg, remoteFile);
                }
                else if (remoteFile.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                {
                    DownloadFile(gif, remoteFile);
                }
                else if (remoteFile.EndsWith("png", StringComparison.InvariantCultureIgnoreCase))
                {
                    DownloadFile(png, remoteFile);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Message.Contains("404"))
                {
                    return CheckExistingFile(person, jpg, gif, png);
                }
                else
                {
                    throw;
                }
            }

            return CheckExistingFile(person, jpg, gif, png);
        }

        private static void DownloadFile(FileInfo imageFileInfo, string remoteFile)
        {
            using (var webClient = OnlineAccess.CreateSystemSettingsWebClient())
            {
                webClient.DownloadFile(remoteFile, imageFileInfo.FullName);

                imageFileInfo.Refresh();
            }
        }

        private static FileInfo CheckExistingFile(PersonInfo person, FileInfo jpg, FileInfo gif, FileInfo png)
        {
            if (jpg.Exists)
            {
                HeadshotCache[person] = jpg;

                return jpg;
            }
            else if (gif.Exists)
            {
                HeadshotCache[person] = gif;

                return gif;
            }
            else if (png.Exists)
            {
                HeadshotCache[person] = png;

                return png;
            }
            else
            {
                HeadshotCache[person] = null;

                return null;
            }
        }

        public static void GetBirthYear(PersonInfo person)
        {
            lock (_getBirthYearLock)
            {
                if (BirthYearCache.ContainsKey(person))
                {
                    person.BirthYear = BirthYearCache[person];

                    return;
                }
            }

            person.BirthYear = BirthYearGetter.Get(person.PersonLink);

            person.BirthYearWasRetrieved = true;

            lock (_getBirthYearLock)
            {
                BirthYearCache[person] = person.BirthYear;
            }
        }

        public static void ProcessCastLine(string linePart, List<Match> matches)
        {
            Debug.Assert(IsInitialized, "IMDbParser not initialized!");

            var matchColl = _castRegex.Matches(linePart);

            if (matchColl.Count > 0)
            {
                foreach (Match match in matchColl)
                {
                    matches.Add(match);
                }
            }
        }

        public static void ProcessCastLine(List<CastInfo> castList, List<Match> matches, DefaultValues defaultValues, SetProgress setProgress)
        {
            var maxCount = matches.Count;

            for (var castIndex = 0; castIndex < maxCount;)
            {
                var maxTasks = ((castIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - castIndex);

                var tasks = new List<Task<List<CastInfo>>>(maxTasks);

                for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, castIndex++)
                {
                    var match = matches[castIndex];

                    var task = Task.Run(() => CastLineProcessor.Process(match, defaultValues));

                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());

                foreach (var task in tasks)
                {
                    foreach (var castMember in task.Result)
                    {
                        castList.Add(castMember);
                    }
                }

                for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++)
                {
                    setProgress();
                }
            }
        }

        public static void ProcessCrewLine(string blockMatch, List<KeyValuePair<Match, List<Match>>> matches)
        {
            Debug.Assert(IsInitialized, "IMDbParser not initialized!");

            var creditTypeMatch = _creditTypeRegex.Match(blockMatch);

            var matchList = new List<Match>();

            var matchColl = _crewRegex.Matches(blockMatch);

            if (matchColl.Count > 0)
            {
                foreach (Match match in matchColl)
                {
                    matchList.Add(match);
                }
            }

            matches.Add(new KeyValuePair<Match, List<Match>>(creditTypeMatch, matchList));
        }

        public static void ProcessCrewLine(List<CrewInfo> crewList, List<KeyValuePair<Match, List<Match>>> matches, DefaultValues defaultValues, SetProgress setProgress)
        {
            var maxCount = matches.Count;

            for (var creditTypeIndex = 0; creditTypeIndex < maxCount;)
            {
                var maxTasks = ((creditTypeIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - creditTypeIndex);

                var tasks = new List<Task<Tuple<IEnumerable<CrewInfo>, int>>>();

                for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, creditTypeIndex++)
                {
                    var kvp = matches[creditTypeIndex];

                    var task = Task.Run(() => ProcessCrewLine(kvp.Key, kvp.Value, defaultValues));

                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());

                foreach (var task in tasks)
                {
                    crewList.AddRange(task.Result.Item1);

                    for (var i = 0; i < task.Result.Item2; i++)
                    {
                        setProgress();
                    }
                }
            }
        }

        private static Tuple<IEnumerable<CrewInfo>, int> ProcessCrewLine(Match creditTypeMatch, List<Match> crewMatches, DefaultValues defaultValues)
        {
            var result = new List<CrewInfo>();

            foreach (Match crewMatch in crewMatches)
            {
                if (crewMatch.Success)
                {
                    if (crewMatch.Groups["Credit"].Success)
                    {
                        var credit = crewMatch.Groups["Credit"].ToString();

                        credit = credit.Replace("</a>", string.Empty);
                        credit = credit.Replace("<br>", string.Empty);
                        credit = credit.Trim();

                        var originalCredit = HttpUtility.HtmlDecode(credit);

                        if (credit.EndsWith(" and"))
                        {
                            credit = credit.Substring(0, credit.Length - 4);
                            credit = credit.Trim();
                        }

                        if (credit.EndsWith(" &amp;"))
                        {
                            credit = credit.Substring(0, credit.Length - 6);
                            credit = credit.Trim();
                        }

                        if (credit.EndsWith(" &"))
                        {
                            credit = credit.Substring(0, credit.Length - 2);
                            credit = credit.Trim();
                        }

                        credit = HttpUtility.HtmlDecode(credit);
                        credit = credit.Trim();

                        var newMatch = UncreditedRegex.Match(credit);

                        if (newMatch.Success)
                        {
                            continue;
                        }

                        var split = credit.Split('/');

                        for (var crewIndex = 0; crewIndex < split.Length; crewIndex++)
                        {
                            credit = split[crewIndex].Trim();

                            var crewInfo = CrewLineProcessor.Process(defaultValues, creditTypeMatch, crewMatch, credit, originalCredit);

                            if (crewInfo != null)
                            {
                                result.Add(crewInfo);
                            }
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<CrewInfo>, int>(result, crewMatches.Count);
        }

        public static void ProcessSoundtrackLine(List<CrewInfo> crewList, Dictionary<string, List<Match>> soundtrackMatches, DefaultValues defaultValues, SetProgress setProgress)
        {
            if (defaultValues.CreditTypeSoundtrack)
            {
                foreach (var kvp in soundtrackMatches)
                {
                    var crewMatches = kvp.Value.ToList();

                    var maxCount = crewMatches.Count;

                    for (var crewIndex = 0; crewIndex < crewMatches.Count;)
                    {
                        var maxTasks = ((crewIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - crewIndex);

                        var tasks = new List<Task<CrewInfo>>(maxTasks);

                        for (var taskIndex = 0; taskIndex < maxTasks; taskIndex++, crewIndex++)
                        {
                            var crewMatch = crewMatches[crewIndex];

                            var task = Task.Run(() => SountrackLineProcessor.Process(kvp.Key, crewMatch, defaultValues.CheckPersonLinkForRedirect));

                            tasks.Add(task);
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks)
                        {
                            crewList.Add(task.Result);
                        }

                        for (int taskIndex = 0; taskIndex < maxTasks; taskIndex++)
                        {
                            setProgress();
                        }
                    }
                }
            }
        }

        public static Dictionary<string, List<Match>> ParseSoundtrack(StringBuilder soundtrack)
        {
            var matches = _soundtrackLiRegex.Matches(soundtrack.ToString());

            //soundtrack = new StringBuilder();

            var liMatches = new Dictionary<string, List<Match>>(matches.Count);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var titleSection = match.Groups["Soundtrack"].Value;

                    var indexOf = titleSection.IndexOf("<br");

                    if (indexOf != -1)
                    {
                        titleSection = titleSection.Substring(0, indexOf);
                    }

                    indexOf = titleSection.IndexOf("\">");

                    if (indexOf != -1)
                    {
                        titleSection = titleSection.Substring(indexOf + 2);
                    }

                    var titleMatch = _soundtrackTitleRegex.Match(titleSection);

                    string key;
                    if (titleMatch.Success)
                    {
                        key = titleMatch.Groups["Title"].Value;
                        key = HttpUtility.HtmlDecode(key);
                        key = key.Trim();
                    }
                    else
                    {
                        key = titleSection;
                        key = HttpUtility.HtmlDecode(key);
                        key = key.Trim();
                    }

                    var personMatches = _soundtrackPersonRegex.Matches(match.Groups["Soundtrack"].Value);

                    if (personMatches.Count > 0)
                    {
                        if (!liMatches.TryGetValue(key, out var list))
                        {
                            list = new List<Match>(personMatches.Count);

                            liMatches.Add(key, list);
                        }

                        foreach (Match personMatch in personMatches)
                        {
                            list.Add(personMatch);
                        }
                    }
                }
            }

            return liMatches;
        }

        internal static string GetUpdatedPersonLink(string personLink)
        {
            lock (_updatedPersonLock)
            {
                if (UpdatedPersonLinks.TryGetValue(personLink, out var newPersonLink))
                {
                    return newPersonLink;
                }
            }

            using (var response = GetWebResponse(PersonUrl + personLink + "/"))
            {
                var responseUri = response.ResponseUri.AbsoluteUri;

                var match = _personUrlRegex.Match(responseUri);

                if (match.Success)
                {
                    var newPersonLink = match.Groups["PersonLink"].Value;

                    lock (_updatedPersonLock)
                    {
                        UpdatedPersonLinks[personLink] = newPersonLink;
                    }

                    return newPersonLink;
                }

                return personLink;
            }
        }
    }
}