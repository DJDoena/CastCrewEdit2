using System.IO;
using System.Text;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;


[TestClass]
public class GoofTests : TestBase
{
    private const string YojimboLink = "tt0055630";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(IMDbParser.TitleUrl, YojimboLink, "goofs");
    }

    [TestMethod]
    public void Yojimbo()
    {
        Goofs(YojimboLink);
    }

    private static void Goofs(string key)
    {
        Program.Settings.DefaultValues.DownloadGoofs = true;

        var goofsUrl = $"{IMDbParser.TitleUrl}{key}/goofs";

        var goofs = GoofsParser.ParseGoofs(goofsUrl).ToString();

        using var sw = new StreamWriter(@"Current\" + key + ".goofs.txt", false, Encoding.UTF8);

        sw.Write(goofs);

        using var sr = new StreamReader(@"Existing\" + key + ".goofs.txt", Encoding.UTF8);

        string existing;

        existing = sr.ReadToEnd();
        Assert.AreEqual(existing.Length, goofs.Length, "goofsTextBox.Text.Length");
    }
}