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
                VersionApp.ShowSimpleVersion();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetVersion()
        {
            using (TestApplication global = new TestApplication()) {
                VersionApp.ShowVersion();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(3));
            }
        }
    }
}
