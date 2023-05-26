namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;

    [TestFixture(typeof(BoolArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(BoolArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(BoolArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(BoolArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
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
    public class BoolArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public BoolArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(false, TestName = "Encode_False")]
        [TestCase(true, TestName = "Encode_True")]
        public void EncodeBool(bool value)
        {
            Span<byte> buffer = ArgEncode(new BoolDltArg(value), 1);
            Assert.That(buffer.Length, Is.EqualTo(IsVerbose ? 5 : 1));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x00, 0x11 } :
                    new byte[] { 0x11, 0x00, 0x00, 0x00 };
                Assert.That(buffer[0..4].ToArray(), Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.Slice(IsVerbose ? 4 : 0, 1);
            Assert.That(payload[0], Is.EqualTo(value ? 1 : 0));
        }

        [TestCase(false, TestName = "InsufficientBuffer_False")]
        [TestCase(true, TestName = "InsufficientBuffer_True")]
        public void InsufficientBuffer(bool value)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + HeaderLen];
            ArgEncode(buffer, new BoolDltArg(value), out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
