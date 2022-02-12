namespace RJCP.App.DltDump.Application
{
    using NUnit.Framework;

    [TestFixture]
    public class VersionAppTest
    {
        [Test]
        public void GetSimpleVersion()
        {
            using (TestApplication global = new TestApplication()) {
                VersionApp app = new VersionApp();
                app.ShowSimpleVersion();

                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetVersion()
        {
            using (TestApplication global = new TestApplication()) {
                VersionApp app = new VersionApp();
                app.ShowVersion();

                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(3));
            }
        }
    }
}
