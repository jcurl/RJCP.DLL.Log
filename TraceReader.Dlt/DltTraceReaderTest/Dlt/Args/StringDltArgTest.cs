namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using NUnit.Framework;

    [TestFixture]
    class StringDltArgTest
    {
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("a", "a")]
        [TestCase("abc DEF", "abc DEF")]
        public void StringToString(string input, string output)
        {
            StringDltArg stringArg = new StringDltArg(input);
            Assert.That(stringArg.ToString(), Is.EqualTo(output));
        }

        [Test]
        public void Coding()
        {
            StringDltArg stringArg = new StringDltArg("string", StringEncodingType.Ascii);
            Assert.That(stringArg.ToString(), Is.EqualTo("string"));
            Assert.That(stringArg.Coding, Is.EqualTo(StringEncodingType.Ascii));
        }
    }
}
