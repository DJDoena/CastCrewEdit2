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
    private const string KickAssLink = "tt1250777";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(IMDbParser.TitleUrl, WinnetouLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, ABiggerSplashLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, LoveIsTheDrugLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, HotShotsLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, UchuLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, KickAssLink, "fullcredits");
    }

    [TestMethod]
    public void CrewWinnetou()
    {
        Crew(WinnetouLink, out var crewMatches, out var crewList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(14, crewMatches.Count, "crewMatches.Count");
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
        Assert.AreEqual(39, crewMatches.Count, "crewMatches.Count");
        Assert.AreEqual(82, progressBarMaxValue, "progressBarMaxValue");
        Assert.AreEqual(82, crewList.Count, "crewList.Count");
        Assert.AreEqual("Steven", crewList[44].FirstName, "crewList[44].FirstName");
        Assert.AreEqual("Avila", crewList[44].LastName, "crewList[44].LastName");
        Assert.AreEqual("Sound Designer", crewList[44].CreditSubtype, "crewList[44].CreditSubtype");
        Assert.AreEqual("Steven", crewList[45].FirstName, "crewList[45].FirstName");
        Assert.AreEqual("Avila", crewList[45].LastName, "crewList[45].LastName");
        Assert.AreEqual("Custom", crewList[45].CreditSubtype, "crewList[45].CreditSubtype");
        Assert.AreEqual("Trip", crewList[47].FirstName, "crewList[47].FirstName");
        Assert.AreEqual("Brock", crewList[47].LastName, "crewList[47].LastName");
        Assert.AreEqual("Sound Re-Recording Mixer", crewList[47].CreditSubtype, "crewList[47].CreditSubtype");
        Assert.AreEqual("Trip", crewList[48].FirstName, "crewList[48].FirstName");
        Assert.AreEqual("Brock", crewList[48].LastName, "crewList[48].LastName");
        Assert.AreEqual("Supervising Sound Editor", crewList[48].CreditSubtype, "crewList[48].CreditSubtype");
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
        Assert.AreEqual(46, crewMatches.Count, "castMatches.Count");
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

    [TestMethod]
    public void CrewKickAss()
    {
        ParseCrew(KickAssLink, out var crewMatches, out var crewList, out var defaultValues);

        var artCrewMatches = crewMatches
            .Where(m => m.Key.CreditType == "Art Direction by")
            .ToList();

        ProcessCrewLine(KickAssLink, artCrewMatches, crewList, out var progressBarMaxValue, out var existing, out var current, defaultValues);

        const string SarahBicknellLink = "nm0999406";

        var sarahMatch = crewMatches
            .FirstOrDefault(m => m.Key.CreditType == "Art Direction by").Value?
            .FirstOrDefault(m => m.Link == SarahBicknellLink);

        Assert.IsNotNull(sarahMatch);
        Assert.AreEqual("(as Sarah Stuart)", sarahMatch.Credit);

        var sarahCrew = crewList.FirstOrDefault(c => c.PersonLink == SarahBicknellLink);

        Assert.IsNotNull(sarahCrew);
        Assert.IsNull(sarahCrew.CustomRole);
        Assert.AreEqual("Sarah Stuart", sarahCrew.CreditedAs);
        Assert.AreEqual("(as Sarah Stuart)", sarahCrew.OriginalCredit);
        Assert.AreEqual("Art", sarahCrew.CreditType);
        Assert.AreEqual("Art Director", sarahCrew.CreditSubtype);

        var makeupDepartmentMatches = crewMatches
            .Where(m => m.Key.CreditType == "Makeup Department")
            .ToList();

        ProcessCrewLine(KickAssLink, makeupDepartmentMatches, crewList, out progressBarMaxValue, out existing, out current, defaultValues);

        const string AmberChaseLink = "nm0153681";

        var amberMatch = crewMatches
            .FirstOrDefault(m => m.Key.CreditType == "Makeup Department").Value?
            .FirstOrDefault(m => m.Link == AmberChaseLink);

        Assert.IsNotNull(amberMatch);
        Assert.AreEqual("assistant makeup artist", amberMatch.Credit);

        var amberCrew = crewList.FirstOrDefault(c => c.PersonLink == AmberChaseLink);

        Assert.IsNotNull(amberCrew);
        Assert.AreEqual("Assistant Makeup Artist", amberCrew.CustomRole);
        Assert.AreEqual("", amberCrew.CreditedAs);
        Assert.AreEqual("assistant makeup artist", amberCrew.OriginalCredit);
        Assert.AreEqual("Art", amberCrew.CreditType);
        Assert.AreEqual("Custom", amberCrew.CreditSubtype);
    }

    internal static void Crew(string key
        , out List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches
        , out List<CrewInfo> crewList
        , out int progressBarMaxValue
        , out FileInfo existing
        , out FileInfo current)
    {
        ParseCrew(key, out crewMatches, out crewList, out var defaultValues);

        ProcessCrewLine(key, crewMatches, crewList, out progressBarMaxValue, out existing, out current, defaultValues);
    }

    private static void ParseCrew(string key
        , out List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches
        , out List<CrewInfo> crewList
        , out DefaultValues defaultValues)
    {
        defaultValues = new DefaultValues()
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
    }

    private static void ProcessCrewLine(string key, List<KeyValuePair<CreditTypeMatch, List<CrewMatch>>> crewMatches, List<CrewInfo> crewList, out int progressBarMaxValue, out FileInfo existing, out FileInfo current, DefaultValues defaultValues)
    {
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