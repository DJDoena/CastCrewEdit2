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
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper
{
    public delegate void SetProgress();

    internal static class IMDbParser
    {
        public const Int32 MaxTasks = 4;

        public const String BaseUrl = @"http://www.imdb.com";
        public const String PersonUrl = BaseUrl + @"/name/";
        public const String TitleUrl = BaseUrl + @"/title/";
        public static readonly Regex TitleUrlRegex;
        public static readonly Regex TitleRegex;
        public static readonly Regex TriviaStartRegex;
        public static readonly Regex TriviaLiRegex;
        public static readonly Regex GoofsStartRegex;
        public static readonly Regex GoofsLiRegex;
        public static readonly Regex GoofSpoilerRegex;
#if UnitTest
        public static readonly Encoding Encoding;
#else
        private static readonly Encoding Encoding;
#endif
        internal static readonly Regex UncreditedRegex;
        internal static readonly Regex CreditedAsRegex;
        private static readonly Regex CastRegex;
        private static readonly Regex CreditTypeRegex;
        private static readonly Regex CrewRegex;
        private static readonly Regex PhotoRegex;
        private static readonly Regex PhotoUrlRegex;
        internal static readonly Regex SoundtrackStartRegex;
        private static readonly Regex SoundtrackLiRegex;
        private static readonly Regex SoundtrackTitleRegex;
        private static readonly Regex SoundtrackPersonRegex;
        internal static Dictionary<PersonInfo, String> BirthYearCache;
        internal static Dictionary<PersonInfo, FileInfo> HeadshotCache;
        private static List<String> KnownFirstnamePrefixes;
        private static List<String> KnownLastnamePrefixes;
        private static List<String> KnownLastnameSuffixes;
        private static Dictionary<String, Name> KnownNames;
        internal static Dictionary<String, UInt16> ForcedFakeBirthYears;
        internal static List<String> IgnoreCustomInIMDbCreditType;
        internal static List<String> IgnoreIMDbCreditTypeInOther;
        internal static IMDbToDVDProfilerCrewRoleTransformation TransformationData;
        //private static readonly XmlSerializer XmlSerializerTransformationData;
        private static readonly Regex PersonUrlRegex;
        private static Dictionary<String, String> UpdatedPersonLinks;
        private static Boolean IsInitialized;
        private static readonly Object UpdatedPersonLock;
        private static readonly Object GetBirthYearLock;

        private static Dictionary<String, String> WebSites;

#if UnitTest
        public static Dictionary<String, WebResponse> WebResponses;
#endif

        static IMDbParser()
        {
            const String DomainPrefix = "https?://((akas.)*|(www.)*|(us.)*|(german.)*)imdb.(com|de)/";

            UpdatedPersonLock = new Object();
            GetBirthYearLock = new Object();

            Encoding = Encoding.GetEncoding("UTF-8");
            TitleUrlRegex = new Regex(DomainPrefix + "title/(?'TitleLink'tt[0-9]+)/.*$", RegexOptions.Compiled);
            TitleRegex = new Regex(@"<title>(?'Title'.+?)</title>", RegexOptions.Compiled);
            UncreditedRegex = new Regex(@"\(?uncredited\)?", RegexOptions.Compiled);
            CreditedAsRegex = new Regex(@"\(as (?'CreditedAs'.+?)\)", RegexOptions.Compiled);
            CastRegex
                = new Regex("<td class=\"primary_photo\">.*?<td><a href=\"/name/(?'PersonLink'[a-z0-9]+)/.*?>(?'PersonName'.+?)</a>          </td><td class=\"ellipsis\">(...)?</td><td class=\"character\">(<div>)?(?'Role'.*?)(</div>)?</td>"
                    , RegexOptions.Compiled);

            CreditTypeRegex = new Regex("<h4 class=\"dataHeaderWithBorder\">(?'CreditType'.+?)(<span.+?)?(&nbsp;)?</h4>", RegexOptions.Compiled);
            //CrewRegex
            //    = new Regex("<td valign=\"top\"><a href=\"/name/(?'PersonLink'[a-z0-9]+)/\">(?'PersonName'.+?)</a></td>((<td( valign=\"top\")*>&nbsp;</td>)|(<td valign=\"top\" nowrap=\"1\"> .... </td>))<td valign=\"top\">(<a.+?>)*(?'Credit'.+?)</td>"
            //        , RegexOptions.Compiled);
            CrewRegex = new Regex("<td class=\"name\"><a href=\"/name/(?'PersonLink'[a-z0-9]+)/.*?>(?'PersonName'.+?)</a>.*?</td>((<td colspan=\"2\">)|(<td class=\"credit\">))(?'Credit'.*?)</td>", RegexOptions.Compiled);
            PhotoRegex = new Regex("<td.+?id=\"img_primary\".*?>", RegexOptions.Compiled);
            PhotoUrlRegex = new Regex("<img (.*?)src=\"(?'PhotoUrl'.+?)\"", RegexOptions.Compiled);
            TriviaStartRegex = new Regex("class=\"soda (even|odd) sodavote\".*?>", RegexOptions.Compiled);
            TriviaLiRegex = new Regex("<div class=\"sodatext\">(?'Trivia'.+?)</div>", RegexOptions.Singleline | RegexOptions.Compiled);
            GoofsStartRegex = new Regex("<h4 class=\"li_group\">", RegexOptions.Compiled);
            GoofsLiRegex = new Regex("<div class=\"sodatext\">(?'Goof'.+?)</div>", RegexOptions.Singleline);
            GoofSpoilerRegex = new Regex("<h4 class=\"inline\">.+?</h4>(?'Goof'.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            SoundtrackStartRegex = new Regex("<div id=\"soundtracks_content\"", RegexOptions.Compiled);
            SoundtrackLiRegex = new Regex("<div id=\"(?'Soundtrack'.+?)</div>", RegexOptions.Singleline | RegexOptions.Compiled);
            SoundtrackTitleRegex = new Regex("\"(?'Title'.+?)\"", RegexOptions.Compiled);
            SoundtrackPersonRegex = new Regex("<a href=\"/name/(?'PersonLink'[a-z0-9]+)(.+?)\">(?'PersonName'.+?)</a>", RegexOptions.Compiled);
            BirthYearCache = new Dictionary<PersonInfo, String>(1000);
            HeadshotCache = new Dictionary<PersonInfo, FileInfo>(1000);
            //XmlSerializerTransformationData = new XmlSerializer(typeof(IMDbToDVDProfilerCrewRoleTransformation));
            PersonUrlRegex = new Regex(DomainPrefix + "name/(?'PersonLink'nm[0-9]+)/.*$", RegexOptions.Compiled);
            UpdatedPersonLinks = new Dictionary<String, String>();
            IsInitialized = false;
            WebSites = new Dictionary<String, String>();


#if UnitTest
            WebResponses = new Dictionary<String, WebResponse>();
#endif

        }

        public static WebResponse GetWebResponse(String targetUrl)
        {

#if UnitTest
            return (WebResponses[targetUrl]);
#else
            WebResponse wr = OnlineAccess.CreateSystemSettingsWebRequest(targetUrl, CultureInfo.GetCultureInfo("en-US"));

            return (wr);
#endif

        }

        public static String GetWebSite(String targetUrl)
        {
            String webSite;
            if (WebSites.TryGetValue(targetUrl, out webSite))
            {
                return (webSite);
            }

            using (WebResponse webResponse = GetWebResponse(targetUrl))
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream, Encoding))
                    {
                        webSite = sr.ReadToEnd();

                        return (webSite);
                    }
                }
            }
        }

        public static String PersonHashCount
            => (BirthYearCache.Count.ToString());

        public static Dictionary<PersonInfo, String> PersonHash
            => (BirthYearCache);

        public static void Initialize(IWin32Window windowHandle)
        {
            String path;
            List<String> temp;

            path = Program.RootPath + @"\Data\";
            InitList(path + @"KnownFirstnamePrefixes.txt", ref KnownFirstnamePrefixes, windowHandle, true);
            InitList(path + @"KnownLastnamePrefixes.txt", ref KnownLastnamePrefixes, windowHandle, true);
            InitList(path + @"KnownLastnameSuffixes.txt", ref KnownLastnameSuffixes, windowHandle, true);
            temp = null;
            InitList(path + @"KnownNames.txt", ref temp, windowHandle, false);
            ProcessKnownNames(temp);
            InitList(path + @"IgnoreCustomInIMDbCategory.txt", ref IgnoreCustomInIMDbCreditType, false);
            InitList(path + @"IgnoreIMDbCategoryInOther.txt", ref IgnoreIMDbCreditTypeInOther, false);
            temp = null;
            InitList(path + @"ForcedFakeBirthYears.txt", ref temp, false);
            ProcessForcedFakeBirthYears(temp);
            InitIMDbToDVDProfilerCrewRoleTransformation(windowHandle);
            IsInitialized = true;
        }

        private static void ProcessKnownNames(List<String> knownNames)
        {
            KnownNames = new Dictionary<String, Name>(knownNames.Count);
            foreach (String knownName in knownNames)
            {
                String[] split;

                split = knownName.Split(';');
                if (split.Length == 4)
                {
                    if ((KnownNames.ContainsKey(split[0]) == false) && (String.IsNullOrEmpty(split[1]) == false))
                    {
                        Name name;

                        name = new Name();
                        name.FirstName = new StringBuilder(split[1]);
                        name.MiddleName = new StringBuilder(split[2]);
                        name.LastName = new StringBuilder(split[3]);
                        KnownNames.Add(split[0], name);
                    }
                }
            }
        }

        private static void ProcessForcedFakeBirthYears(List<String> forcedFakeBirthYears)
        {
            ForcedFakeBirthYears = new Dictionary<String, UInt16>(forcedFakeBirthYears.Count);
            foreach (String forcedFakeBirthYear in forcedFakeBirthYears)
            {
                String[] split;

                split = forcedFakeBirthYear.Split(';');
                if (split.Length == 2)
                {
                    if ((KnownNames.ContainsKey(split[0]) == false) && (String.IsNullOrEmpty(split[1]) == false))
                    {
                        UInt16 birthYear;

                        if (UInt16.TryParse(split[1], out birthYear))
                        {
                            ForcedFakeBirthYears.Add(split[0], birthYear);
                        }
                    }
                }
            }
        }

        public static void InitIMDbToDVDProfilerCrewRoleTransformation(IWin32Window windowHandle)
        {
            String fileName;
            String path;

            path = Program.RootPath + @"\Data\";
            fileName = path + @"IMDbToDVDProfilerCrewRoleTransformation.xml";
            if (File.Exists(fileName))
            {
                try
                {
                    TransformationData = DVDProfilerSerializer<IMDbToDVDProfilerCrewRoleTransformation>.Deserialize(fileName);
                }
                catch
                {
                    TransformationData = new IMDbToDVDProfilerCrewRoleTransformation();
                    TransformationData.CreditTypeList = new CreditType[0];
                    MessageBox.Show(windowHandle, String.Format(MessageBoxTexts.FileCantBeRead, fileName), MessageBoxTexts.ErrorHeader
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                TransformationData = new IMDbToDVDProfilerCrewRoleTransformation();
                TransformationData.CreditTypeList = new CreditType[0];
                MessageBox.Show(windowHandle, String.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void InitList(String fileName, FileNameType fileNameType, IWin32Window windowHandle)
        {
            switch (fileNameType)
            {
                case (FileNameType.FirstnamePrefixes):
                    {
                        InitList(fileName, ref KnownFirstnamePrefixes, windowHandle, true);
                        break;
                    }
                case (FileNameType.LastnamePrefixes):
                    {
                        InitList(fileName, ref KnownLastnamePrefixes, windowHandle, true);
                        break;
                    }
                case (FileNameType.LastnameSuffixes):
                    {
                        InitList(fileName, ref KnownLastnameSuffixes, windowHandle, true);
                        break;
                    }
                case (FileNameType.KnownNames):
                    {
                        List<String> knownNames;

                        knownNames = null;
                        InitList(fileName, ref knownNames, windowHandle, false);
                        ProcessKnownNames(knownNames);
                        break;
                    }
                case (FileNameType.IgnoreCustomInIMDbCreditType):
                    {
                        InitList(fileName, ref IgnoreCustomInIMDbCreditType, windowHandle, false);
                        break;
                    }
                case (FileNameType.IgnoreIMDbCreditTypeInOther):
                    {
                        InitList(fileName, ref IgnoreIMDbCreditTypeInOther, windowHandle, false);
                        break;
                    }
                case (FileNameType.ForcedFakeBirthYears):
                    {
                        List<String> forcedFakeBirthYears;

                        forcedFakeBirthYears = null;
                        InitList(fileName, ref forcedFakeBirthYears, windowHandle, false);
                        ProcessForcedFakeBirthYears(forcedFakeBirthYears);
                        break;
                    }
            }
        }

        private static void InitList(String fileName, ref List<String> list, IWin32Window windowHandle, Boolean toLower)
        {
            if (InitList(fileName, ref list, toLower) == false)
            {
                MessageBox.Show(windowHandle, String.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static Boolean InitList(String fileName, ref List<String> list, Boolean toLower)
        {
            list = new List<String>(50);
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (sr.EndOfStream == false)
                    {
                        String line;

                        line = sr.ReadLine();
                        if (String.IsNullOrEmpty(line) == false)
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
                return (true);
            }
            else
            {
                return (false);
            }
        }




        internal static Name ParsePersonName(String fullName)
        {
            Name retVal;
            String[] nameSplit;
            Int32 beginOfMiddleName;
            Int32 beginOfLastName;
            Boolean canBeSuffix;
            Boolean canBePrefix;

            fullName = HttpUtility.HtmlDecode(fullName).Trim();
            if (KnownNames.TryGetValue(fullName, out retVal))
            {
                return (retVal);
            }
            retVal = new Name();
            if ((fullName.ToLower().StartsWith("the ")) || ((fullName[0] >= '0') && (fullName[0] <= '9')))
            {
                //For bands/performers, e.g. "The Who", "50 Cent"
                retVal.FirstName = new StringBuilder(fullName);
                return (retVal);
            }
            if (Program.Settings.DefaultValues.ParseFirstNameInitialsIntoFirstAndMiddleName)
            {
                if (fullName.Contains("."))
                {
                    fullName = BreakInitialsApart(fullName);
                    //Now it might have happened that we split "M.D." into "M. D."
                    fullName = FindAndRepairBrokenInitials(fullName, KnownLastnameSuffixes);
                    fullName = FindAndRepairBrokenInitials(fullName, KnownLastnamePrefixes);
                    fullName = FindAndRepairBrokenInitials(fullName, KnownFirstnamePrefixes);
                }
            }
            fullName = CheckForQuotes(fullName, '\'', 0);
            fullName = CheckForQuotes(fullName, '"', 0);
            nameSplit = fullName.Split(' ');
            if (nameSplit.Length > 0)
            {
                nameSplit[0] = nameSplit[0].Replace("#SpacePlaceHolder#", " ");
            }
            if (nameSplit.Length == 1)
            {
                retVal.FirstName = new StringBuilder(nameSplit[0]);
                return (retVal);
            }
            beginOfMiddleName = -1;
            beginOfLastName = -1;
            canBeSuffix = true;
            canBePrefix = false;
            for (Int32 i = nameSplit.Length - 1; i >= 1; i--)
            {
                nameSplit[i] = nameSplit[i].Replace("#SpacePlaceHolder#", " ");
                if (canBeSuffix)
                {
                    beginOfLastName = i;
                    if (KnownLastnameSuffixes.Contains(nameSplit[i].ToLower()) == false)
                    {
                        canBeSuffix = false;
                        canBePrefix = true;
                    }
                    continue;
                }
                if (canBePrefix)
                {
                    if (KnownLastnamePrefixes.Contains(nameSplit[i].ToLower()))
                    {
                        beginOfLastName = i;
                        continue;
                    }
                }
                if ((i > 0) && (beginOfLastName > 1))
                {
                    beginOfMiddleName = 1;
                }
            }
            if (KnownFirstnamePrefixes.Contains(nameSplit[0].ToLower()))
            {
                for (Int32 i = 1; i < nameSplit.Length; i++)
                {
                    if (beginOfMiddleName == i)
                    {
                        beginOfMiddleName++;
                    }
                    if (beginOfLastName == i)
                    {
                        beginOfLastName++;
                    }
                    if (KnownFirstnamePrefixes.Contains(nameSplit[i].ToLower()))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (beginOfMiddleName == beginOfLastName)
            {
                beginOfMiddleName = -1;
            }
            if (beginOfMiddleName > 0)
            {
                for (Int32 i = 0; i < beginOfMiddleName; i++)
                {
                    retVal.FirstName.Append(" " + nameSplit[i]);
                }
                retVal.FirstName = new StringBuilder(retVal.FirstName.ToString().Trim());
                for (Int32 i = beginOfMiddleName; i < beginOfLastName; i++)
                {
                    retVal.MiddleName.Append(" " + nameSplit[i]);
                }
                retVal.MiddleName = new StringBuilder(retVal.MiddleName.ToString().Trim());
            }
            else
            {
                for (Int32 i = 0; i < beginOfLastName; i++)
                {
                    retVal.FirstName.Append(" " + nameSplit[i]);
                }
                retVal.FirstName = new StringBuilder(retVal.FirstName.ToString().Trim());
            }
            for (Int32 i = beginOfLastName; i < nameSplit.Length; i++)
            {
                retVal.LastName.Append(" " + nameSplit[i]);
            }
            retVal.LastName = new StringBuilder(retVal.LastName.ToString().Trim());
            return (retVal);
        }

        private static String FindAndRepairBrokenInitials(String fullName, List<String> list)
        {
            foreach (String entry in list)
            {
                if (entry.Contains("."))
                {
                    String newEntry;

                    newEntry = BreakInitialsApart(entry);
                    if (newEntry != entry)
                    {
                        Int32 indexOf;

                        indexOf = fullName.IndexOf(newEntry, StringComparison.InvariantCultureIgnoreCase);
                        if (indexOf != -1)
                        {
                            String toBeReplacedEntry;
                            String replaceEntry;

                            toBeReplacedEntry = fullName.Substring(indexOf, newEntry.Length);
                            //remove spaces again
                            replaceEntry = toBeReplacedEntry.Replace(" ", "");
                            fullName = fullName.Replace(toBeReplacedEntry, replaceEntry);
                        }
                    }
                }
            }
            return (fullName);
        }

        private static String BreakInitialsApart(String name)
        {
            String[] nameSplit;
            StringBuilder dotSplitter;

            nameSplit = name.Split('.');
            dotSplitter = new StringBuilder();
            for (Int32 i = 0; i < nameSplit.Length - 1; i++)
            {
                dotSplitter.Append(nameSplit[i].Trim() + ". ");
            }
            dotSplitter.Append(nameSplit[nameSplit.Length - 1].Trim());
            name = dotSplitter.ToString().Trim();
            return (name);
        }

        private static String CheckForQuotes(String fullName, Char unsplittable, Int32 rootIndexOf)
        {
            if (rootIndexOf < fullName.Length)
            {
                Int32 indexOf;
                indexOf = fullName.IndexOf(unsplittable, rootIndexOf);
                if ((indexOf != -1) && (indexOf != fullName.Length - 1))
                {
                    Int32 indexOf2;

                    indexOf2 = fullName.IndexOf(unsplittable, indexOf + 1);
                    if (indexOf2 != -1)
                    {
                        StringBuilder newName;
                        String section;

                        newName = new StringBuilder();
                        if (indexOf != 0)
                        {
                            newName.Append(fullName.Substring(0, indexOf));
                        }
                        section = fullName.Substring(indexOf, indexOf2 - indexOf + 1);
                        newName.Append(section.Replace(" ", "#SpacePlaceHolder#"));
                        if (indexOf2 != fullName.Length - 1)
                        {
                            newName.Append(fullName.Substring(indexOf2 + 1, fullName.Length - indexOf2 - 1));
                        }
                        fullName = newName.ToString();
                        indexOf2 = fullName.IndexOf(unsplittable, indexOf + 1);
                        return (CheckForQuotes(fullName, unsplittable, indexOf2 + 1));
                    }
                }
            }
            return (fullName);
        }

        public static FileInfo GetHeadshot(PersonInfo person)
        {
            if (HeadshotCache.ContainsKey(person) == false)
            {
                FileInfo jpg;
                FileInfo gif;
                String targetUrl;
                WebResponse webResponse;

                jpg = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + person.PersonLink + ".jpg");
                gif = new FileInfo(Program.RootPath + @"\Images\CastCrewEdit2\" + person.PersonLink + ".gif");
                targetUrl = PersonUrl + person.PersonLink;
                webResponse = null;
                try
                {
                    try
                    {
                        webResponse = GetWebResponse(targetUrl);
                    }
                    catch (WebException webEx)
                    {
                        if (webEx.Message.Contains("404"))
                        {
                            return (CheckExistingFile(person, jpg, gif));
                        }
                        else
                        {
                            throw;
                        }
                    }
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream, Encoding))
                        {
                            while (sr.EndOfStream == false)
                            {
                                String line;
                                Match match;

                                line = sr.ReadLine();
                                match = PhotoRegex.Match(line);
                                if (match.Success)
                                {
                                    line = String.Empty;
                                    while (line.Contains("</td>") == false)
                                    {
                                        line += sr.ReadLine();
                                    }
                                    match = PhotoUrlRegex.Match(line);
                                    if (match.Success)
                                    {
                                        WebClient webClient;
                                        String remoteFile;

                                        webClient = null;
                                        try
                                        {
                                            Int64 contentLength;

                                            contentLength = -1;
                                            remoteFile = match.Groups["PhotoUrl"].Value.ToString();
                                            if ((remoteFile.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase))
                                               && (jpg.Exists))
                                            {
                                                contentLength = GetContentLength(remoteFile);
                                                if (contentLength != jpg.Length)
                                                {
                                                    contentLength = -1;
                                                }
                                            }
                                            else if ((remoteFile.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                                              && (gif.Exists))
                                            {
                                                contentLength = GetContentLength(remoteFile);
                                                if (contentLength != gif.Length)
                                                {
                                                    contentLength = -1;
                                                }
                                            }
                                            if (contentLength == -1)
                                            {
                                                try
                                                {
                                                    //File either don't exist, couldn't be determined remotely or is of different size
                                                    webClient = OnlineAccess.CreateSystemSettingsWebClient();
                                                    if (remoteFile.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        webClient.DownloadFile(remoteFile, jpg.FullName);
                                                        jpg.Refresh();
                                                    }
                                                    else if (remoteFile.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        webClient.DownloadFile(remoteFile, gif.FullName);
                                                        gif.Refresh();
                                                    }
                                                }
                                                catch (WebException webEx)
                                                {
                                                    if (webEx.Message.Contains("404"))
                                                    {
                                                        return (CheckExistingFile(person, jpg, gif));
                                                    }
                                                    else
                                                    {
                                                        throw;
                                                    }
                                                }
                                            }
                                            return (CheckExistingFile(person, jpg, gif));
                                        }
                                        finally
                                        {
                                            try
                                            {
                                                if (webClient != null)
                                                {
                                                    webClient.Dispose();
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                            return (CheckExistingFile(person, jpg, gif));
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
            else
            {
                return (HeadshotCache[person]);
            }
        }

        private static FileInfo CheckExistingFile(PersonInfo person, FileInfo jpg, FileInfo gif)
        {
            if (jpg.Exists)
            {
                HeadshotCache[person] = jpg;

                return (jpg);
            }
            else if (gif.Exists)
            {
                HeadshotCache[person] = gif;

                return (gif);
            }
            else
            {
                HeadshotCache[person] = null;

                return (null);
            }
        }

        private static Int64 GetContentLength(String remoteFile)
        {
            WebResponse webResponse;
            Int64 contentLength;

            contentLength = -1;
            webResponse = null;
            try
            {
                webResponse = GetWebResponse(remoteFile);
                contentLength = webResponse.ContentLength;
            }
            catch
            {
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
            return (contentLength);
        }

        public static void GetBirthYear(PersonInfo person)
        {
            lock (GetBirthYearLock)
            {
                if (BirthYearCache.ContainsKey(person))
                {
                    person.BirthYear = BirthYearCache[person];

                    return;
                }
            }

            person.BirthYear = BirthYearGetter.Get(person.PersonLink);

            person.BirthYearWasRetrieved = true;

            lock (GetBirthYearLock)
            {
                BirthYearCache[person] = person.BirthYear;
            }
        }

        public static void ProcessCastLine(String linePart, List<CastInfo> castList, List<Match> matches
            , DefaultValues defaultValues)
        {
            MatchCollection matchColl;

            Debug.Assert(IsInitialized, "IMDbParser not initialized!");
            matchColl = CastRegex.Matches(linePart);
            if (matchColl.Count > 0)
            {
                foreach (Match match in matchColl)
                {
                    matches.Add(match);
                }
            }
        }

        public static void ProcessCastLine(List<CastInfo> castList
            , List<Match> matches
            , DefaultValues defaultValues
            , SetProgress setProgress)
        {
            Int32 maxCount = matches.Count;

            for (Int32 castIndex = 0; castIndex < maxCount;)
            {
                Int32 maxTasks = ((castIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - castIndex);

                List<Task<List<CastInfo>>> tasks = new List<Task<List<CastInfo>>>(maxTasks);

                for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++, castIndex++)
                {
                    Match match = matches[castIndex];

                    Task<List<CastInfo>> task = Task.Run(() => CastLineProcessor.Process(match, defaultValues));

                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());

                foreach (Task<List<CastInfo>> task in tasks)
                {
                    foreach (CastInfo castMember in task.Result)
                    {
                        castList.Add(castMember);
                    }
                }

                for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++)
                {
                    setProgress();
                }
            }
        }

        public static void ProcessCrewLine(String blockMatch, List<CrewInfo> crewList
            , List<KeyValuePair<Match, List<Match>>> matches, DefaultValues defaultValues)
        {
            Match creditTypeMatch;
            MatchCollection matchColl;
            List<Match> matchList;

            Debug.Assert(IsInitialized, "IMDbParser not initialized!");
            creditTypeMatch = CreditTypeRegex.Match(blockMatch);
            matchList = new List<Match>();
            matchColl = CrewRegex.Matches(blockMatch);
            if (matchColl.Count > 0)
            {
                foreach (Match match in matchColl)
                {
                    matchList.Add(match);
                }
            }
            matches.Add(new KeyValuePair<Match, List<Match>>(creditTypeMatch, matchList));
        }

        public static void ProcessCrewLine(List<CrewInfo> crewList
            , List<KeyValuePair<Match, List<Match>>> matches
            , DefaultValues defaultValues
            , SetProgress setProgress)
        {
            Int32 maxCount = matches.Count;

            for (Int32 creditTypeIndex = 0; creditTypeIndex < maxCount;)
            {
                Int32 maxTasks = ((creditTypeIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - creditTypeIndex);

                List<Task<Tuple<IEnumerable<CrewInfo>, Int32>>> tasks = new List<Task<Tuple<IEnumerable<CrewInfo>, Int32>>>();

                for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++, creditTypeIndex++)
                {
                    KeyValuePair<Match, List<Match>> kvp = matches[creditTypeIndex];

                    Task<Tuple<IEnumerable<CrewInfo>, Int32>> task = Task.Run(() => ProcessCrewLine(kvp.Key, kvp.Value, defaultValues));

                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());

                foreach (Task<Tuple<IEnumerable<CrewInfo>, Int32>> task in tasks)
                {
                    crewList.AddRange(task.Result.Item1);

                    for (Int32 i = 0; i < task.Result.Item2; i++)
                    {
                        setProgress();
                    }
                }
            }
        }

        private static Tuple<IEnumerable<CrewInfo>, Int32> ProcessCrewLine(Match creditTypeMatch
            , List<Match> crewMatches
            , DefaultValues defaultValues)
        {
            List<CrewInfo> result = new List<CrewInfo>();

            foreach (Match crewMatch in crewMatches)
            {
                if (crewMatch.Success)
                {
                    if (crewMatch.Groups["Credit"].Success)
                    {
                        String credit = crewMatch.Groups["Credit"].ToString();

                        credit = credit.Replace("</a>", String.Empty);
                        credit = credit.Replace("<br>", String.Empty);
                        credit = credit.Trim();

                        String originalCredit = HttpUtility.HtmlDecode(credit);

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

                        Match newMatch = UncreditedRegex.Match(credit);

                        if (newMatch.Success)
                        {
                            continue;
                        }

                        String[] split = credit.Split('/');

                        for (Int32 crewIndex = 0; crewIndex < split.Length; crewIndex++)
                        {
                            credit = split[crewIndex].Trim();

                            CrewInfo crewInfo = CrewLineProcessor.Process(defaultValues, creditTypeMatch, crewMatch, credit, originalCredit);

                            if (crewInfo != null)
                            {
                                result.Add(crewInfo);
                            }
                        }
                    }
                }
            }

            return (new Tuple<IEnumerable<CrewInfo>, Int32>(result, crewMatches.Count));
        }

        public static void ProcessSoundtrackLine(List<CrewInfo> crewList
            , Dictionary<String, List<Match>> soundtrackMatches
            , DefaultValues defaultValues
            , SetProgress setProgress)
        {
            if (defaultValues.CreditTypeSoundtrack)
            {
                foreach (KeyValuePair<String, List<Match>> kvp in soundtrackMatches)
                {
                    List<Match> crewMatches = kvp.Value.ToList();

                    Int32 maxCount = crewMatches.Count;

                    for (Int32 crewIndex = 0; crewIndex < crewMatches.Count;)
                    {
                        Int32 maxTasks = ((crewIndex + MaxTasks - 1) < maxCount) ? MaxTasks : (maxCount - crewIndex);

                        List<Task<CrewInfo>> tasks = new List<Task<CrewInfo>>(maxTasks);

                        for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++, crewIndex++)
                        {
                            Match crewMatch = crewMatches[crewIndex];

                            Task<CrewInfo> task = Task.Run(() => SountrackLineProcessor.Process(kvp.Key, crewMatch, defaultValues.CheckPersonLinkForRedirect));

                            tasks.Add(task);
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (Task<CrewInfo> task in tasks)
                        {
                            crewList.Add(task.Result);
                        }

                        for (Int32 taskIndex = 0; taskIndex < maxTasks; taskIndex++)
                        {
                            setProgress();
                        }
                    }
                }
            }
        }

        public static Dictionary<String, List<Match>> ParseSoundtrack(StringBuilder soundtrack)
        {
            MatchCollection matches;
            Dictionary<String, List<Match>> liMatches;

            matches = IMDbParser.SoundtrackLiRegex.Matches(soundtrack.ToString());
            soundtrack = new StringBuilder();
            liMatches = new Dictionary<String, List<Match>>(matches.Count);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    String titleSection;
                    Match titleMatch;
                    Int32 indexOf;
                    String key;
                    MatchCollection personMatches;

                    titleSection = match.Groups["Soundtrack"].Value;
                    indexOf = titleSection.IndexOf("<br");
                    if (indexOf != -1)
                    {
                        titleSection = titleSection.Substring(0, indexOf);
                    }
                    indexOf = titleSection.IndexOf("\">");
                    if (indexOf != -1)
                    {
                        titleSection = titleSection.Substring(indexOf + 2);
                    }
                    titleMatch = IMDbParser.SoundtrackTitleRegex.Match(titleSection);
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
                    personMatches = IMDbParser.SoundtrackPersonRegex.Matches(match.Groups["Soundtrack"].Value);
                    if (personMatches.Count > 0)
                    {
                        List<Match> list;

                        if (liMatches.TryGetValue(key, out list) == false)
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
            return (liMatches);
        }

        internal static String GetUpdatedPersonLink(String personLink)
        {
            String newPersonLink;

            lock (UpdatedPersonLock)
            {
                if (UpdatedPersonLinks.TryGetValue(personLink, out newPersonLink))
                {
                    return (newPersonLink);
                }
            }

            using (WebResponse response = GetWebResponse(PersonUrl + personLink + "/"))
            {
                String responseUri = response.ResponseUri.AbsoluteUri;

                Match match = PersonUrlRegex.Match(responseUri);

                if (match.Success)
                {
                    newPersonLink = match.Groups["PersonLink"].Value;

                    lock (UpdatedPersonLock)
                    {
                        UpdatedPersonLinks[personLink] = newPersonLink;
                    }

                    return (newPersonLink);
                }

                return (personLink);
            }
        }
    }
}