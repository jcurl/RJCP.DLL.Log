namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using NUnit.Framework;

    [TestFixture]
    public class BoolDltArgTest
    {
        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void BoolToString(bool value, string output)
        {
            BoolDltArg boolArg = new BoolDltArg(value);
            Assert.That(boolArg.ToString(), Is.EqualTo(output));
        }
    }
}
