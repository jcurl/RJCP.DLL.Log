namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class BoolDltArgTest
    {
        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public void BoolToString(bool value, string output)
        {
            BoolDltArg boolArg = new(value);
            Assert.That(boolArg.ToString(), Is.EqualTo(output));

            StringBuilder sb = new();
            Assert.That(boolArg.Append(sb).ToString(), Is.EqualTo(output));
        }
    }
}
