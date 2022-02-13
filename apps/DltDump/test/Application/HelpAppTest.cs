namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Collections.Generic;
    using System.Text;
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
                Assert.That(global.StdOut.Lines.Count, Is.GreaterThanOrEqualTo(8));
            }
        }

        [Test]
        public void GetHelp()
        {
            using (TestApplication global = new TestApplication()) {
                HelpApp.ShowHelp();

                global.WriteStd();
                Assert.That(global.StdOut.Lines.Count, Is.GreaterThanOrEqualTo(18));
            }
        }
    }
}
