namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class RawArgDecoderTest : VerboseDecoderTestBase<RawArgDecoder>
    {
        public RawArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian)
        { }

        [Test]
        public void RawArg()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x04, 0x00, 0x00, 0x08, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 } :
                new byte[] { 0x00, 0x00, 0x04, 0x00, 0x00, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

            Decode(payload, "RawArg", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<RawDltArg>());
            RawDltArg arg = (RawDltArg)verboseArg;
            Assert.That(arg.Data, Is.EqualTo(payload[6..]));
        }

        [Test]
        public void RawArgEmpty()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x04, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x04, 0x00, 0x00, 0x00 };

            Decode(payload, "RawArgEmpty", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<RawDltArg>());
            RawDltArg arg = (RawDltArg)verboseArg;
            Assert.That(arg.Data.Length, Is.EqualTo(0));
        }

        [Test]
        public void RawArgLarge()
        {
            byte[] payload = new byte[65000];
            Random r = new Random();
            if (Endian == Endianness.Little) {
                payload[0] = 0x00;
                payload[1] = 0x04;
                payload[2] = 0x00;
                payload[3] = 0x00;
                payload[4] = (byte)((payload.Length - 6) & 0xFF);
                payload[5] = (byte)((payload.Length - 6) >> 8);
            } else {
                payload[0] = 0x00;
                payload[1] = 0x00;
                payload[2] = 0x04;
                payload[3] = 0x00;
                payload[4] = (byte)((payload.Length - 6) >> 8);
                payload[5] = (byte)((payload.Length - 6) & 0xFF);
            }

            for (int i = 6; i < payload.Length; i++) {
                payload[i] = (byte)r.Next(0, 255);
            }

            Decode(payload, "RawArgLarge", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<RawDltArg>());
            RawDltArg arg = (RawDltArg)verboseArg;
            Assert.That(arg.Data, Is.EqualTo(payload[6..]));
        }
    }
}
