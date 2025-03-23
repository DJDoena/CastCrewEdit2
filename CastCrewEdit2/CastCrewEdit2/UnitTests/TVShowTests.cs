using System.Collections.Generic;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Forms;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public class TVShowTests : TestBase
{
    private const string FerdinandLink = "tt1307254";
    private const string FridayNightLightsLink = "tt0758745";
    private const string RootsLink = "tt4338588";
    private const string TrackerLink = "tt13875494";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(IMDbParser.TitleUrl, FerdinandLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, FridayNightLightsLink, "episodes?season=1");
        CreateMockWebResponse(IMDbParser.TitleUrl, RootsLink, "fullcredits");
        CreateMockWebResponse(IMDbParser.TitleUrl, TrackerLink, "episodes");
        CreateMockWebResponse(IMDbParser.TitleUrl, TrackerLink, "episodes?season=1");
    }

    [TestMethod]
    public void CrewFerdinand()
    {
        var episodeInfo = new EpisodeInfo
        {
            Link = FerdinandLink,
            SeasonNumber = "1",
            EpisodeNumber = "1",
        };
        Crew(episodeInfo);
        Assert.AreEqual(20, episodeInfo.CrewMatches.Count, "episodeInfo.CrewMatches.Count");
        Assert.AreEqual(32, episodeInfo.CrewList.Count, "episodeInfo.CrewList.Count");
    }

    [TestMethod]
    public void CastFerdinand()
    {
        var episodeInfo = new EpisodeInfo
        {
            Link = FerdinandLink,
            SeasonNumber = "1",
            EpisodeNumber = "1",
        };
        Cast(episodeInfo);
        Assert.AreEqual(30, episodeInfo.CastMatches.Count, "episodeInfo.CastMatches.Count");
        Assert.AreEqual(30, episodeInfo.CastList.Count, "episodeInfo.CastList.Count");
        Assert.AreEqual("True", episodeInfo.CastList[29].Uncredited, "episodeInfo.CastList[29].Uncredited");
    }

    [TestMethod]
    public void CrewRoots()
    {
        var episodeInfo = new EpisodeInfo
        {
            Link = RootsLink,
            SeasonNumber = "1",
            EpisodeNumber = "1",
        };
        Crew(episodeInfo);
        Assert.AreEqual(104, episodeInfo.CrewMatches.Count, "episodeInfo.CrewMatches.Count");
        Assert.AreEqual(153, episodeInfo.CrewList.Count, "episodeInfo.CrewList.Count");
    }

    private static void Crew(EpisodeInfo episodeInfo)
    {
        Program.Settings.DefaultValues.ParseCrew = true;
        Program.Settings.DefaultValues.CapitalizeCustomRole = true;
        Program.Settings.DefaultValues.CreditTypeArt = true;
        Program.Settings.DefaultValues.CreditTypeCinematography = true;
        Program.Settings.DefaultValues.CreditTypeDirection = true;
        Program.Settings.DefaultValues.CreditTypeFilmEditing = true;
        Program.Settings.DefaultValues.CreditTypeMusic = true;
        Program.Settings.DefaultValues.CreditTypeOther = true;
        Program.Settings.DefaultValues.CreditTypeProduction = true;
        Program.Settings.DefaultValues.CreditTypeSound = true;
        Program.Settings.DefaultValues.CreditTypeSoundtrack = true;
        Program.Settings.DefaultValues.CreditTypeWriting = true;
        Program.Settings.DefaultValues.IncludeCustomCredits = true;
        Program.Settings.DefaultValues.RetainCrewCreditedAs = true;
        Program.Settings.DefaultValues.RetainOriginalCredit = true;
        Program.Settings.DefaultValues.CheckPersonLinkForRedirect = false;

        Parse(episodeInfo);
    }

    private static void Parse(EpisodeInfo episodeInfo)
    {
        var episodes = new List<EpisodeInfo>(1)
        {
            episodeInfo,
        };

        using var episodesForm = new EpisodesForm(episodes);

        episodesForm.ParseIMDb(episodeInfo);
    }

    private static void Cast(EpisodeInfo episodeInfo)
    {
        Program.Settings.DefaultValues.ParseCast = true;
        Program.Settings.DefaultValues.IgnoreArchiveFootage = false;
        Program.Settings.DefaultValues.IgnoreCreditOnly = false;
        Program.Settings.DefaultValues.IgnoreLanguageVersion = false;
        Program.Settings.DefaultValues.IgnoreScenesDeleted = false;
        Program.Settings.DefaultValues.IgnoreUnconfirmed = false;
        Program.Settings.DefaultValues.IgnoreUncredited = false;
        Program.Settings.DefaultValues.ParseRoleSlash = true;
        Program.Settings.DefaultValues.ParseVoiceOf = true;
        Program.Settings.DefaultValues.RetainCastCreditedAs = true;
        //Program.Settings.DefaultValues.ParseCrew = true;
        //Program.Settings.DefaultValues.CapitalizeCustomRole = true;
        //Program.Settings.DefaultValues.CreditTypeArt = true;
        //Program.Settings.DefaultValues.CreditTypeCinematography = true;
        //Program.Settings.DefaultValues.CreditTypeDirection = true;
        //Program.Settings.DefaultValues.CreditTypeFilmEditing = true;
        //Program.Settings.DefaultValues.CreditTypeMusic = true;
        //Program.Settings.DefaultValues.CreditTypeOther = true;
        //Program.Settings.DefaultValues.CreditTypeProduction = true;
        //Program.Settings.DefaultValues.CreditTypeSound = true;
        //Program.Settings.DefaultValues.CreditTypeSoundtrack = true;
        //Program.Settings.DefaultValues.CreditTypeWriting = true;
        //Program.Settings.DefaultValues.IncludeCustomCredits = true;
        //Program.Settings.DefaultValues.RetainCrewCreditedAs = true;
        //Program.Settings.DefaultValues.RetainOriginalCredit = true;
        Program.Settings.DefaultValues.CheckPersonLinkForRedirect = false;

        Parse(episodeInfo);
    }
}
