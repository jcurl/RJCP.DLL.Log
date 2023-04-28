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
    public class NonVerboseFloat32ArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseFloat32ArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_FLOA32", 0))
            .Add(1, new TestPdu("S_FLOA32", 4))
            .Add(2, new TestPdu("S_FLOA32", 8));

        public NonVerboseFloat32ArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        [Test]
        public void DecodeFloat32Negative()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x33, 0x33, 0xF3, 0xBF } :
                new byte[] { 0xBF, 0xF3, 0x33, 0x33 };

            Decode(1, payload, "nv_Float32_Negative", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-1.9"));
            Assert.That(arg.Data, Is.EqualTo(-1.89999998f));
        }

        [Test]
        public void DecodeFloat32Positive()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x33, 0x33, 0xF3, 0x3F } :
                new byte[] { 0x3F, 0xF3, 0x33, 0x33 };

            Decode(1, payload, "nv_Float32_Positive", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.9"));
            Assert.That(arg.Data, Is.EqualTo(1.89999998f));
        }

        [Test]
        public void DecodeFloat32MinValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xFF, 0xFF, 0x7F, 0xFF } :
                new byte[] { 0xFF, 0x7F, 0xFF, 0xFF };

            Decode(1, payload, "nv_Float32_MinValue", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-3.40282e+38"));
            Assert.That(arg.Data, Is.EqualTo(float.MinValue));
        }

        [Test]
        public void DecodeFloat32MaxValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xFF, 0xFF, 0x7F, 0x7F } :
                new byte[] { 0x7F, 0x7F, 0xFF, 0xFF };

            Decode(1, payload, "nv_Float32_MaxValue", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("3.40282e+38"));
            Assert.That(arg.Data, Is.EqualTo(float.MaxValue));
        }

        [Test]
        public void DecodeFloat32PositiveInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x00, 0x80, 0x7F } :
                new byte[] { 0x7F, 0x80, 0x00, 0x00 };

            Decode(1, payload, "nv_Float32_PositiveInfinity", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("inf"));
            Assert.That(arg.Data, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void DecodeFloat32NegativeInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x00, 0x80, 0xFF } :
                new byte[] { 0xFF, 0x80, 0x00, 0x00 };

            Decode(1, payload, "nv_Float32_NegativeInfinity", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-inf"));
            Assert.That(arg.Data, Is.EqualTo(float.NegativeInfinity));
        }

        [Test]
        public void DecodeFloat32NaN()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x00, 0xC0, 0x7F } :
                new byte[] { 0x7F, 0xC0, 0x00, 0x00 };

            Decode(1, payload, "nv_Float32_NaN", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("nan"));
            Assert.That(arg.Data, Is.EqualTo(float.NaN));
        }

        [Test]
        public void DecodeFloat32_Long()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x33, 0x33, 0xF3, 0x3F, 0x00, 0x01, 0x02, 0x03 } :
                new byte[] { 0x3F, 0xF3, 0x33, 0x33, 0x00, 0x01, 0x02, 0x03 };

            Decode(2, payload, "nv_Float32_Long", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float32DltArg>());

            Float32DltArg arg = (Float32DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.9"));
            Assert.That(arg.Data, Is.EqualTo(1.89999998f));
        }

        [Test]
        public void DecodeFloat32NoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_Float32_Invalid");
        }

        [Test]
        public void DecodeFloat32PduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_Float32_InvalidPduZero");
        }

        [Test]
        public void DecodeFloat32PduInvalidPdu()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(1, payload, "nv_Float32_InvalidPdu");
        }
    }
}
