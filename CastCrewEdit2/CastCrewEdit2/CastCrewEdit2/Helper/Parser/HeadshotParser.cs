using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;

internal static class HeadshotParser
{
    private static readonly Regex _photoUrlRegex;

    internal static Dictionary<PersonInfo, FileInfo> HeadshotCache
        => IMDbParser.SessionData.HeadshotCache;

    static HeadshotParser()
    {
        _photoUrlRegex = new Regex("\"image\":\"(?'PhotoUrl'.+?)\"", RegexOptions.Compiled);
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

        var targetUrl = $"{PersonLinkParser.PersonUrl}{person.PersonLink}/";

        string webSite;
        try
        {
            webSite = IMDbParser.GetWebSite(targetUrl);
        }
        catch (WebException webEx)
        {
            if (IMDbParser.PageNotFound(webEx))
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

                var indexOfPerson = line.IndexOf($"\"Person\",\"url\":\"https://www.imdb.com/name/{person.PersonLink}", StringComparison.InvariantCultureIgnoreCase);

                if (indexOfPerson != -1)
                {
                    line = line.Substring(indexOfPerson);

                    while (!line.Contains("</script>"))
                    {
                        line += sr.ReadLine();
                    }

                    var indexOfScript = line.IndexOf("</script>", StringComparison.InvariantCultureIgnoreCase);

                    if (indexOfScript != -1)
                    {
                        line = line.Substring(0, indexOfScript);
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
            if (IMDbParser.PageNotFound(webEx))
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
        lock (IMDbParser.GetImdbLock)
        {
            var retryCount = 0;

            if (IMDbParser.LastRequestTimestamp.AddSeconds(1) >= DateTime.Now)
            {
                Task.Delay(100).Wait();
            }

            while (true)
            {
                try
                {
                    using (var webClient = OnlineAccess.CreateSystemSettingsWebClient())
                    {
                        webClient.DownloadFile(remoteFile, imageFileInfo.FullName);

                        imageFileInfo.Refresh();

                        return;
                    }
                }
                catch
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

                        Task.Delay(retryCount * factor).Wait();
                    }
                }
            }
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
}
