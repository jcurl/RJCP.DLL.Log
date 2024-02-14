namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
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
            StringDltArg stringArg = new(input);
            Assert.That(stringArg.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(stringArg.Append(sb).ToString(), Is.EqualTo(output));
        }

        [Test]
        public void Coding()
        {
            StringDltArg stringArg = new("string", StringEncodingType.Ascii);
            Assert.That(stringArg.ToString(), Is.EqualTo("string"));
            Assert.That(stringArg.Coding, Is.EqualTo(StringEncodingType.Ascii));

            StringBuilder sb = new();
            Assert.That(stringArg.Append(sb).ToString(), Is.EqualTo("string"));
        }
    }
}
