namespace RJCP.App.DltDump.Application
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class FilterAppTest
    {
        // Most of the functionality of FilterApp is tested through the FilterCommandTest. This is because it takes a
        // CmdOptions as input which requires the CommandLine.Run() method to be called. Anything that can't be tested
        // via an integration test can be tested additionally here.

        [Test]
        public void NullConfig()
        {
            Assert.That(() => { _ = new FilterApp(null); }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
