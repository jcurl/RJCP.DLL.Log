namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(RawArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(RawArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(RawArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(RawArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
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
    public class RawArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public RawArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(new byte[] { }, TestName = "Encode_RawEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, TestName = "Encode_Raw")]
        public void EncodeRaw(byte[] value)
        {
            Span<byte> buffer = ArgEncode(new RawDltArg(value), 2 + value.Length);
            Assert.That(buffer.Length, Is.EqualTo((IsVerbose ? 4 : 0) + 2 + value.Length));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4].ToArray(), Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer[(IsVerbose ? 4 : 0)..];
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !IsBigEndian));
            Assert.That(len, Is.EqualTo(value.Length));
            Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
        }

        [TestCase(TestName = "Encode_RawMaxData")]
        public void EncodeRawMax()
        {
            byte[] data = new byte[65535 - (IsVerbose ? 4 : 0) - 2 - HeaderLen];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeRaw(data);
        }

        [TestCase(TestName = "Encode_RawOverSize")]
        public void EncodeRawOverSize()
        {
            byte[] data = new byte[65535 - (IsVerbose ? 4 : 0) - 1 - HeaderLen];
            Random rnd = new Random();
            rnd.NextBytes(data);

            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + HeaderLen + 2 + 65535];
            ArgEncode(buffer, new RawDltArg(data), out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
