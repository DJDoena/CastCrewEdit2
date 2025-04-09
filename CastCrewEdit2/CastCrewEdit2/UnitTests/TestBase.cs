using System.IO;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

public abstract class TestBase
{
    protected static void CreateMockWebResponse(string baseUrl, string key, string appendix)
    {
        var targetUrl = baseUrl + key;

        var fileName = @"Current\" + key;

        if (appendix != null)
        {
            var fileAppendix = appendix;

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileAppendix = fileAppendix.Replace(c, '_');
            }

            fileName += "." + fileAppendix;

            targetUrl += "/" + appendix;
        }
        else if (!targetUrl.EndsWith("/"))
        {
            targetUrl += "/";
        }

        fileName += ".html.txt";

        using var webResponse = WebSiteReader.CreateSystemSettingsWebRequestAsync(targetUrl).GetAwaiter().GetResult();

        using var webStream = webResponse.GetResponseStream();

        using var sr = new StreamReader(webStream, WebSiteReader.Encoding);

        using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);

        using var sw = new StreamWriter(fileStream, WebSiteReader.Encoding);

        sw.Write(sr.ReadToEnd());

        WebSiteReader.WebResponses[targetUrl] = new MockWebResponse(fileName);

        webResponse.Close();
    }

    protected static void CreateMockWebResponse(string baseUrl, string key)
    {
        CreateMockWebResponse(baseUrl, key, null);
    }

    protected static CrewMember ConvertCrew(CrewInfo crewInfo)
    {
        var crewMember = new CrewMember
        {
            LastName = crewInfo.LastName,
            FirstName = crewInfo.FirstName,
            MiddleName = crewInfo.MiddleName,
            CreditedAs = crewInfo.CreditedAs,
            CreditSubtype = crewInfo.CreditSubtype,
            CreditType = crewInfo.CreditType,
        };
        if (int.TryParse(crewInfo.BirthYear, out var birthYear))
        {
            crewMember.BirthYear = birthYear;
        }
        if (string.IsNullOrEmpty(crewInfo.CustomRole) == false)
        {
            crewMember.CustomRole = crewInfo.CustomRole;
            crewMember.CustomRoleSpecified = true;
        }
        return crewMember;
    }
}