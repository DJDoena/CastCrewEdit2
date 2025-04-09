using System.Linq;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public class MovieCrewOldStyleTests : TestBase
{
    private const string ABiggerSplashLink = "tt2056771";

    [TestMethod]
    public void MyTestMethod()
    {
        const string FernandaPerezLink = "nm0673489";

        const string ABiggerSplashOldStyleLink = $"{ABiggerSplashLink}OS";

        WebSiteReader.WebResponses[$"https://www.imdb.com/title/{ABiggerSplashOldStyleLink}/fullcredits"] = new MockWebResponse($@"Existing\{ABiggerSplashOldStyleLink}.fullcredits.html.txt");

        MovieTests.Crew(ABiggerSplashOldStyleLink, out var crewMatches, out var crewList, out var progressBarMaxValue, out var existing, out var current);

        var fernandaMatch = crewMatches
            .FirstOrDefault(m => m.Key.CreditType == "Makeup Department").Value?
            .FirstOrDefault(m => m.Link == FernandaPerezLink);

        Assert.IsNotNull(fernandaMatch);
        Assert.AreEqual("makeup creator (as Fernanda Lucia Perez)", fernandaMatch.Credit);

        var fernandaCrew = crewList.FirstOrDefault(c => c.PersonLink == FernandaPerezLink);

        Assert.IsNotNull(fernandaCrew);
        Assert.AreEqual("Makeup Creator", fernandaCrew.CustomRole);
        Assert.AreEqual("Fernanda Lucia Perez", fernandaCrew.CreditedAs);
        Assert.AreEqual("makeup creator (as Fernanda Lucia Perez)", fernandaCrew.OriginalCredit);
        Assert.AreEqual("Art", fernandaCrew.CreditType);
        Assert.AreEqual("Custom", fernandaCrew.CreditSubtype);

        const string WalterFasanoLink = "nm0002576";

        var walterMatch = crewMatches
            .FirstOrDefault(m => m.Key.CreditType == "Editing by").Value?
            .FirstOrDefault(m => m.Link == WalterFasanoLink);

        Assert.IsNotNull(walterMatch);
        Assert.AreEqual("", walterMatch.Credit);

        var walterCrew = crewList.FirstOrDefault(c => c.PersonLink == WalterFasanoLink);

        Assert.IsNotNull(walterCrew);
        Assert.IsNull(walterCrew.CustomRole);
        Assert.IsNull(walterCrew.CreditedAs);
        Assert.AreEqual("", walterCrew.OriginalCredit);
        Assert.AreEqual("Film Editing", walterCrew.CreditType);
        Assert.AreEqual("Film Editor", walterCrew.CreditSubtype);

        const string YorickLeSauxLink = "nm0494617";

        var yorickMatch = crewMatches
            .FirstOrDefault(m => m.Key.CreditType == "Cinematography by").Value?
            .FirstOrDefault(m => m.Link == YorickLeSauxLink);

        Assert.IsNotNull(yorickMatch);
        Assert.AreEqual("(as Yorick LeSaux)", yorickMatch.Credit);

        var yorickCrew = crewList.FirstOrDefault(c => c.PersonLink == YorickLeSauxLink);

        Assert.IsNotNull(yorickCrew);
        Assert.IsNull(yorickCrew.CustomRole);
        Assert.AreEqual("Yorick LeSaux", yorickCrew.CreditedAs);
        Assert.AreEqual("(as Yorick LeSaux)", yorickCrew.OriginalCredit);
        Assert.AreEqual("Cinematography", yorickCrew.CreditType);
        Assert.AreEqual("Director of Photography", yorickCrew.CreditSubtype);
    }
}
