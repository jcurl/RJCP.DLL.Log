namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture]
    public class Float32ArgEncoderTest
    {
        [TestCase(0, true, false, 0x83, TestName = "Encode_LittleEndian_FloatZero")]
        [TestCase(float.NaN, true, false, 0x83, TestName = "Encode_LittleEndian_FloatNan")]
        [TestCase(float.PositiveInfinity, true, false, 0x83, TestName = "Encode_LittleEndian_FloatPosInf")]
        [TestCase(float.NegativeInfinity, true, false, 0x83, TestName = "Encode_LittleEndian_FloatNegInf")]
        [TestCase(float.Epsilon, true, false, 0x83, TestName = "Encode_LittleEndian_FloatEps")]
        [TestCase(float.MaxValue, true, false, 0x83, TestName = "Encode_LittleEndian_FloatMax")]
        [TestCase(float.MinValue, true, false, 0x83, TestName = "Encode_LittleEndian_FloatMin")]
        [TestCase(3.141592F, true, false, 0x83, TestName = "Encode_LittleEndian_FloatPi")]
        [TestCase(0, true, true, 0x83, TestName = "Encode_BigEndian_FloatZero")]
        [TestCase(float.NaN, true, true, 0x83, TestName = "Encode_BigEndian_FloatNan")]
        [TestCase(float.PositiveInfinity, true, true, 0x83, TestName = "Encode_BigEndian_FloatPosInf")]
        [TestCase(float.NegativeInfinity, true, true, 0x83, TestName = "Encode_BigEndian_FloatNegInf")]
        [TestCase(float.Epsilon, true, true, 0x83, TestName = "Encode_BigEndian_FloatEps")]
        [TestCase(float.MaxValue, true, true, 0x83, TestName = "Encode_BigEndian_FloatMax")]
        [TestCase(float.MinValue, true, true, 0x83, TestName = "Encode_BigEndian_FloatMin")]
        [TestCase(3.141592F, true, true, 0x83, TestName = "Encode_BigEndian_FloatPi")]

        [TestCase(0, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatZero")]
        [TestCase(float.NaN, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatNan")]
        [TestCase(float.PositiveInfinity, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatPosInf")]
        [TestCase(float.NegativeInfinity, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatNegInf")]
        [TestCase(float.Epsilon, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatEps")]
        [TestCase(float.MaxValue, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatMax")]
        [TestCase(float.MinValue, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatMin")]
        [TestCase(3.141592F, false, false, 0x83, TestName = "EncodeNv_LittleEndian_FloatPi")]
        [TestCase(0, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatZero")]
        [TestCase(float.NaN, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatNan")]
        [TestCase(float.PositiveInfinity, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatPosInf")]
        [TestCase(float.NegativeInfinity, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatNegInf")]
        [TestCase(float.Epsilon, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatEps")]
        [TestCase(float.MaxValue, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatMax")]
        [TestCase(float.MinValue, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatMin")]
        [TestCase(3.141592F, false, true, 0x83, TestName = "EncodeNv_BigEndian_FloatPi")]
        public void EncodeFloat32(float value, bool verbose, bool msbf, byte expTypeInfo)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 4];
            Float32DltArg arg = new Float32DltArg(value);
            IArgEncoder encoder = new Float32ArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(buffer.Length));

            if (verbose) {
                byte[] typeInfo = msbf ?
                    new byte[] { 0x00, 0x00, 0x00, expTypeInfo } :
                    new byte[] { expTypeInfo, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(verbose ? 4 : 0, 4);
            float result = BitOperations.To32FloatShift(payload[0..4], !msbf);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(3.141592F, true, false, TestName = "InsufficientBuffer_LittleEndian_FloatPi")]
        [TestCase(3.141592F, true, true, TestName = "InsufficientBuffer_BigEndian_FloatPi")]

        [TestCase(3.141592F, false, false, TestName = "InsufficientBufferNv_LittleEndian_FloatPi")]
        [TestCase(3.141592F, false, true, TestName = "InsufficientBufferNv_BigEndian_FloatPi")]
        public void InsufficientBuffer(float value, bool verbose, bool msbf)
        {
            byte[] buffer = new byte[(verbose ? 4 : 0) + 3];
            Float32DltArg arg = new Float32DltArg(value);
            IArgEncoder encoder = new Float32ArgEncoder();
            Assert.That(encoder.Encode(buffer, verbose, msbf, arg), Is.EqualTo(-1));
        }
    }
}
