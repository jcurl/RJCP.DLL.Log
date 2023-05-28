namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(Float64ArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(Float64ArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(Float64ArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(Float64ArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
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
    public class Float64ArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public Float64ArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(0, 0x84, TestName = "Encode_DoubleZero")]
        [TestCase(double.NaN, 0x84, TestName = "Encode_DoubleNan")]
        [TestCase(double.PositiveInfinity, 0x84, TestName = "Encode_DoublePosInf")]
        [TestCase(double.NegativeInfinity, 0x84, TestName = "Encode_DoubleNegInf")]
        [TestCase(double.Epsilon, 0x84, TestName = "Encode_DoubleEps")]
        [TestCase(double.MaxValue, 0x84, TestName = "Encode_DoubleMax")]
        [TestCase(double.MinValue, 0x84, TestName = "Encode_DoubleMin")]
        [TestCase(3.141592, 0x84, TestName = "Encode_DoublePi")]
        public void EncodeFloat64(double value, byte expTypeInfo)
        {
            Span<byte> buffer = ArgEncode(new Float64DltArg(value), 8);
            Assert.That(buffer.Length, Is.EqualTo((IsVerbose ? 4 : 0) + 8));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x00, expTypeInfo } :
                    new byte[] { expTypeInfo, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4].ToArray(), Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.Slice(IsVerbose ? 4 : 0, 8);
            double result = BitOperations.To64FloatShift(payload[0..8], !IsBigEndian);
            Assert.That(result, Is.EqualTo(value));
        }

        [TestCase(3.141592, TestName = "InsufficientBuffer_DoublePi")]
        public void InsufficientBuffer(double value)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + HeaderLen + 7];
            ArgEncode(buffer, new Float64DltArg(value), out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
