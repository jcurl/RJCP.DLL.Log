namespace RJCP.App.DltDump.Application
{
    using System;
    using NUnit.Framework;
    using Infrastructure.Terminal;

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
            using (TestApplication global = new TestApplication()) {
                ((VirtualTerminal)Global.Instance.Terminal).TerminalWidth = 132;
                VersionApp.ShowSimpleVersion();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(1));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(VersionApp.GetVersion()));
            }
        }

        [Test]
        public void GetVersion()
        {
            using (TestApplication global = new TestApplication()) {
                ((VirtualTerminal)Global.Instance.Terminal).TerminalWidth = 132;
                VersionApp.ShowVersion();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.EqualTo(4));
                Assert.That(global.StdOut.Lines[0], Is.EqualTo(VersionApp.GetVersion()));
            }
        }
    }
}
