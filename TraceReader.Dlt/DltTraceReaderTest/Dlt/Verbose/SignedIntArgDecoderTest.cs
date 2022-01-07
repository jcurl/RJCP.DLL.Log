namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System.Globalization;
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class SignedIntArgDecoderTest : VerboseDecoderTestBase<SignedIntArgDecoder>
    {
        public SignedIntArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian)
        { }

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSignedInt8bit(byte value, int result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x21, 0x00, 0x00, 0x00, value } :
                new byte[] { 0x00, 0x00, 0x00, 0x21, value };

            Decode(payload, $"SignedInt_8bit_{value:x2}", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(result.ToString(CultureInfo.InvariantCulture)));
            Assert.That(arg.Data, Is.EqualTo(result));
        }

        [Test]
        public void DecodeSignedInt16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x22, 0x00, 0x00, 0x00, 0x34, 0x12 } :
                new byte[] { 0x00, 0x00, 0x00, 0x22, 0x12, 0x34 };

            Decode(payload, "SignedInt_16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("4660"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
        }

        [Test]
        public void DecodeSignedInt32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x23, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x00, 0x00, 0x00, 0x23, 0x78, 0x56, 0x34, 0x12 };

            Decode(payload, "SignedInt_32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("2018915346"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
        }

        [Test]
        public void DecodeSignedInt64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x24, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x00, 0x00, 0x00, 0x24, 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(payload, "SignedInt_64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-8545785751253561941"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
        }

        [TestCase(0x40, 64)]
        [TestCase(0x80, -128)]
        [TestCase(0xFF, -1)]
        public void DecodeSignedIntHex8bit(byte value, int result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x21, 0x00, 0x01, 0x00, value } :
                new byte[] { 0x00, 0x01, 0x00, 0x21, value };

            Decode(payload, $"SignedIntHex_8bit_{value:x2}", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(result.ToString(CultureInfo.InvariantCulture)));
            Assert.That(arg.Data, Is.EqualTo(result));
        }

        [Test]
        public void DecodeSignedIntHex16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x22, 0x00, 0x01, 0x00, 0x34, 0x12 } :
                new byte[] { 0x00, 0x01, 0x00, 0x22, 0x12, 0x34 };

            Decode(payload, "SignedIntHex_16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("4660"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
        }

        [Test]
        public void DecodeSignedIntHex32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x23, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x00, 0x01, 0x00, 0x23, 0x78, 0x56, 0x34, 0x12 };

            Decode(payload, "SignedIntHex_32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("2018915346"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
        }

        [Test]
        public void DecodeSignedIntHex64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x24, 0x00, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x00, 0x01, 0x00, 0x24, 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(payload, "SignedIntHex_64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<SignedIntDltArg>());
            SignedIntDltArg arg = (SignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-8545785751253561941"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
        }

        [Test]
        public void DecodeSignedInt128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x25, 0x00, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x25, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x00, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            Decode(payload, "SignedInt_128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeSignedIntHex128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x25, 0x00, 0x01, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF } :
                new byte[] { 0x00, 0x01, 0x00, 0x25, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x00, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            Decode(payload, "SignedIntHex_128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [TestCase(0x20, 0x00, TestName = "DecodeSignedIntZeroLength")]
        [TestCase(0x27, 0x00, TestName = "DecodeSignedUnknownLength")]
        public void DecodeSignedIntInvalid(byte typeInfo, byte cod1)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { typeInfo, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, typeInfo };

            DecodeIsInvalid(payload, $"SignedInt_Invalid_{typeInfo:x2}");
        }
    }
}
