namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using NUnit.Framework;

    [TestFixture]
    public class SignedIntDltArgTest
    {
        [TestCase(-100, "-100")]
        [TestCase(0, "0")]
        [TestCase(100, "100")]
        [TestCase(256, "256")]
        [TestCase(byte.MaxValue, "255")]
        [TestCase(byte.MinValue, "0")]
        [TestCase(32768, "32768")]
        [TestCase(short.MaxValue, "32767")]
        [TestCase(short.MinValue, "-32768")]
        [TestCase(2147483648, "2147483648")]
        [TestCase(int.MaxValue, "2147483647")]
        [TestCase(int.MinValue, "-2147483648")]
        [TestCase(-2147483649, "-2147483649")]
        [TestCase(-9223372036854775807, "-9223372036854775807")]
        [TestCase(9223372036854775807, "9223372036854775807")]
        public void SignedIntToString(long value, string output)
        {
            SignedIntDltArg signedInt = new SignedIntDltArg(value);
            Assert.That(signedInt.ToString(), Is.EqualTo(output));
        }
    }
}
