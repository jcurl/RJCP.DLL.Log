namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class NonVerboseArgTest
    {
        [Test]
        public void SmallNonVerboseArg()
        {
            NonVerboseDltArg arg = new(new byte[] { 0x01, 0x02, 0x03 });
            Assert.That(arg.Data, Has.Length.EqualTo(3));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x01, 0x02, 0x03 }));
            Assert.That(arg.ToString(), Is.EqualTo("---|01 02 03"));

            StringBuilder sb = new();
            Assert.That(arg.Append(sb).ToString(), Is.EqualTo("---|01 02 03"));
        }

        [Test]
        public void SmallNonVerboseArgWithAscii()
        {
            NonVerboseDltArg arg = new(new byte[] { 0x65, 0x32, 0x0A });
            Assert.That(arg.Data, Has.Length.EqualTo(3));
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x65, 0x32, 0x0a }));
            Assert.That(arg.ToString(), Is.EqualTo("e2-|65 32 0a"));

            StringBuilder sb = new();
            Assert.That(arg.Append(sb).ToString(), Is.EqualTo("e2-|65 32 0a"));
        }

        [Test]
        public void LargeNonVerboseArg()
        {
            byte[] payload = new byte[65000];
            new Random().NextBytes(payload);

            NonVerboseDltArg arg = new(payload);
            Assert.That(arg.Data, Has.Length.EqualTo(payload.Length));
            Assert.That(arg.Data, Is.EqualTo(payload));
            Assert.That(arg.ToString(), Is.Not.Empty);

            StringBuilder sb = new();
            Assert.That(arg.Append(sb).ToString(), Is.Not.Empty);
        }
    }
}
