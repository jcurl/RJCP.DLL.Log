namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class NonVerboseArgTest
    {
        [Test]
        public void SmallNonVerboseArg()
        {
            NonVerboseDltArg arg = new NonVerboseDltArg(new byte[] { 0x01, 0x02, 0x03 });
            Assert.That(arg.Data.Length, Is.EqualTo(3));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x01, 0x02, 0x03 }));
            Assert.That(arg.ToString(), Is.EqualTo("---|01 02 03"));
        }

        [Test]
        public void SmallNonVerboseArgWithAscii()
        {
            NonVerboseDltArg arg = new NonVerboseDltArg(new byte[] { 0x65, 0x32, 0x0A });
            Assert.That(arg.Data.Length, Is.EqualTo(3));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x65, 0x32, 0x0a }));
            Assert.That(arg.ToString(), Is.EqualTo("e2-|65 32 0a"));
        }

        [Test]
        public void LargeNonVerboseArg()
        {
            byte[] payload = new byte[65000];
            new Random().NextBytes(payload);

            NonVerboseDltArg arg = new NonVerboseDltArg(payload);
            Assert.That(arg.Data.Length, Is.EqualTo(payload.Length));
            Assert.That(arg.Data, Is.EqualTo(payload));
            Assert.That(arg.ToString(), Is.Not.Empty);
        }
    }
}
