namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class UnknownNonVerboseDltArgTest
    {
        [Test]
        public void UnknownNonVerboseDltArg()
        {
            byte[] data = { 0x01, 0x23, 0x45, 0x67, 0x89 };
            UnknownNonVerboseDltArg arg = new(data);
            Assert.That(arg.Data, Is.EqualTo(data));
            Assert.That(arg.ToString(), Is.EqualTo("(05) 01 23 45 67 89"));

            StringBuilder sb = new();
            Assert.That(arg.Append(sb).ToString(), Is.EqualTo("(05) 01 23 45 67 89"));
        }

        [Test]
        public void UnknownNonVerboseDltArgNull()
        {
            UnknownNonVerboseDltArg arg = new(null);
            Assert.That(arg.Data, Is.Empty);
            Assert.That(arg.ToString(), Is.EqualTo("(00)"));

            StringBuilder sb = new();
            Assert.That(arg.Append(sb).ToString(), Is.EqualTo("(00)"));
        }

        [Test]
        public void UnknownNonVerboseDltArgEmpty()
        {
            UnknownNonVerboseDltArg arg = new(Array.Empty<byte>());
            Assert.That(arg.Data, Is.Empty);
            Assert.That(arg.ToString(), Is.EqualTo("(00)"));

            StringBuilder sb = new();
            Assert.That(arg.Append(sb).ToString(), Is.EqualTo("(00)"));
        }
    }
}
