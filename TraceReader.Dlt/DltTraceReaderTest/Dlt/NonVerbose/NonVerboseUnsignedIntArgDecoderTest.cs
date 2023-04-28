namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using System.Globalization;
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class NonVerboseUnsignedIntArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseUnsignedIntArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_UINT8", 0))
            .Add(1, new TestPdu("S_UINT8", 1))
            .Add(2, new TestPdu("S_UINT16", 2))
            .Add(3, new TestPdu("S_UINT32", 4))
            .Add(4, new TestPdu("S_UINT64", 8))
            .Add(5, new TestPdu("S_UINT64", 10));

        public NonVerboseUnsignedIntArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override NonVerboseUnsignedIntArgDecoder CreateArgDecoder(int messageId)
        {
            string decoderType = Map.GetFrame(messageId, null, null, null).Arguments[0].PduType;
            switch (decoderType) {
            case "S_UINT8": return new NonVerboseUnsignedIntArgDecoder(1);
            case "S_UINT16": return new NonVerboseUnsignedIntArgDecoder(2);
            case "S_UINT32": return new NonVerboseUnsignedIntArgDecoder(4);
            case "S_UINT64": return new NonVerboseUnsignedIntArgDecoder(8);
            default: throw new NotImplementedException();
            }
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUInt8bit(byte value, uint result)
        {
            byte[] payload = new byte[] { value };

            Decode(1, payload, $"nv_UnsignedInt_8bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnsignedIntDltArg>());

            UnsignedIntDltArg arg = (UnsignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(result.ToString(CultureInfo.InvariantCulture)));
            Assert.That(arg.Data, Is.EqualTo(result));
            Assert.That(arg.DataUnsigned, Is.EqualTo(result));
        }

        [Test]
        public void DecodeUInt16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x34, 0x12 } :
                new byte[] { 0x12, 0x34 };

            Decode(2, payload, "nv_UnsignedInt_16bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnsignedIntDltArg>());

            UnsignedIntDltArg arg = (UnsignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("4660"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x1234));
        }

        [Test]
        public void DecodeUInt32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x78, 0x56, 0x34, 0x12 };

            Decode(3, payload, "nv_UnsignedInt_32bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnsignedIntDltArg>());

            UnsignedIntDltArg arg = (UnsignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("2018915346"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x78563412));
        }

        [Test]
        public void DecodeUInt64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(4, payload, "nv_UnsignedInt_64bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnsignedIntDltArg>());

            UnsignedIntDltArg arg = (UnsignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("9900958322455989675"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x8967452301EFCDAB));
        }

        [Test]
        public void DecodeUInt64bit_Long()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0x00, 0x00 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB, 0x00, 0x00 };

            Decode(5, payload, "nv_UnsignedInt_64bit_Long", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<UnsignedIntDltArg>());

            UnsignedIntDltArg arg = (UnsignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("9900958322455989675"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x8967452301EFCDAB));
        }

        [Test]
        public void DecodeUnsignedIntNoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_UnsignedInt_Invalid");
        }

        [Test]
        public void DecodeUnsignedIntPduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_UnsignedInt_InvalidPduZero");
        }

        [Test]
        public void DecodeUnsignedIntPduInvalidPdu()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(2, payload, "nv_UnsignedInt_InvalidPdu");
        }
    }
}
