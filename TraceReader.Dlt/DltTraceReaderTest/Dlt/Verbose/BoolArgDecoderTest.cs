namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using Args;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class BoolArgDecoderTest : VerboseDecoderTestBase<BoolArgDecoder>
    {
        public BoolArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian)
        { }

        [Test]
        public void DecodeBoolFalseUnknownLength()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x10, 0x00 };

            Decode(payload, "Bool_FalseUnknownLength", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse8bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x11, 0x00 };

            Decode(payload, "Bool_False8bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse16bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x12, 0x00, 0x00 };

            Decode(payload, "Bool_False16bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Bool_False32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Bool_False64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Bool_False128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolTrueUnknownLength()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x10, 0x00, 0x00, 0x00, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x10, 0xFF };

            Decode(payload, "Bool_TrueUnknownLength", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0x01)]
        [TestCase(0xFF)]
        public void DecodeBoolTrue8bit(byte value)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x11, 0x00, 0x00, 0x00, value } :
                new byte[] { 0x00, 0x00, 0x00, 0x11, value };

            Decode(payload, $"Bool_True8bit_{value:x2}", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0x00, 0x01)]
        [TestCase(0x01, 0x00)]
        [TestCase(0xFF, 0xFF)]
        public void DecodeBoolTrue16bit(byte value1, byte value2)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x12, 0x00, 0x00, 0x00, value1, value2 } :
                new byte[] { 0x00, 0x00, 0x00, 0x12, value2, value1 };

            Decode(payload, $"Bool_True16bit_{value1:x2}{value2:x2}", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [Test]
        public void DecodeBoolTrue32bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x13, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x13, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(payload, "Bool_True32bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [Test]
        public void DecodeBoolTrue64bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x14, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x14, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(payload, "Bool_True64bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [Test]
        public void DecodeBoolTrue128bit()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x15, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x15, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(payload, "Bool_True128bit", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<BoolDltArg>());
            BoolDltArg arg = (BoolDltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0x10, 0x00, TestName = "DecodeBoolZeroLength")]
        [TestCase(0x17, 0x00, TestName = "DecodeBoolUnknownLength")]
        [TestCase(0x10, 0x08, TestName = "DecodeBoolZeroLengthVarInfo")]
        [TestCase(0x17, 0x08, TestName = "DecodeBoolUnknownLengthVarInfo")]
        public void DecodeBoolInvalid(byte typeInfo, byte cod1)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { typeInfo, cod1, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, cod1, typeInfo };

            DecodeIsInvalid(payload, $"Bool_Invalid_{typeInfo:x2}");
        }
    }
}
