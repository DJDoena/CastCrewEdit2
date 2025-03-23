using DoenaSoft.DVDProfiler.CastCrewEdit2.Helper.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoenaSoft.DVDProfiler.CastCrewEdit2.UnitTests;

[TestClass]
public class NameTests
{
    [TestMethod]
    public void StageNameSplit()
    {
        var fullName = "Bud Spencer & Terence Hill & Oliver Onions & DJ Doena";

        var name = NameParser.Parse(fullName, true).ToString();

        Assert.AreEqual("<Bud Spencer & Terence Hill & Oliver> [Onions & DJ Doena]", name, "name");
    }
}
