using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public sealed class BirthYearTests : TestBase
{
    private const string JuneEllisLink = "nm0254889";
    private const string DreweHenleyLink = "nm0377120";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(PersonLinkParser.PersonUrl, JuneEllisLink);
        CreateMockWebResponse(PersonLinkParser.PersonUrl, DreweHenleyLink);
    }

    [TestMethod]
    public void JuneEllis()
    {
        var birthYear = BirthYearGetter.Get(JuneEllisLink);

        Assert.AreEqual("1926", birthYear);
    }

    [TestMethod]
    public void DreweHenley()
    {
        var birthYear = BirthYearGetter.Get(DreweHenleyLink);

        Assert.AreEqual("1940", birthYear);
    }
}
