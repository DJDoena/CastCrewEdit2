using System.IO;
using System.Text;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;


[TestClass]
public class TriviaTests : TestBase
{
    private const string YojimboLink = "tt0055630";
    private const string GreaseLink = "tt0077631";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(IMDbParser.TitleUrl, YojimboLink, "trivia");
        CreateMockWebResponse(IMDbParser.TitleUrl, GreaseLink, "trivia");
    }

    [TestMethod]
    public void Yojimbo()
    {
        Trivia(YojimboLink);
    }

    [TestMethod]
    public void Grease()
    {
        Trivia(GreaseLink);
    }

    private static void Trivia(string key)
    {
        Program.Settings.DefaultValues.DownloadTrivia = true;

        var triviaUrl = $"{IMDbParser.TitleUrl}{key}/trivia";

        var trivia = TriviaParser.ParseTrivia(triviaUrl).ToString();

        using var sw = new StreamWriter(@"Current\" + key + ".trivia.txt", false, Encoding.UTF8);

        sw.Write(trivia);

        using var sr = new StreamReader(@"Existing\" + key + ".trivia.txt", Encoding.UTF8);

        string existing;

        existing = sr.ReadToEnd();
        Assert.AreEqual(existing.Length, trivia.Length, "triviaTextBox.Text.Length");
    }
}