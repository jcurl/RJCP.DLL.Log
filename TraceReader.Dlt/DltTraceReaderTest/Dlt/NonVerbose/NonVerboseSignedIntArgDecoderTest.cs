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
    public class NonVerboseSignedIntArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseSignedIntArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_SINT8", 0))
            .Add(1, new TestPdu("S_SINT8", 1))
            .Add(2, new TestPdu("S_SINT16", 2))
            .Add(3, new TestPdu("S_SINT32", 4))
            .Add(4, new TestPdu("S_SINT64", 8))
            .Add(5, new TestPdu("S_SINT64", 10));

        public NonVerboseSignedIntArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        protected override NonVerboseSignedIntArgDecoder CreateArgDecoder(int messageId)
        {
            string decoderType = Map.GetFrame(messageId, null, null, null).Arguments[0].PduType;
            switch (decoderType) {
            case "S_SINT8": return new NonVerboseSignedIntArgDecoder(1);
            case "S_SINT16": return new NonVerboseSignedIntArgDecoder(2);
            case "S_SINT32": return new NonVerboseSignedIntArgDecoder(4);
            case "S_SINT64": return new NonVerboseSignedIntArgDecoder(8);
            default: throw new NotImplementedException();
            }
        }

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSignedInt8bit(byte value, int result)
        {
            byte[] payload = new byte[] { value };

            Decode(1, payload, $"nv_SignedInt_8bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<SignedIntDltArg>());

            SignedIntDltArg arg = (SignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo(result.ToString(CultureInfo.InvariantCulture)));
            Assert.That(arg.Data, Is.EqualTo(result));
        }

        [Test]
        public void DecodeSignedInt16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x34, 0x12 } :
                new byte[] { 0x12, 0x34 };

            Decode(2, payload, "nv_SignedInt_16bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<SignedIntDltArg>());

            SignedIntDltArg arg = (SignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("4660"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
        }

        [Test]
        public void DecodeSignedInt32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x78, 0x56, 0x34, 0x12 };

            Decode(3, payload, "nv_SignedInt_32bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<SignedIntDltArg>());

            SignedIntDltArg arg = (SignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("2018915346"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
        }

        [Test]
        public void DecodeSignedInt64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(4, payload, "nv_SignedInt_64bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<SignedIntDltArg>());

            SignedIntDltArg arg = (SignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-8545785751253561941"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
        }

        [Test]
        public void DecodeSignedInt64bit_Long()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89, 0x00, 0x00 } :
                new byte[] { 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB, 0x00, 0x00 };

            Decode(5, payload, "nv_SignedInt_64bit_Long", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<SignedIntDltArg>());

            SignedIntDltArg arg = (SignedIntDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-8545785751253561941"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
        }

        [Test]
        public void DecodeSignedIntNoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_SignedInt_Invalid");
        }

        [Test]
        public void DecodeSignedIntPduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_SignedInt_InvalidPduZero");
        }

        [Test]
        public void DecodeSignedIntPduInvalidPdu()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(2, payload, "nv_SignedInt_InvalidPdu");
        }
    }
}
