using System.Collections.Generic;
using System.IO;
using System.Linq;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public class MovieTests : TestBase
{
    private const string WinnetouLink = "tt0057687";
    private const string ABiggerSplashLink = "tt2056771";
    private const string LoveIsTheDrugLink = "tt0266732";
    private const string HotShotsLink = "tt0102059";
    private const string UchuLink = "tt0078435";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(IMDbParser.TitleUrl, WinnetouLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, ABiggerSplashLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, LoveIsTheDrugLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, HotShotsLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, UchuLink, "fullcredits");

    }

    [TestMethod]
    public void CrewWinnetou()
    {
        Crew(WinnetouLink, out var crewMatches, out var crewList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(15, crewMatches.Count, "crewMatches.Count");
        Assert.AreEqual(19, progressBarMaxValue, "progressBarMaxValue");
        Assert.AreEqual(18, crewList.Count, "crewList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CastWinnetou()
    {
        Cast(WinnetouLink, out var castMatches, out var castList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(39, castMatches.Count, "castMatches.Count");
        Assert.AreEqual(39, castList.Count, "castList.Count");
        Assert.AreEqual("Old Shatterhand", castList[0].Role, "castList[0].Role");
        Assert.AreEqual("Frederick Santer", castList[3].Role, "castList[3].Role");
        Assert.AreEqual("Vlado Krstulovic", castList[16].CreditedAs, "castList[16].CreditedAs");
        Assert.AreEqual("", castList[24].Role, "castList[24].Role");
        Assert.AreEqual("True", castList[25].Voice, "castList[25].Voice");
        Assert.AreEqual("True", castList[25].Uncredited, "castList[25].Uncredited");
        Assert.AreEqual("Schröder", castList[38].LastName, "castList[38].LastName");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CastABiggerSplash()
    {
        Cast(ABiggerSplashLink, out var castMatches, out var castList, out var progressBarMaxValue, out var existing, out var current);

        Assert.AreEqual(35, castMatches.Count, "castMatches.Count");
        Assert.AreEqual(35, castList.Count, "castList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CrewLoveIsTheDrug()
    {
        Crew(LoveIsTheDrugLink, out var crewMatches, out var crewList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(24, crewMatches.Count, "crewMatches.Count");
        Assert.AreEqual(106, progressBarMaxValue, "progressBarMaxValue");
        Assert.AreEqual(110, crewList.Count, "crewList.Count");
        Assert.AreEqual("Steven", crewList[51].FirstName, "crewList[51].FirstName");
        Assert.AreEqual("Avila", crewList[51].LastName, "crewList[51].LastName");
        Assert.AreEqual("Sound Designer", crewList[51].CreditSubtype, "crewList[51].CreditSubtype");
        Assert.AreEqual("Steven", crewList[52].FirstName, "crewList[52].FirstName");
        Assert.AreEqual("Avila", crewList[52].LastName, "crewList[52].LastName");
        Assert.AreEqual("Custom", crewList[52].CreditSubtype, "crewList[52].CreditSubtype");
        Assert.AreEqual("Trip", crewList[54].FirstName, "crewList[54].FirstName");
        Assert.AreEqual("Brock", crewList[54].LastName, "crewList[54].LastName");
        Assert.AreEqual("Sound Re-Recording Mixer", crewList[54].CreditSubtype, "crewList[54].CreditSubtype");
        Assert.AreEqual("Trip", crewList[55].FirstName, "crewList[55].FirstName");
        Assert.AreEqual("Brock", crewList[55].LastName, "crewList[55].LastName");
        Assert.AreEqual("Supervising Sound Editor", crewList[55].CreditSubtype, "crewList[55].CreditSubtype");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CastHotShots()
    {
        Cast(HotShotsLink, out var castMatches, out var castList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(74, castMatches.Count, "castMatches.Count");
        Assert.AreEqual(79, castList.Count, "castList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CrewHotShots()
    {
        Crew(HotShotsLink, out var crewMatches, out var crewList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(69, crewMatches.Count, "castMatches.Count");
        Assert.AreEqual(123, progressBarMaxValue, "progressBarMaxValue");
        Assert.AreEqual(116, crewList.Count, "crewList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CastUchu()
    {
        Cast(UchuLink, out var castMatches, out var castList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(50, castMatches.Count, "castMatches.Count");
        Assert.AreEqual(50, castList.Count, "castList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    [TestMethod]
    public void CrewUchu()
    {
        Crew(UchuLink, out var crewMatches, out var crewList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(37, progressBarMaxValue, "progressBarMaxValue");
        Assert.AreEqual(37, crewList.Count, "crewList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    private static void Crew(string key
        , out List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches
        , out List<CrewInfo> crewList
        , out int progressBarMaxValue
        , out FileInfo existing
        , out FileInfo current)
    {
        var defaultValues = new DefaultValues()
        {
            ParseCrew = true,
            CapitalizeCustomRole = true,
            CreditTypeArt = true,
            CreditTypeCinematography = true,
            CreditTypeDirection = true,
            CreditTypeFilmEditing = true,
            CreditTypeMusic = true,
            CreditTypeOther = true,
            CreditTypeProduction = true,
            CreditTypeSound = true,
            CreditTypeSoundtrack = true,
            CreditTypeWriting = true,
            IncludeCustomCredits = true,
            RetainCrewCreditedAs = true,
            RetainOriginalCredit = true,
            CheckPersonLinkForRedirect = false,
        };

        //using var mainForm = new MainForm(true, BrowserControlSelection.FormsDefault);

        //var mainFormType = mainForm.GetType();
        //var fieldInfo = mainFormType.GetField("ParseCrewCheckBox", BindingFlags.Instance | BindingFlags.NonPublic);
        //Assert.IsNotNull(fieldInfo, "fieldInfo");
        //var parseCrewCheckBox = (CheckBox)(fieldInfo.GetValue(mainForm));
        //parseCrewCheckBox.Checked = true;
        //var methodInfo = mainFormType.GetMethod("ParseCastAndCrew", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        //Assert.IsNotNull(methodInfo, "methodInfo");
        //crewMatches = new List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>>();
        //crewList = new List<CrewInfo>();
        //var parameters = new object[10]
        //{
        //   
        //};
        //methodInfo.Invoke(mainForm, parameters);
        //crewMatches = (List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>>)(parameters[7]);
        //crewList = (List<CrewInfo>)(parameters[8]);
        //mainForm._progressMax = 0;
        //mainForm._progressInterval = int.MaxValue;
        //foreach (var kvp in crewMatches)
        //{
        //    mainForm._progressMax += kvp.Value.Count;
        //}
        //mainForm._progressBar = new()
        //{
        //    Minimum = 0,
        //    Maximum = mainForm._progressMax,
        //};
        //mainForm._progressValue = 0;

        var castMatches = new List<CastMatch>();
        var castList = new List<CastInfo>();
        crewMatches = new List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>>();
        crewList = new List<CrewInfo>();
        var soundtrackMatches = new Dictionary<string, List<SoundtrackMatch>>();

        CastCrewEdit2BaseForm.ParseCastAndCrew(key
            , false
            , true
            , false
            , false
            , ref castMatches
            , ref castList
            , ref crewMatches
            , ref crewList
            , ref soundtrackMatches);

        var progressMax = 0;
        CrewParser.ProcessCrewLine(crewList, crewMatches, defaultValues, () => progressMax++);
        var crewInformation = new CrewInformation()
        {
            Title = key,
            CrewList = crewList.Select(ConvertCrew).ToArray(),
        };
        crewInformation.Serialize(@"Current\" + key + ".crew.xml");
        existing = new FileInfo(@"Existing\" + key + ".crew.xml");
        current = new FileInfo(@"Current\" + key + ".crew.xml");

        progressBarMaxValue = progressMax;
    }

    private static void Cast(string key
        , out List<CastMatch> castMatches
        , out List<CastInfo> castList
        , out int progressBarMaxValue
        , out FileInfo existing
        , out FileInfo current)
    {
        var defaultValues = new DefaultValues()
        {
            ParseCast = true,
            IgnoreArchiveFootage = false,
            IgnoreCreditOnly = false,
            IgnoreLanguageVersion = false,
            IgnoreScenesDeleted = false,
            IgnoreUnconfirmed = false,
            IgnoreUncredited = false,
            ParseRoleSlash = true,
            ParseVoiceOf = true,
            RetainCastCreditedAs = true,
            CheckPersonLinkForRedirect = false,
        };

        castMatches = new List<CastMatch>();
        castList = new List<CastInfo>();
        var crewMatches = new List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>>();
        var crewList = new List<CrewInfo>();
        var soundtrackMatches = new Dictionary<string, List<SoundtrackMatch>>();

        CastCrewEdit2BaseForm.ParseCastAndCrew(key
            , true
            , false
            , false
            , false
            , ref castMatches
            , ref castList
            , ref crewMatches
            , ref crewList
            , ref soundtrackMatches);

        var progressMax = 0;
        CastParser.ProcessCastLine(castList, castMatches, defaultValues, () => progressMax++);

        var castInformation = new CastInformation()
        {
            Title = key,
            CastList = castList.Select(ConvertCast).ToArray(),
        };
        castInformation.Serialize(@"Current\" + key + ".cast.xml");
        existing = new FileInfo(@"Existing\" + key + ".cast.xml");
        current = new FileInfo(@"Current\" + key + ".cast.xml");

        progressBarMaxValue = progressMax;
    }

    private static CastMember ConvertCast(CastInfo castInfo)
    {
        var castMember = new CastMember
        {
            LastName = castInfo.LastName,
            FirstName = castInfo.FirstName,
            MiddleName = castInfo.MiddleName,
            CreditedAs = castInfo.CreditedAs,
            Role = castInfo.Role,
            Uncredited = bool.Parse(castInfo.Uncredited),
            Voice = bool.Parse(castInfo.Voice),
        };
        if (int.TryParse(castInfo.BirthYear, out var birthYear))
        {
            castMember.BirthYear = birthYear;
        }
        return castMember;
    }
}