namespace RJCP.App.DltDump.Application
{
    using NUnit.Framework;

    [TestFixture]
    public class HelpAppTest
    {
        [Test]
        public void GetSimpleHelp()
        {
            using (TestApplication global = new TestApplication()) {
                HelpApp.ShowSimpleHelp();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.GreaterThanOrEqualTo(10));
            }
        }

        [Test]
        public void GetHelp()
        {
            using (TestApplication global = new TestApplication()) {
                HelpApp.ShowHelp();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.GreaterThanOrEqualTo(90));
            }
        }
    }
}
