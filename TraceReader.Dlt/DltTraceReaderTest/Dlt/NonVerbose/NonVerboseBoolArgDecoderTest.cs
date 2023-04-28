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
    public class NonVerboseBoolArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseBoolArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_BOOL", 0))
            .Add(1, new TestPdu("S_BOOL", 1))
            .Add(2, new TestPdu("S_BOOL", 2))
            .Add(3, new TestPdu("S_BOOL", 4))
            .Add(4, new TestPdu("S_BOOL", 8))
            .Add(5, new TestPdu("S_BOOL", 10));

        public NonVerboseBoolArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        [Test]
        public void DecodeBoolFalse8bit()
        {
            byte[] payload = new byte[] { 0x00 };

            Decode(1, payload, "nv_Bool_False8bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse16bit()
        {
            byte[] payload = new byte[] { 0x00, 0x00 };

            Decode(2, payload, "nv_Bool_False16bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse32bit()
        {
            byte[] payload = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            Decode(3, payload, "nv_Bool_False32bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [Test]
        public void DecodeBoolFalse64bit()
        {
            byte[] payload = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(4, payload, "nv_Bool_False64bit", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("false"));
            Assert.That(arg.Data, Is.False);
        }

        [TestCase(0xFF)]
        [TestCase(0x01)]
        public void DecodeBoolTrue8bit(byte value)
        {
            byte[] payload = new byte[] { value };

            Decode(1, payload, $"nv_Bool_True8bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0xFF)]
        [TestCase(0x01)]
        public void DecodeBoolTrue16bit(byte value)
        {
            byte[] payload;
            if (value == 0xFF)
                payload = new byte[] { 0xFF, 0xFF };
            else
                payload = Endian == Endianness.Little ?
                    new byte[] { value, 0x00 } :
                    new byte[] { 0x00, value };

            Decode(2, payload, $"nv_Bool_True16bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0xFF)]
        [TestCase(0x01)]
        public void DecodeBoolTrue32bit(byte value)
        {
            byte[] payload;
            if (value == 0xFF)
                payload = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            else
                payload = Endian == Endianness.Little ?
                    new byte[] { value, 0x00, 0x00, 0x00 } :
                    new byte[] { 0x00, 0x00, 0x00, value };

            Decode(3, payload, $"nv_Bool_True32bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0xFF)]
        [TestCase(0x01)]
        public void DecodeBoolTrue64bit(byte value)
        {
            byte[] payload;
            if (value == 0xFF)
                payload = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            else
                payload = Endian == Endianness.Little ?
                    new byte[] { value, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                    new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, value };

            Decode(4, payload, $"nv_Bool_True64bit_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [TestCase(0xFF)]
        [TestCase(0x01)]
        public void DecodeBoolTrue64bit_Long(byte value)
        {
            byte[] payload;
            if (value == 0xFF)
                payload = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            else
                payload = Endian == Endianness.Little ?
                    new byte[] { value, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } :
                    new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, value };

            Decode(5, payload, $"nv_Bool_True64bit_Long_{value:x2}", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<BoolDltArg>());

            BoolDltArg arg = (BoolDltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("true"));
            Assert.That(arg.Data, Is.True);
        }

        [Test]
        public void DecodeBoolNoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_Bool_Invalid");
        }

        [Test]
        public void DecodeBoolPduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_Bool_InvalidPdu");
        }
    }
}
