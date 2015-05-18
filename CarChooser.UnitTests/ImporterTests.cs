using System.Globalization;
using DataImporter;
using NUnit.Framework;

namespace CarChooser.UnitTests
{
    [TestFixture]
    public class ImporterTests
    {
        [TestCase("320d", 0, 0, "320d")]
        [TestCase("320d (08 on)", 2008, 0, "320d")]
        [TestCase("320d (08 - 10)", 2008, 2010, "320d")]
        [TestCase("320d (08-10)", 2008, 2010, "320d")]
        [TestCase("320d (08-10)", 2008, 2010, "320d")]
        [TestCase("320d (98-99)", 1998, 1999, "320d")]
        public void ShouldExtractYearAndModel(string fullModelName, int expectedYearFrom, int expectedYearTo, string modelName)
        {
            var result = new ParkersRipper(null).GetModelAndYear(fullModelName);
            Assert.That(result.YearFrom == expectedYearFrom);
            Assert.That(result.YearTo == expectedYearTo);
            Assert.That(result.ModelName == modelName);
        }

        [Test]
        public void IgnoreSillyCharacters()
        {
            Assert.True(string.Compare("coupé", "coupe", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace)==0);
        }
    }


}
