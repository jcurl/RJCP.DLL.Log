namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Text;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(FloatArgDecoder))]
    [TestFixture(typeof(VerboseArgDecoder))]
    public class FloatArgDecoderTest<T> where T : IVerboseArgDecoder
    {
        [SetUp]
        public void InitializeTestCase()
        {
            if (typeof(T).Equals(typeof(VerboseArgDecoder))) {
                // Required to decode ISO-8859-15 when encoded as ASCII.
                var instance = CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(instance);
            }
        }

        private const int FloatArgType = 0x100;

        [Test]
        public void DecodeFloat32NegativeLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0xBF }, false, -1.89999998f);
        }

        [Test]
        public void DecodeFloat32NegativeBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0xBF, 0xF3, 0x33, 0x33 }, true, -1.89999998f);
        }

        [Test]
        public void DecodeFloat32PositiveLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x33, 0x33, 0xF3, 0x3F }, false, 1.89999998f);
        }

        [Test]
        public void DecodeFloat32PositiveBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x3F, 0xF3, 0x33, 0x33 }, true, 1.89999998f);
        }

        [Test]
        public void DecodeFloat32MinValueLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xFF }, false, float.MinValue);
        }

        [Test]
        public void DecodeFloat32MinValueBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0xFF, 0x7F, 0xFF, 0xFF }, true, float.MinValue);
        }

        [Test]
        public void DecodeFloat32MaxValueLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0x7F }, false, float.MaxValue);
        }

        [Test]
        public void DecodeFloat32MaxValueBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0xFF, 0xFF }, true, float.MaxValue);
        }

        [Test]
        public void DecodeFloat32PositiveInfinityLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x7F }, false, float.PositiveInfinity);
        }

        [Test]
        public void DecodeFloat32PositiveInfinityBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x7F, 0x80, 0x00, 0x00 }, true, float.PositiveInfinity);
        }

        [Test]
        public void DecodeFloat32NegativeInfinityLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xFF }, false, float.NegativeInfinity);
        }

        [Test]
        public void DecodeFloat32NegativeInfinityBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0xFF, 0x80, 0x00, 0x00 }, true, float.NegativeInfinity);
        }

        [Test]
        public void DecodeFloat32NaNLE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x7F }, false, float.NaN);
        }

        [Test]
        public void DecodeFloat32NaNBE()
        {
            DecodeFloat32(new byte[] { 0x83, 0x00, 0x00, 0x00, 0x7F, 0xC0, 0x00, 0x00 }, true, float.NaN);
        }

        [Test]
        public void DecodeFloat64NegativeLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0xBF }, false, -1.2d);
        }

        [Test]
        public void DecodeFloat64NegativeBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0xBF, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 }, true, -1.2d);
        }

        [Test]
        public void DecodeFloat64PositiveLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0xF3, 0x3F }, false, 1.2d);
        }

        [Test]
        public void DecodeFloat64PositiveBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x3F, 0xF3, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33 }, true, 1.2d);
        }

        [Test]
        public void DecodeFloat64MinValueLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF }, false, double.MinValue);
        }

        [Test]
        public void DecodeFloat64MinValueBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, true, double.MinValue);
        }

        [Test]
        public void DecodeFloat64MaxValueLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0x7F }, false, double.MaxValue);
        }

        [Test]
        public void DecodeFloat64MaxValueBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, true, double.MaxValue);
        }

        [Test]
        public void DecodeFloat64PositiveInfinityLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x7F }, false, double.PositiveInfinity);
        }

        [Test]
        public void DecodeFloat64PositiveInfinityBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x7F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, true, double.PositiveInfinity);
        }

        [Test]
        public void DecodeFloat64NegativeInfinityLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xFF }, false, double.NegativeInfinity);
        }

        [Test]
        public void DecodeFloat64NegativeInfinityBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0xFF, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, true, double.NegativeInfinity);
        }

        [Test]
        public void DecodeFloat64NaNLE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x7F }, false, double.NaN);
        }

        [Test]
        public void DecodeFloat64NaNBE()
        {
            DecodeFloat64(new byte[] { 0x84, 0x00, 0x00, 0x00, 0x7F, 0xF8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, true, double.NaN);
        }

        [Test]
        public void DecodeFloat16NegativeLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x48, 0xC2 }, false); // -3.141
        }

        [Test]
        public void DecodeFloat16NegativeBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0xC2, 0x48 }, true); // -3.141
        }

        [Test]
        public void DecodeFloat16PositiveLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x48, 0x42 }, false); // 3.141
        }

        [Test]
        public void DecodeFloat16PositiveBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x42, 0x48 }, true); // 3.141
        }

        [Test]
        public void DecodeFloat16MinValueLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFF, 0xFB }, false); // Half.MinValue
        }

        [Test]
        public void DecodeFloat16MinValueBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFB, 0xFF }, true); // Half.MinValue
        }

        [Test]
        public void DecodeFloat16EpsilonLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x01, 0x00 }, false); // Half.Epsilon
        }

        [Test]
        public void DecodeFloat16EpsilonBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0x01 }, true); // Half.Epsilon
        }

        [Test]
        public void DecodeFloat16MaxValueLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFF, 0x7B }, false); // Half.MaxValue
        }

        [Test]
        public void DecodeFloat16MaxValueBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x7B, 0xFF }, true); // Half.MaxValue
        }

        [Test]
        public void DecodeFloat16PositiveInfinityLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0x7C }, false); // Half.PositiveInfinity
        }

        [Test]
        public void DecodeFloat16PositiveInfinityBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x7C, 0x00 }, true); // Half.PositiveInfinity
        }

        [Test]
        public void DecodeFloat16NegativeInfinityLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0xFC }, false); // Half.NegativeInfinity
        }

        [Test]
        public void DecodeFloat16NegativeInfinityBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFC, 0x00 }, true); // Half.NegativeInfinity
        }

        [Test]
        public void DecodeFloat16NaNLE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0x00, 0xFE }, false); // Half.NaN
        }

        [Test]
        public void DecodeFloat16NaNBE()
        {
            DecodeFloat16(new byte[] { 0x82, 0x00, 0x00, 0x00, 0xFE, 0x00 }, true); // Half.NaN
        }

        [Test]
        public void DecodeFloat128PositiveInfinityLE()
        {
            DecodeFloat128(new byte[] {
                0x85, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7F
            }, false); // QFloat.PositiveInfinity
        }

        [Test]
        public void DecodeFloat128PositiveInfinityBE()
        {
            DecodeFloat128(new byte[] {
                0x85, 0x00, 0x00, 0x00,
                0x7F, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }, true); // QFloat.PositiveInfinity
        }

        [Test]
        public void DecodeFloat128NegativeInfinityLE()
        {
            DecodeFloat128(new byte[] {
                0x85, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7F
            }, false); // QFloat.PositiveInfinity
        }

        [Test]
        public void DecodeFloat128NegativeInfinityBE()
        {
            DecodeFloat128(new byte[] {
                0x85, 0x00, 0x00, 0x00,
                0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }, true); // QFloat.PositiveInfinity
        }

        [TestCase(0x81, TestName = "DecodeFloat8")]
        [TestCase(0x80, TestName = "DecodeFloatUnknown")]
        [TestCase(0x87, TestName = "DecodeFloatUndefined")]
        public void DecodeFloat(byte typeInfo)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(new byte[] { typeInfo, 0x00, 0x00, 0x00, 0x01 }, false, out IDltArg arg);
            Assert.That(length, Is.EqualTo(-1));
            Assert.That(arg, Is.Null);
        }

        private static void DecodeFloat16(byte[] buffer, bool msbf)
        {
            ArgDecoderTest.DecodeUnknown<T>(buffer, msbf, 0, FloatArgType);
        }

        private static void DecodeFloat32(byte[] buffer, bool msbf, float result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<Float32DltArg>());
            Assert.That(((Float32DltArg)arg).Data, Is.EqualTo(result));
        }

        private static void DecodeFloat64(byte[] buffer, bool msbf, double result)
        {
            T decoder = Activator.CreateInstance<T>();
            int length = decoder.Decode(buffer, msbf, out IDltArg arg);
            Assert.That(length, Is.EqualTo(buffer.Length));
            Assert.That(arg, Is.TypeOf<Float64DltArg>());
            Assert.That(((Float64DltArg)arg).Data, Is.EqualTo(result));
        }

        private static void DecodeFloat128(byte[] buffer, bool msbf)
        {
            ArgDecoderTest.DecodeUnknown<T>(buffer, msbf, 0, FloatArgType);
        }
    }
}
