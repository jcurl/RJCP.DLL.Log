namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
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
    public class NonVerboseHexArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseHexArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_HEX8", 0))
            .Add(1, new TestPdu("S_HEX8", 1))
            .Add(2, new TestPdu("S_HEX16", 2))
            .Add(3, new TestPdu("S_HEX32", 4))
            .Add(4, new TestPdu("S_HEX64", 8))
            .Add(5, new TestPdu("S_HEX64", 10));

        public NonVerboseHexArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override NonVerboseHexArgDecoder CreateArgDecoder(int messageId)
        {
            string decoderType = Map.GetFrame(messageId, null, null, null).Arguments[0].PduType;
            switch (decoderType) {
            case "S_HEX8": return new NonVerboseHexArgDecoder(1);
            case "S_HEX16": return new NonVerboseHexArgDecoder(2);
            case "S_HEX32": return new NonVerboseHexArgDecoder(4);
            case "S_HEX64": return new NonVerboseHexArgDecoder(8);
            default: throw new NotImplementedException();
            }
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeHex8bit(byte value, uint result)
        {
            byte[] payload = new byte[] { value };

            Decode(1, payload, $"nv_Hex_8bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<HexIntDltArg>());

            HexIntDltArg arg = (HexIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo($"0x{value:x02}"));
            Assert.That(arg.Data, Is.EqualTo(value));
            Assert.That(arg.DataBytesLength, Is.EqualTo(1));
        }

        [Test]
        public void DecodeHex16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x34, 0x12 } :
                new byte[] { 0x12, 0x34 };

            Decode(2, payload, "nv_Hex_16bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<HexIntDltArg>());

            HexIntDltArg arg = (HexIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x1234"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
            Assert.That(arg.DataBytesLength, Is.EqualTo(2));
        }

        [Test]
        public void DecodeHex32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x78, 0x56, 0x34, 0x12 };

            Decode(3, payload, "nv_Hex_32bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<HexIntDltArg>());

            HexIntDltArg arg = (HexIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x78563412"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
            Assert.That(arg.DataBytesLength, Is.EqualTo(4));
        }

        [Test]
        public void DecodeHex64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(4, payload, "nv_Hex_64bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<HexIntDltArg>());

            HexIntDltArg arg = (HexIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x8967452301efcdab"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataBytesLength, Is.EqualTo(8));
        }

        [Test]
        public void DecodeHex64bit_Long()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0x00, 0x00 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB, 0x00, 0x00 };

            Decode(5, payload, "nv_Hex_64bit_Long", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<HexIntDltArg>());

            HexIntDltArg arg = (HexIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x8967452301efcdab"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataBytesLength, Is.EqualTo(8));
        }

        [Test]
        public void DecodeHexNoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_Hex_Invalid");
        }

        [Test]
        public void DecodeHexPduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_Hex_InvalidPduZero");
        }

        [Test]
        public void DecodeHexPduInvalidPdu()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(2, payload, "nv_Hex_InvalidPdu");
        }
    }
}
