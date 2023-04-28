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
    public class NonVerboseFloat64ArgDecoderTest : NonVerboseDecoderTestBase<NonVerboseFloat64ArgDecoder>
    {
        private static readonly IFrameMap FrameMap = new TestFrameMap()
            .Add(0, new TestPdu("S_FLOA64", 0))
            .Add(1, new TestPdu("S_FLOA64", 8))
            .Add(2, new TestPdu("S_FLOA64", 12));

        public NonVerboseFloat64ArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, FrameMap) { }

        [Test]
        public void DecodeFloat64Negative()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0xBF } :
                new byte[] { 0xBF, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 };

            Decode(1, payload, "nv_Float64_Negative", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-1.2"));
            Assert.That(arg.Data, Is.EqualTo(-1.2d));
        }

        [Test]
        public void DecodeFloat64Positive()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0x3F } :
                new byte[] { 0x3F, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 };

            Decode(1, payload, "nv_Float64_Positive", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.2"));
            Assert.That(arg.Data, Is.EqualTo(1.2d));
        }

        [Test]
        public void DecodeFloat64MinValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF } :
                new byte[] { 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(1, payload, "nv_Float64_MinValue", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-1.79769e+308"));
            Assert.That(arg.Data, Is.EqualTo(double.MinValue));
        }

        [Test]
        public void DecodeFloat64MaxValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0x7F } :
                new byte[] { 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(1, payload, "nv_Float64_MaxValue", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.79769e+308"));
            Assert.That(arg.Data, Is.EqualTo(double.MaxValue));
        }

        [Test]
        public void DecodeFloat64PositiveInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x7F } :
                new byte[] { 0x7F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(1, payload, "nv_Float64_PositiveInfinity", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("inf"));
            Assert.That(arg.Data, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void DecodeFloat64NegativeInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xFF } :
                new byte[] { 0xFF, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(1, payload, "nv_Float64_NegativeInfinity", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("-inf"));
            Assert.That(arg.Data, Is.EqualTo(double.NegativeInfinity));
        }

        [Test]
        public void DecodeFloat64NaN()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x7F } :
                new byte[] { 0x7F, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(1, payload, "nv_Float64_NaN", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("nan"));
            Assert.That(arg.Data, Is.EqualTo(double.NaN));
        }

        [Test]
        public void DecodeFloat64Long()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0x3F, 0xAA, 0x55, 0x88, 0x33 } :
                new byte[] { 0x3F, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xAA, 0x55, 0x88, 0x33 };

            Decode(2, payload, "nv_Float64_Long", out IDltArg dltArg);
            Assert.That(dltArg, Is.TypeOf<Float64DltArg>());

            Float64DltArg arg = (Float64DltArg)dltArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.2"));
            Assert.That(arg.Data, Is.EqualTo(1.2d));
        }

        [Test]
        public void DecodeFloat64NoBuffer()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(1, payload, "nv_Float64_Invalid");
        }

        [Test]
        public void DecodeFloat64PduZeroBytes()
        {
            byte[] payload = Array.Empty<byte>();

            DecodeIsInvalid(0, payload, "nv_Float64_InvalidPduZero");
        }

        [Test]
        public void DecodeFloat64PduInvalidPdu()
        {
            byte[] payload = new byte[] { 0x00 };

            DecodeIsInvalid(1, payload, "nv_Float64_InvalidPdu");
        }
    }
}
