namespace RJCP.App.DltDump.Application
{
    using NUnit.Framework;
    using RJCP.Core.CommandLine;

    [TestFixture]
    public class HelpAppTest
    {
        [Test]
        public void GetSimpleHelp()
        {
            using (TestApplication global = new TestApplication()) {
                Options cmdLine = Options.Parse(null, null);
                HelpApp.ShowSimpleHelp(cmdLine);

                global.WriteStd();
                Assert.That(global.Terminal.StdOutLines, Has.Count.GreaterThanOrEqualTo(10));
            }
        }

        [Test]
        public void GetHelp()
        {
            using (TestApplication global = new TestApplication()) {
                Options cmdLine = Options.Parse(null, null);
                HelpApp.ShowHelp(cmdLine);

                global.WriteStd();
                Assert.That(global.Terminal.StdOutLines, Has.Count.GreaterThanOrEqualTo(102));
            }
        }
    }
}
