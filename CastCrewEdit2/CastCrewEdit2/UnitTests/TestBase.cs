using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DoenaSoft.AbstractionLayer.WebServices;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public abstract class TestBase
{
    [AssemblyInitialize]
    public static void BeforeClass(TestContext _)
    {
        var assembly = typeof(TestBase).Assembly;

        AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;

        CleanUp();

        Program.RootPath = (new FileInfo(assembly.Location)).DirectoryName;

        Program.InitDataPaths();

        Program.Main(null);
    }

    private static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = args.Name.Split(',')[0];

        var assemblyFileName = Path.Combine(Program.RootPath, $"{assemblyName}.dll");

        if (File.Exists(assemblyFileName))
        {
            var assembly = Assembly.LoadFrom(assemblyFileName);

            return assembly;
        }
        else
        {
            return null;
        }
    }

    protected static void CreateMockWebResponse(string baseUrl, string key, string appendix)
    {
        string targetUrl;
        IWebResponse webResponse;
        string fileName;

        targetUrl = baseUrl + key;
        fileName = @"Current\" + key;
        if (appendix != null)
        {
            string fileAppendix;

            fileAppendix = appendix;
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
        webResponse = OnlineAccess.GetSystemSettingsWebResponseAsync(targetUrl).GetAwaiter().GetResult();

        using var webStream = webResponse.GetResponseStream();

        using var sr = new StreamReader(webStream, IMDbParser.Encoding);

        using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);

        using var sw = new StreamWriter(fileStream, IMDbParser.Encoding);

        sw.Write(sr.ReadToEnd());

        IMDbParser.WebResponses[targetUrl] = new MockWebResponse(fileName);

        webResponse.Close();
    }

    protected static void CreateMockWebResponse(string baseUrl, string key)
    {
        CreateMockWebResponse(baseUrl, key, null);
    }

    private static void CleanUp()
    {
        Process p;

        p = new Process();
        p.StartInfo = new ProcessStartInfo("CleanUp.cmd");
        p.Start();
        p.WaitForExit();
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