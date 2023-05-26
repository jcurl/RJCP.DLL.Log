namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(Float32ArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(Float32ArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(Float32ArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(Float32ArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceEncoder, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceEncoder, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceWriter, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.TraceWriter, Endianness.Little, LineType.Verbose)]
    public class Float32ArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public Float32ArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(0, 0x83, TestName = "Encode_FloatZero")]
        [TestCase(float.NaN, 0x83, TestName = "Encode_FloatNan")]
        [TestCase(float.PositiveInfinity, 0x83, TestName = "Encode_FloatPosInf")]
        [TestCase(float.NegativeInfinity, 0x83, TestName = "Encode_FloatNegInf")]
        [TestCase(float.Epsilon, 0x83, TestName = "Encode_FloatEps")]
        [TestCase(float.MaxValue, 0x83, TestName = "Encode_FloatMax")]
        [TestCase(float.MinValue, 0x83, TestName = "Encode_FloatMin")]
        [TestCase(3.141592F, 0x83, TestName = "Encode_FloatPi")]
        public void EncodeFloat32(float value, byte expTypeInfo)
        {
            Span<byte> buffer = ArgEncode(new Float32DltArg(value), 4);
            Assert.That(buffer.Length, Is.EqualTo((IsVerbose ? 4 : 0) + 4));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x00, expTypeInfo } :
                    new byte[] { expTypeInfo, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4].ToArray(), Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.Slice(IsVerbose ? 4 : 0, 4);
            float result = BitOperations.To32FloatShift(payload[0..4], !IsBigEndian);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(3.141592F, TestName = "InsufficientBuffer_FloatPi")]
        public void InsufficientBuffer(float value)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + HeaderLen + 3];
            ArgEncode(buffer, new Float32DltArg(value), out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
