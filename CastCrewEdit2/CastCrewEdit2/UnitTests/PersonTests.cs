using System.IO;
using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public class PersonTests : TestBase
{
    private const string EmmaWatson = "nm0914612";
    private const string AlexanderRhodes = "nm4659673";
    //private const string Tamara = "nm0848453";

    [ClassInitialize]
    public static void ClassInitialize(TestContext _)
    {
        CreateMockWebResponse(PersonLinkParser.PersonUrl, EmmaWatson);
        CreateMockWebResponse(PersonLinkParser.PersonUrl, AlexanderRhodes);
        //CreateMockWebResponse(IMDbParser.PersonUrl, Tamara);
    }

    [TestMethod]
    public void PersonWithHeadshotEmmaWatson()
    {
        var fileInfo = GetHeadshot(EmmaWatson);
        Assert.IsNotNull(fileInfo, "fileInfo");
        Assert.IsTrue(fileInfo.Exists, "fileInfo.Exists");
    }

    [TestMethod]
    public void PersonWithoutHeadshotAlexanderRhodes()
    {
        var fileInfo = GetHeadshot(AlexanderRhodes);
        Assert.IsNull(fileInfo, "FileInfo is not null!");
    }

    [TestMethod]
    public void PersonWithBirthYearEmmaWatson()
    {
        PersonInfo personInfo;

        personInfo = new PersonInfo()
        {
            PersonLink = EmmaWatson,
        };
        BirthYearParser.GetBirthYear(personInfo);
        Assert.AreEqual("1990", personInfo.BirthYear, "personInfo.BirthYear");
    }

    //[TestMethod]
    //public void PersonWithCircaBirthYearTamara()
    //{
    //    PersonInfo personInfo;

    //    personInfo = new PersonInfo();
    //    personInfo.PersonLink = Tamara;
    //    IMDbParser.GetBirthYear(personInfo);
    //    Assert.AreEqual("1910", personInfo.BirthYear, "personInfo.BirthYear");
    //}


    private static FileInfo GetHeadshot(string key)
    {
        var personInfo = new PersonInfo()
        {
            PersonLink = key
        };

        var fileInfo = HeadshotParser.GetHeadshot(personInfo);

        return fileInfo;
    }
}
