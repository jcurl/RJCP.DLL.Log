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
    public class Float32ArgDecoderTest : VerboseDecoderTestBase<FloatArgDecoder>
    {
        public Float32ArgDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian)
        { }

        [Test]
        public void DecodeFloat32Negative()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0xBF } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0xBF, 0xF3, 0x33, 0x33 };

            Decode(payload, "Float32_Negative", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-1.9"));
            Assert.That(arg.Data, Is.EqualTo(-1.89999998f));
        }

        [Test]
        public void DecodeFloat32Positive()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0x3F } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0x3F, 0xF3, 0x33, 0x33 };

            Decode(payload, "Float32_Positive", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.9"));
            Assert.That(arg.Data, Is.EqualTo(1.89999998f));
        }

        [Test]
        public void DecodeFloat32MinValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0xFF, 0x7F, 0xFF, 0xFF };

            Decode(payload, "Float32_MinValue", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-3.40282e+38"));
            Assert.That(arg.Data, Is.EqualTo(float.MinValue));
        }

        [Test]
        public void DecodeFloat32MaxValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0x7F, 0x7F, 0xFF, 0xFF };

            Decode(payload, "Float32_MaxValue", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("3.40282e+38"));
            Assert.That(arg.Data, Is.EqualTo(float.MaxValue));
        }

        [Test]
        public void DecodeFloat32PositiveInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0x7F, 0x80, 0x00, 0x00 };

            Decode(payload, "Float32_PositiveInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("inf"));
            Assert.That(arg.Data, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void DecodeFloat32NegativeInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0xFF, 0x80, 0x00, 0x00 };

            Decode(payload, "Float32_NegativeInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-inf"));
            Assert.That(arg.Data, Is.EqualTo(float.NegativeInfinity));
        }

        [Test]
        public void DecodeFloat32NaN()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x83, 0x7F, 0xC0, 0x00, 0x00 };

            Decode(payload, "Float32_NaN", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float32DltArg>());
            Float32DltArg arg = (Float32DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("nan"));
            Assert.That(arg.Data, Is.EqualTo(float.NaN));
        }

        [Test]
        public void DecodeFloat64Negative()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0xBF } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0xBF, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 };

            Decode(payload, "Float64_Negative", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-1.2"));
            Assert.That(arg.Data, Is.EqualTo(-1.2d));
        }

        [Test]
        public void DecodeFloat64Positive()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0x3F } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0x3F, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 };

            Decode(payload, "Float64_Positive", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.2"));
            Assert.That(arg.Data, Is.EqualTo(1.2d));
        }

        [Test]
        public void DecodeFloat64MinValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(payload, "Float64_MinValue", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-1.79769e+308"));
            Assert.That(arg.Data, Is.EqualTo(double.MinValue));
        }

        [Test]
        public void DecodeFloat64MaxValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            Decode(payload, "Float64_MaxValue", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("1.79769e+308"));
            Assert.That(arg.Data, Is.EqualTo(double.MaxValue));
        }

        [Test]
        public void DecodeFloat64PositiveInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0x7F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Float64_PositiveInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("inf"));
            Assert.That(arg.Data, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void DecodeFloat64NegativeInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xFF } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0xFF, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Float64_NegativeInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("-inf"));
            Assert.That(arg.Data, Is.EqualTo(double.NegativeInfinity));
        }

        [Test]
        public void DecodeFloat64NaN()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x84, 0x7F, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Float64_NaN", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<Float64DltArg>());
            Float64DltArg arg = (Float64DltArg)verboseArg;
            Assert.That(arg.ToString(), Is.EqualTo("nan"));
            Assert.That(arg.Data, Is.EqualTo(double.NaN));
        }

        [Test]
        public void DecodeFloat16Negative()
        {
            byte[] payload = Endian == Endianness.Little ?            // -3.141
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0x48, 0xC2 } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0xC2, 0x48 };

            Decode(payload, "Float16_Negative", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16Positive()
        {
            byte[] payload = Endian == Endianness.Little ?            // 3.141
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0x48, 0x42 } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0x42, 0x48 };

            Decode(payload, "Float16_Positive", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16MinValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFF, 0xFB } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0xFB, 0xFF };

            Decode(payload, "Float16_MinValue", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16MaxValue()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFF, 0x7B } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0x7B, 0xFF };

            Decode(payload, "Float16_MaxValue", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16Epsilon()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0x01, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0x00, 0x01 };

            Decode(payload, "Float16_Epsilon", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16PositiveInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0x7C } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0x7C, 0x00 };

            Decode(payload, "Float16_PositiveInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16NegativeInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0xFC } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0xFC, 0x00 };

            Decode(payload, "Float16_NegativeInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat16NaN()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0xFE } :
                new byte[] { 0x00, 0x00, 0x00, 0x82, 0xFE, 0x00 };

            Decode(payload, "Float16_NaN", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat128PositiveInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x85, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x85, 0x7F, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Float128_PositiveInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat128NegativeInfinity()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x85, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x85, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Decode(payload, "Float128_NegativeInfinity", out IDltArg verboseArg);
            Assert.That(verboseArg, Is.TypeOf<UnknownVerboseDltArg>());
        }

        [Test]
        public void DecodeFloat8()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x81, 0x00, 0x00, 0x00, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x81, 0x7F };

            DecodeIsInvalid(payload, "Float8_Invalid");
        }

        [TestCase(0x80, TestName = "DecodeFloatUnknownLength")]
        [TestCase(0x87, TestName = "DecodeFloatInvalidLength")]
        public void DecodeFloatInvalid(byte typeInfo)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x81, 0x00, 0x00, 0x00, 0x7F } :
                new byte[] { 0x00, 0x00, 0x00, 0x81, 0x7F };

            DecodeIsInvalid(payload, $"Float_Invalid_{typeInfo:x2}");
        }
    }
}
