namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(Float64ArgEncoder))]
    [TestFixture(typeof(DltArgEncoder))]
    public class Float64ArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        [TestCase(0, true, false, 0x84, TestName = "Encode_LittleEndian_DoubleZero")]
        [TestCase(double.NaN, true, false, 0x84, TestName = "Encode_LittleEndian_DoubleNan")]
        [TestCase(double.PositiveInfinity, true, false, 0x84, TestName = "Encode_LittleEndian_DoublePosInf")]
        [TestCase(double.NegativeInfinity, true, false, 0x84, TestName = "Encode_LittleEndian_DoubleNegInf")]
        [TestCase(double.Epsilon, true, false, 0x84, TestName = "Encode_LittleEndian_DoubleEps")]
        [TestCase(double.MaxValue, true, false, 0x84, TestName = "Encode_LittleEndian_DoubleMax")]
        [TestCase(double.MinValue, true, false, 0x84, TestName = "Encode_LittleEndian_DoubleMin")]
        [TestCase(3.141592, true, false, 0x84, TestName = "Encode_LittleEndian_DoublePi")]
        [TestCase(0, true, true, 0x84, TestName = "Encode_BigEndian_DoubleZero")]
        [TestCase(double.NaN, true, true, 0x84, TestName = "Encode_BigEndian_DoubleNan")]
        [TestCase(double.PositiveInfinity, true, true, 0x84, TestName = "Encode_BigEndian_DoublePosInf")]
        [TestCase(double.NegativeInfinity, true, true, 0x84, TestName = "Encode_BigEndian_DoubleNegInf")]
        [TestCase(double.Epsilon, true, true, 0x84, TestName = "Encode_BigEndian_DoubleEps")]
        [TestCase(double.MaxValue, true, true, 0x84, TestName = "Encode_BigEndian_DoubleMax")]
        [TestCase(double.MinValue, true, true, 0x84, TestName = "Encode_BigEndian_DoubleMin")]
        [TestCase(3.141592, true, true, 0x84, TestName = "Encode_BigEndian_DoublePi")]

        [TestCase(0, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoubleZero")]
        [TestCase(double.NaN, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoubleNan")]
        [TestCase(double.PositiveInfinity, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoublePosInf")]
        [TestCase(double.NegativeInfinity, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoubleNegInf")]
        [TestCase(double.Epsilon, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoubleEps")]
        [TestCase(double.MaxValue, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoubleMax")]
        [TestCase(double.MinValue, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoubleMin")]
        [TestCase(3.141592, false, false, 0x84, TestName = "EncodeNv_LittleEndian_DoublePi")]
        [TestCase(0, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoubleZero")]
        [TestCase(double.NaN, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoubleNan")]
        [TestCase(double.PositiveInfinity, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoublePosInf")]
        [TestCase(double.NegativeInfinity, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoubleNegInf")]
        [TestCase(double.Epsilon, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoubleEps")]
        [TestCase(double.MaxValue, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoubleMax")]
        [TestCase(double.MinValue, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoubleMin")]
        [TestCase(3.141592, false, true, 0x84, TestName = "EncodeNv_BigEndian_DoublePi")]
        public void EncodeFloat32(double value, bool verbose, bool msbf, byte expTypeInfo)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 8];
            Float64DltArg arg = new Float64DltArg(value);
            IArgEncoder encoder = GetEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x00, expTypeInfo } :
                    new byte[] { expTypeInfo, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0, 8);
            double result = BitOperations.To64FloatShift(payload[0..8], !msbf);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(3.141592, true, false, TestName = "InsufficientBuffer_LittleEndian_DoublePi")]
        [TestCase(3.141592, true, true, TestName = "InsufficientBuffer_BigEndian_DoublePi")]

        [TestCase(3.141592, false, false, TestName = "InsufficientBufferNv_LittleEndian_DoublePi")]
        [TestCase(3.141592, false, true, TestName = "InsufficientBufferNv_BigEndian_DoublePi")]
        public void InsufficientBuffer(double value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 3];
            Float64DltArg arg = new Float64DltArg(value);
            IArgEncoder encoder = GetEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
