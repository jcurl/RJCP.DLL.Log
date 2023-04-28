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
    public class NonVerboseBinArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseBinArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_BIN8", 0))
            .Add(1, new TestPdu("S_BIN8", 1))
            .Add(2, new TestPdu("S_BIN16", 2))
            .Add(3, new TestPdu("S_BIN32", 4))
            .Add(4, new TestPdu("S_BIN64", 8))
            .Add(5, new TestPdu("S_BIN64", 10));

        public NonVerboseBinArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override NonVerboseBinArgDecoder CreateArgDecoder(int messageId)
        {
            string decoderType = Map.GetFrame(messageId, null, null, null).Arguments[0].PduType;
            switch (decoderType) {
            case "S_BIN8": return new NonVerboseBinArgDecoder(1);
            case "S_BIN16": return new NonVerboseBinArgDecoder(2);
            case "S_BIN32": return new NonVerboseBinArgDecoder(4);
            case "S_BIN64": return new NonVerboseBinArgDecoder(8);
            default: throw new NotImplementedException();
            }
        }

        [Test]
        public void DecodeBin8bit()
        {
            byte[] payload = new byte[] { 0x40 };

            Decode(1, payload, $"nv_Bin_8bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BinaryIntDltArg>());

            BinaryIntDltArg arg = (BinaryIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo($"0b0100 0000"));
            Assert.That(arg.Data, Is.EqualTo(64));
            Assert.That(arg.DataBytesLength, Is.EqualTo(1));
        }

        [Test]
        public void DecodeBin16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x34, 0x12 } :
                new byte[] { 0x12, 0x34 };

            Decode(2, payload, "nv_Bin_16bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BinaryIntDltArg>());

            BinaryIntDltArg arg = (BinaryIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b0001 0010 0011 0100"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
            Assert.That(arg.DataBytesLength, Is.EqualTo(2));
        }

        [Test]
        public void DecodeBin32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x78, 0x56, 0x34, 0x12 };

            Decode(3, payload, "nv_Bin_32bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BinaryIntDltArg>());

            BinaryIntDltArg arg = (BinaryIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b0111 1000 0101 0110 0011 0100 0001 0010"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
            Assert.That(arg.DataBytesLength, Is.EqualTo(4));
        }

        [Test]
        public void DecodeBin64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(4, payload, "nv_Bin_64bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BinaryIntDltArg>());

            BinaryIntDltArg arg = (BinaryIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b1000 1001 0110 0111 0100 0101 0010 0011 0000 0001 1110 1111 1100 1101 1010 1011"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataBytesLength, Is.EqualTo(8));
        }

        [Test]
        public void DecodeBin64Bit_Long()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0xEF, 0xAB } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB, 0xAB, 0xEF };

            Decode(5, payload, "nv_Bin_64bit_Long", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BinaryIntDltArg>());

            BinaryIntDltArg arg = (BinaryIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b1000 1001 0110 0111 0100 0101 0010 0011 0000 0001 1110 1111 1100 1101 1010 1011"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataBytesLength, Is.EqualTo(8));
        }

        [Test]
        public void DecodeBinNoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_Bin_Invalid");
        }

        [Test]
        public void DecodeBinPduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_Bin_InvalidPduZero");
        }

        [Test]
        public void DecodeBinPduInvalidPdu()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(2, payload, "nv_Bin_InvalidPdu");
        }
    }
}
