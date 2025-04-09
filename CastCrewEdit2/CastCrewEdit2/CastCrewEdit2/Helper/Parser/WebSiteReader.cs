using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
internal static class WebSiteReader
{
    private static readonly object _getWebsiteLock;

    internal static object GetImdbLock { get; }

    internal static DateTime LastRequestTimestamp { get; private set; }

    internal static Encoding Encoding { get; }

#if UnitTest

    internal static Dictionary<string, WebResponse> WebResponses { get; }

#endif

    private static Dictionary<string, string> WebSites
        => IMDbParser.SessionData.WebSites;

    static WebSiteReader()
    {
        _getWebsiteLock = new object();

        GetImdbLock = new object();

        LastRequestTimestamp = DateTime.MinValue;

        Encoding = Encoding.GetEncoding("UTF-8");

#if UnitTest

        WebResponses = new Dictionary<string, WebResponse>();

#endif
    }

    internal static string GetWebSite(string targetUrl
       , bool tryUseBrowser = false)
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
                webSite = ReadFromCache(targetUrl, cacheFile);
            }
            else
            {
                if (Program.UseBrowserWindow)
                {
                    webSite = TryReadFromBrowser(targetUrl);
                }

                if (string.IsNullOrEmpty(webSite))
                {
                    webSite = ReadFromWebResponse(targetUrl);
                }

                WriteCacheFile(webSite, cacheFile);

                return webSite;
            }

            WebSites.Add(targetUrl, webSite);

            return webSite;
        }
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

    internal static bool PageNotFound(WebException webEx)
    {
        return webEx.Message.Contains("404"); //not found
                                              //|| webEx.Message.Contains("308"); //permanent redirect
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

    private static string ReadFromCache(string targetUrl, FileInfo cacheFile)
    {
        using var fileStreamReader = new StreamReader(cacheFile.FullName, Encoding);

        var webSite = fileStreamReader.ReadToEnd();

        return webSite;
    }

    private static string TryReadFromBrowser(string targetUrl)
    {
        try
        {
            using var browserForm = new BrowserForm(targetUrl);

            var navigationCompleted = false;

            browserForm.NavidationCompleted += (s, e) =>
            {
                navigationCompleted = true;
            };

            browserForm.Show();

            while (!navigationCompleted)
            {
                Application.DoEvents();
            }

            var html = browserForm.Html;

            browserForm.Close();

            return html;
        }
        catch
        {
            return null;
        }
    }

    private static string ReadFromWebResponse(string targetUrl)
    {
        using var webResponse = GetWebResponse(targetUrl);

        using var webStream = webResponse.GetResponseStream();

        using var webStreamReader = new StreamReader(webStream, Encoding);

        var webSite = webStreamReader.ReadToEnd();

        return webSite;
    }

    private static void WriteCacheFile(string webSite, FileInfo cacheFile)
    {
        using var fileStreamWriter = new StreamWriter(cacheFile.FullName, false, Encoding);

        fileStreamWriter.Write(webSite);
    }
}