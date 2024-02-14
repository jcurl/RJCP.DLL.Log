namespace RJCP.App.DltDump.Application
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VersionAppTest
    {
        [Test]
        public void GetVersionString()
        {
            using (new TestApplication()) {
                string version = VersionApp.GetVersion();
                Console.WriteLine("Version: {0}", version);

                Assert.That(version, Is.Not.Null);
                Assert.That(version, Is.Not.Empty);
            }
        }

        [Test]
        public void GetSimpleVersion()
        {
            using (TestApplication global = new()) {
                global.Terminal.Width = 132;
                VersionApp.ShowSimpleVersion();

                global.WriteStd();
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(1));
                Assert.That(global.Terminal.StdOutLines[0], Is.EqualTo(VersionApp.GetVersion()));
            }
        }

        [Test]
        public void GetVersion()
        {
            using (TestApplication global = new()) {
                global.Terminal.Width = 132;
                VersionApp.ShowVersion();

                global.WriteStd();
                Assert.That(global.Terminal.StdOutLines, Has.Count.EqualTo(4));
                Assert.That(global.Terminal.StdOutLines[0], Is.EqualTo(VersionApp.GetVersion()));
            }
        }
    }
}
