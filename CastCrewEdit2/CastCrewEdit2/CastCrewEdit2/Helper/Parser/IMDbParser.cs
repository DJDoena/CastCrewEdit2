using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Resources;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal delegate void SetProgress();

internal static class IMDbParser
{
#if DEBUG || UnitTest
    internal const int MaxTasks = 1;
#else
    internal const int MaxTasks = 4;
#endif

    internal const string DomainPrefix = "https?:\\/\\/((akas.)*|(www.)*|(us.)*|(german.)*)imdb.(com|de)\\/([a-z]{2}\\/)?";

    internal const string BaseUrl = @"https://www.imdb.com";

    internal const string TitleUrl = BaseUrl + @"/title/";

    private static readonly object _getWebsiteLock;

    internal static object GetImdbLock { get; }

    internal static bool IsInitialized { get; private set; }

    internal static DateTime LastRequestTimestamp { get; private set; }

    internal static Regex BlockEndRegex { get; }

    internal static Regex TitleUrlRegex { get; }

    internal static Regex TitleRegex { get; }

    internal static Encoding Encoding { get; }

#if UnitTest

    internal static Dictionary<string, WebResponse> WebResponses { get; }

#endif

    internal static Regex UncreditedRegex { get; }

    internal static Regex CreditedAsRegex { get; }

    private static Dictionary<string, string> WebSites
        => SessionData.WebSites;

    internal static IMDbToDVDProfilerCrewRoleTransformation TransformationData { get; private set; }

    internal static SessionData SessionData { get; set; }

    internal static string PersonHashCount
        => SessionData.BirthYearCache.Count.ToString();

    internal static Dictionary<PersonInfo, string> PersonHash
        => SessionData.BirthYearCache;

    static IMDbParser()
    {
        _getWebsiteLock = new object();

        Encoding = Encoding.GetEncoding("UTF-8");

        BlockEndRegex = new Regex("</table>", RegexOptions.Compiled);

        TitleUrlRegex = new Regex(DomainPrefix + "title\\/(?'TitleLink'tt[0-9]+)\\/.*$", RegexOptions.Compiled);

        TitleRegex = new Regex(@"<title>(?'Title'.+?)</title>", RegexOptions.Compiled);

        UncreditedRegex = new Regex(@"\(?uncredited\)?", RegexOptions.Compiled);

        CreditedAsRegex = new Regex(@"\(as (?'CreditedAs'.+?)\)", RegexOptions.Compiled);

        SessionData = new SessionData();

        IsInitialized = false;

        LastRequestTimestamp = DateTime.MinValue;

        GetImdbLock = new object();

#if UnitTest

        WebResponses = new Dictionary<string, WebResponse>();

#endif
    }

    internal static WebResponse GetWebResponse(string targetUrl)
    {

#if !UnitTest
        lock (GetImdbLock)
        {
            var retryCount = 0;

            if (LastRequestTimestamp.AddSeconds(1) >= DateTime.Now)
            {
                System.Threading.Thread.Sleep(250);
            }

            while (true)
            {
                try
                {
                    //var wr = OnlineAccess.CreateSystemSettingsWebRequest(targetUrl, CultureInfo.GetCultureInfo("en-US"));

                    var wr = CreateSystemSettingsWebRequestAsync(targetUrl, CultureInfo.GetCultureInfo("en-US")).GetAwaiter().GetResult();

                    LastRequestTimestamp = DateTime.Now;

                    return wr;
                }
                catch (WebException webEx)
                {
                    if (!PageNotFound(webEx))
                    {
                        retryCount++;

                        if (retryCount == 10)
                        {
                            throw;
                        }
                        else
                        {
                            var factor = retryCount < 5
                                ? 1000
                                : 2000;

                            System.Threading.Thread.Sleep(retryCount * factor);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

#else

        if (WebResponses.TryGetValue(targetUrl, out var webResponse))
        {
            return webResponse;
        }
        else if (!targetUrl.EndsWith("/") && WebResponses.TryGetValue(targetUrl + "/", out webResponse))
        {
            return webResponse;
        }
        else if (targetUrl.EndsWith("/") && WebResponses.TryGetValue(targetUrl.Substring(0, targetUrl.Length - 1), out webResponse))
        {
            return webResponse;
        }
        else
        {
            throw new ArgumentException($"Key '{targetUrl}' not found!");
        }

#endif

    }

    internal static async Task<WebResponse> CreateSystemSettingsWebRequestAsync(string targetUrl, CultureInfo ci = null)
    {
        var webRequest = (HttpWebRequest)WebRequest.Create(targetUrl);

        //webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        if (ci != null)
        {
            var value = ci.TwoLetterISOLanguageName.ToLower();

            webRequest.Headers["Accept-Language"] = value;
        }

        webRequest.Proxy = WebRequest.GetSystemWebProxy();
        webRequest.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

        try
        {
            var webResponse = await webRequest.GetResponseAsync().ConfigureAwait(false);

            return webResponse;
        }
        catch (WebException webEx)
        {
            if (webEx.Message?.Contains("308") == true)
            {
                var redirectTo = webEx.Response?.Headers["Location"];

                if (!string.IsNullOrEmpty(redirectTo))
                {
                    var targetUri = new Uri(targetUrl);

                    var newTargetUrl = $"{targetUri.Scheme}://{targetUri.Host}{redirectTo}";

                    var webResponse = await CreateSystemSettingsWebRequestAsync(newTargetUrl, ci).ConfigureAwait(false);

                    return webResponse;
                }
            }

            throw;
        }
    }

    internal static string GetWebSite(string targetUrl)
    {
        lock (_getWebsiteLock)
        {
            if (WebSites.TryGetValue(targetUrl, out var webSite))
            {
                return webSite;
            }

            var cacheFile = GetCacheFileName(targetUrl);

#if !UnitTest
            if (cacheFile.Exists && cacheFile.LastWriteTimeUtc > DateTime.UtcNow.AddDays(-7)) //one week
#else
            if (false)
#endif
            {
                using var fileStreamReader = new StreamReader(cacheFile.FullName, Encoding);

                webSite = fileStreamReader.ReadToEnd();

                WebSites.Add(targetUrl, webSite);
            }
            else
            {
                using var webResponse = GetWebResponse(targetUrl);

                using var webStream = webResponse.GetResponseStream();

                using var webStreamReader = new StreamReader(webStream, Encoding);

                webSite = webStreamReader.ReadToEnd();

                WebSites.Add(targetUrl, webSite);

                using var fileStreamWriter = new StreamWriter(cacheFile.FullName, false, Encoding);

                fileStreamWriter.Write(webSite);
            }

            return webSite;
        }
    }

    private static FileInfo GetCacheFileName(string targetUrl)
    {
        var cacheFileName = targetUrl;

        foreach (var c in Path.GetInvalidFileNameChars())
        {
            cacheFileName = cacheFileName.Replace(c, '_');
        }

        cacheFileName = Path.Combine(Path.GetTempPath(), $"{cacheFileName}.cce2");

        return new FileInfo(cacheFileName);
    }

    internal static void Initialize(IWin32Window windowHandle)
    {
        var path = Program.RootPath + @"\Data\";

        NameParser.Initialize(windowHandle, path);

        InitList(path + @"IgnoreCustomInIMDbCategory.txt", out var list, false);
        CrewParser.IgnoreCustomInIMDbCreditType = list;

        InitList(path + @"IgnoreIMDbCategoryInOther.txt", out list, false);
        CrewParser.IgnoreIMDbCreditTypeInOther = list;

        InitList(path + @"ForcedFakeBirthYears.txt", out list, false);
        BirthYearParser.ProcessForcedFakeBirthYears(list);

        InitIMDbToDVDProfilerCrewRoleTransformation(windowHandle);

        IsInitialized = true;
    }

    internal static void InitIMDbToDVDProfilerCrewRoleTransformation(IWin32Window windowHandle)
    {
        var path = Program.RootPath + @"\Data\";

        var fileName = path + @"IMDbToDVDProfilerCrewRoleTransformation.xml";

        if (File.Exists(fileName))
        {
            try
            {
                TransformationData = XmlSerializer<IMDbToDVDProfilerCrewRoleTransformation>.Deserialize(fileName);
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

    internal static void InitList(string fileName, FileNameType fileNameType, IWin32Window windowHandle)
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
                    InitList(fileName, out var list, windowHandle, false);
                    CrewParser.IgnoreCustomInIMDbCreditType = list;

                    break;
                }
            case FileNameType.IgnoreIMDbCreditTypeInOther:
                {
                    InitList(fileName, out var list, windowHandle, false);
                    CrewParser.IgnoreIMDbCreditTypeInOther = list;

                    break;
                }
            case FileNameType.ForcedFakeBirthYears:
                {
                    InitList(fileName, out var forcedFakeBirthYears, windowHandle, false);

                    BirthYearParser.ProcessForcedFakeBirthYears(forcedFakeBirthYears);

                    break;
                }
        }
    }

    internal static void InitList(string fileName, out List<string> list, IWin32Window windowHandle, bool toLower)
    {
        if (!InitList(fileName, out list, toLower))
        {
            MessageBox.Show(windowHandle, string.Format(MessageBoxTexts.FileDoesNotExist, fileName), MessageBoxTexts.WarningHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static bool InitList(string fileName, out List<string> list, bool toLower)
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

    internal static bool PageNotFound(WebException webEx)
    {
        return webEx.Message.Contains("404"); //not found
                                              //|| webEx.Message.Contains("308"); //permanent redirect
    }
}