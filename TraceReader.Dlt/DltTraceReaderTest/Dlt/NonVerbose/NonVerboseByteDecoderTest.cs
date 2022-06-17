namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    public class NonVerboseByteDecoderTest : NonVerboseByteDecoderTestBase
    {
        public NonVerboseByteDecoderTest(DecoderType decoderType, Endianness endian) : base(decoderType, endian) { }

        [Test]
        public void DecodeNonVerboseBytes()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x12 } :
                new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x12 };

            Decode(payload, "NonVerbose", 1, out DltNonVerboseTraceLine line);
            Assert.That(line.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
            NonVerboseDltArg arg = (NonVerboseDltArg)line.Arguments[0];
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x00, 0x12 }));
            Assert.That(arg.ToString(), Is.EqualTo("--|00 12"));
            Assert.That(line.Text, Is.EqualTo("[1] --|00 12"));
        }

        [Test]
        public void DecodeNonVerboseBytesLargeIdentifier()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x00, 0x12 } :
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFE, 0x00, 0x12 };

            Decode(payload, "NonVerbose", -2, out DltNonVerboseTraceLine line);
            Assert.That(line.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
            NonVerboseDltArg arg = (NonVerboseDltArg)line.Arguments[0];
            Assert.That(arg.Data, Is.EqualTo(new byte[] { 0x00, 0x12 }));
            Assert.That(arg.ToString(), Is.EqualTo("--|00 12"));
            Assert.That(line.Text, Is.EqualTo("[4294967294] --|00 12"));
        }

        [Test]
        public void DecodeNonVerboseEmpty()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x01, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x01 };

            Decode(payload, "NonVerboseZeroLength", 1, out DltNonVerboseTraceLine line);
            Assert.That(line.Arguments[0], Is.TypeOf<NonVerboseDltArg>());
            NonVerboseDltArg arg = (NonVerboseDltArg)line.Arguments[0];
            Assert.That(arg.Data, Is.EqualTo(Array.Empty<byte>()));
            Assert.That(arg.ToString(), Is.EqualTo(string.Empty));
            Assert.That(line.Text, Is.EqualTo("[1] "));
        }
    }
}
