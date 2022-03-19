namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class RawDltArgTest
    {
        [Test]
        public void EmptyArray()
        {
            byte[] value = Array.Empty<byte>();
            RawDltArg rawArg = new RawDltArg(value);
            Assert.That(rawArg.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NullArray()
        {
            RawDltArg rawDltArg = new RawDltArg(null);
            Assert.That(rawDltArg.ToString(), Is.EqualTo(string.Empty));
        }

        [TestCase(0x00, "00")]
        [TestCase(0x01, "01")]
        [TestCase(0x7F, "7f")]
        [TestCase(0x80, "80")]
        [TestCase(0xCD, "cd")]
        [TestCase(0xFF, "ff")]
        public void OneElement(byte value, string output)
        {
            byte[] arg = { value };
            RawDltArg rawArg = new RawDltArg(arg);
            Assert.That(rawArg.ToString(), Is.EqualTo(output));
        }

        [TestCase(0x00, 0x00, "00 00")]
        [TestCase(0x01, 0x00, "01 00")]
        [TestCase(0x7F, 0xff, "7f ff")]
        [TestCase(0x80, 0x7f, "80 7f")]
        [TestCase(0xCD, 0x12, "cd 12")]
        [TestCase(0xFF, 0xFE, "ff fe")]
        public void TwoElements(byte v1, byte v2, string output)
        {
            byte[] arg = { v1, v2 };
            RawDltArg rawArg = new RawDltArg(arg);
            Assert.That(rawArg.ToString(), Is.EqualTo(output));
        }

        [Test]
        public void LongArray()
        {
            byte[] data = new byte[256];
            new Random().NextBytes(data);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                if (i != 0) builder.Append(' ');
                builder.AppendFormat("{0:x2}", data[i]);
            }

            RawDltArg rawArg = new RawDltArg(data);
            Assert.That(rawArg.ToString(), Is.EqualTo(builder.ToString()));
        }
    }
}
