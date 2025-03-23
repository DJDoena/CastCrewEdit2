using System.Collections.Generic;
using System.IO;
using System.Linq;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Extended;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using DoenaSoft.DVDProfiler.DVDProfilerXML.Version400;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public class SoundtrackTests : TestBase
{
    private const string ThisIsSpinalTapLink = "tt0088258";
    //private const string RootsLink = "tt4338588";
    //private const string FerdinandLink = "tt1307254";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(IMDbParser.TitleUrl, ThisIsSpinalTapLink, "soundtrack");
        //CreateMockWebResponse(IMDbParser.TitleUrl, RootsLink, "soundtrack");
        //CreateMockWebResponse(IMDbParser.TitleUrl, FerdinandLink, "soundtrack");
    }

    [TestMethod]
    public void ThisIsSpinalTap()
    {
        Soundtrack(ThisIsSpinalTapLink, out var matches, out var crewList, out var progressBarMaxValue, out var existing, out var current);
        Assert.AreEqual(17, matches.Count, "matches.Count");
        Assert.AreEqual(73, progressBarMaxValue, "progressBarMaxValue");
        Assert.AreEqual(73, crewList.Count, "crewList.Count");
        Assert.AreEqual(existing.Length, current.Length, "current.Length");
    }

    private static void Soundtrack(string key
        , out Dictionary<string, List<SoundtrackMatch>> matches
        , out List<CrewInfo> crewList
        , out int progressBarMaxValue
        , out FileInfo existing, out FileInfo current)
    {
        matches = new Dictionary<string, List<SoundtrackMatch>>();

        SoundtrackParser.ParseSoundtrack(key, ref matches);

        Assert.IsNotNull(matches, "matches");

        var defaultValues = new DefaultValues()
        {
            CreditTypeSoundtrack = true,
            CheckPersonLinkForRedirect = false,
        };

        var progressMax = 0;
        crewList = new List<CrewInfo>(0);
        SoundtrackParser.ProcessSoundtrackLine(crewList, matches, defaultValues, () => progressMax++);
        var crewInformation = new CrewInformation()
        {
            Title = key,
            CrewList = crewList.Select(ConvertCrew).ToArray()
        };
        crewInformation.Serialize(@"Current\" + key + ".soundtrack.xml");
        existing = new FileInfo(@"Existing\" + key + ".soundtrack.xml");
        current = new FileInfo(@"Current\" + key + ".soundtrack.xml");

        progressBarMaxValue = progressMax;
    }
}
