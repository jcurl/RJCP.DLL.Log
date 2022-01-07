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
    public class UnsignedIntArgDecoderTest : VerboseDecoderTestBase<UnsignedIntArgDecoder>
    {
        public UnsignedIntArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian)
        { }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUInt8bit(byte value, uint result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x41, 0x00, 0x00, 0x00, value } :
                new byte[] { 0x00, 0x00, 0x00, 0x41, value };

            Decode(payload, $"UnsignedInt_8bit_{value:x2}", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo(result.ToString(CultureInfo.InvariantCulture)));
            Assert.That(arg.Data, Is.EqualTo(result));
            Assert.That(arg.DataUnsigned, Is.EqualTo(result));
        }

        [Test]
        public void DecodeUInt16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x42, 0x00, 0x00, 0x00, 0x34, 0x12 } :
                new byte[] { 0x00, 0x00, 0x00, 0x42, 0x12, 0x34 };

            Decode(payload, "UnsignedInt_16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("4660"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x1234));
        }

        [Test]
        public void DecodeUInt32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x43, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x00, 0x00, 0x00, 0x43, 0x78, 0x56, 0x34, 0x12 };

            Decode(payload, "UnsignedInt_32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("2018915346"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x78563412));
        }

        [Test]
        public void DecodeUInt64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x44, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x00, 0x00, 0x00, 0x44, 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(payload, "UnsignedInt_64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("9900958322455989675"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataUnsigned, Is.EqualTo(0x8967452301EFCDAB));
        }

        [Test]
        public void DecodeUInt128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x45, 0x00, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x45, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x00, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            Decode(payload, "UnsignedInt_128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [TestCase(0x40, 64U)]
        [TestCase(0x80, 128U)]
        [TestCase(0xFF, 255U)]
        public void DecodeUIntHex8bit(byte value, uint result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x41, 0x00, 0x01, 0x00, value } :
                new byte[] { 0x00, 0x01, 0x00, 0x41, value };

            Decode(payload, $"UnsignedIntHex_8bit_{value:x2}", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<HexIntDltArg>());
            HexIntDltArg arg = (HexIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo($"0x{value:x02}"));
            Assert.That(arg.Data, Is.EqualTo(result));
            Assert.That(arg.DataBytesLength, Is.EqualTo(1));
        }

        [Test]
        public void DecodeUIntHex16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x42, 0x00, 0x01, 0x00, 0x34, 0x12 } :
                new byte[] { 0x00, 0x01, 0x00, 0x42, 0x12, 0x34 };

            Decode(payload, "UnsignedIntHex_16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<HexIntDltArg>());
            HexIntDltArg arg = (HexIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x1234"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
            Assert.That(arg.DataBytesLength, Is.EqualTo(2));
        }

        [Test]
        public void DecodeUIntHex32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x43, 0x00, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x00, 0x01, 0x00, 0x43, 0x78, 0x56, 0x34, 0x12 };

            Decode(payload, "UnsignedIntHex_32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<HexIntDltArg>());
            HexIntDltArg arg = (HexIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x78563412"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
            Assert.That(arg.DataBytesLength, Is.EqualTo(4));
        }

        [Test]
        public void DecodeUIntHex64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x44, 0x00, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x00, 0x01, 0x00, 0x44, 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(payload, "UnsignedIntHex_64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<HexIntDltArg>());
            HexIntDltArg arg = (HexIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0x8967452301efcdab"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataBytesLength, Is.EqualTo(8));
        }

        [Test]
        public void DecodeUIntHex128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x45, 0x00, 0x01, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF } :
                new byte[] { 0x00, 0x01, 0x00, 0x45, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x00, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            Decode(payload, "UnsignedIntHex_128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeUIntBin8bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x41, 0x80, 0x01, 0x00, 0x40 } :
                new byte[] { 0x00, 0x01, 0x80, 0x41, 0x40 };

            Decode(payload, "UnsignedIntBin_8bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BinaryIntDltArg>());
            BinaryIntDltArg arg = (BinaryIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b0100 0000"));
            Assert.That(arg.Data, Is.EqualTo(64));
            Assert.That(arg.DataBytesLength, Is.EqualTo(1));
        }

        [Test]
        public void DecodeUIntBin16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x42, 0x80, 0x01, 0x00, 0x34, 0x12 } :
                new byte[] { 0x00, 0x01, 0x80, 0x42, 0x12, 0x34 };

            Decode(payload, "UnsignedIntBin_16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BinaryIntDltArg>());
            BinaryIntDltArg arg = (BinaryIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b0001 0010 0011 0100"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
            Assert.That(arg.DataBytesLength, Is.EqualTo(2));
        }

        [Test]
        public void DecodeUIntBin32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x43, 0x80, 0x01, 0x00, 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x00, 0x01, 0x80, 0x43, 0x78, 0x56, 0x34, 0x12 };

            Decode(payload, "UnsignedIntBin_32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BinaryIntDltArg>());
            BinaryIntDltArg arg = (BinaryIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b0111 1000 0101 0110 0011 0100 0001 0010"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
            Assert.That(arg.DataBytesLength, Is.EqualTo(4));
        }

        [Test]
        public void DecodeUIntBin64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x44, 0x80, 0x01, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x00, 0x01, 0x80, 0x44, 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(payload, "UnsignedIntBin_64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BinaryIntDltArg>());
            BinaryIntDltArg arg = (BinaryIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("0b1000 1001 0110 0111 0100 0101 0010 0011 0000 0001 1110 1111 1100 1101 1010 1011"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
            Assert.That(arg.DataBytesLength, Is.EqualTo(8));
        }

        [Test]
        public void DecodeUIntBin128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x45, 0x80, 0x01, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF } :
                new byte[] { 0x00, 0x01, 0x80, 0x45, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x00, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            Decode(payload, "UnsignedIntBin_128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeUIntUnknownCoding8bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x41, 0x80, 0x03, 0x00, 0x40 } :
                new byte[] { 0x00, 0x03, 0x80, 0x41, 0x40 };

            Decode(payload, "UnsignedIntUnknownCoding_8bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("64"));
            Assert.That(arg.Data, Is.EqualTo(64));
        }

        [Test]
        public void DecodeUIntUnknownCoding16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x42, 0x80, 0x03, 0x00, 0x34, 0x12 } :
                new byte[] { 0x00, 0x03, 0x80, 0x42, 0x12, 0x34 };

            Decode(payload, "UnsignedIntUnknownCoding_16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("4660"));
            Assert.That(arg.Data, Is.EqualTo(0x1234));
        }

        [Test]
        public void DecodeUIntUnknownCoding32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x43, 0x80, 0x03, 0x00, 0x12, 0x34, 0x56, 0x78 } :
                new byte[] { 0x00, 0x03, 0x80, 0x43, 0x78, 0x56, 0x34, 0x12 };

            Decode(payload, "UnsignedIntUnknownCoding_32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("2018915346"));
            Assert.That(arg.Data, Is.EqualTo(0x78563412));
        }

        [Test]
        public void DecodeUIntUnknownCoding64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x44, 0x80, 0x03, 0x00, 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 } :
                new byte[] { 0x00, 0x03, 0x80, 0x44, 0x89, 0x67, 0x45, 0x23, 0x01, 0xEF, 0xCD, 0xAB };

            Decode(payload, "UnsignedIntUnknownCoding_64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnsignedIntDltArg>());
            UnsignedIntDltArg arg = (UnsignedIntDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("9900958322455989675"));
            Assert.That(arg.Data, Is.EqualTo(unchecked((long)0x8967452301EFCDAB)));
        }

        [Test]
        public void DecodeUIntUnknownCoding128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x45, 0x80, 0x03, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x00, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF } :
                new byte[] { 0x00, 0x03, 0x80, 0x45, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x00, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            Decode(payload, "UnsignedIntUnknownCoding_128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [TestCase(0x40, 0x00, 0x00, TestName = "DecodeUintUnknown")]
        [TestCase(0x47, 0x00, 0x00, TestName = "DecodeUintUndefined")]
        [TestCase(0x40, 0x80, 0x01, TestName = "DecodeBinIntUnknown")]
        [TestCase(0x47, 0x80, 0x01, TestName = "DecodeBinIntUndefined")]
        [TestCase(0x40, 0x00, 0x01, TestName = "DecodeHexIntUnknown")]
        [TestCase(0x47, 0x00, 0x01, TestName = "DecodeHexIntUndefined")]
        public void DecodeSignedIntInvalid(byte typeInfo, byte cod1, byte cod2)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { typeInfo, cod1, cod2, 0x00 } :
                new byte[] { 0x00, cod2, cod1, typeInfo };

            DecodeIsInvalid(payload, $"UnsignedInt_Invalid_{typeInfo:x2}_{cod1:x2}_{cod2:x2}");
        }
    }
}
