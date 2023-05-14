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
    public class RawArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public RawArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(new byte[] { }, TestName = "Encode_RawEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, TestName = "Encode_Raw")]
        public void EncodeRaw(byte[] value)
        {
            byte[] buffer = new byte[(IsVerbose ? 4 : 0) + 2 + value.Length];
            RawDltArg arg = new RawDltArg(value);
            Assert.That(ArgEncode(buffer, arg), Is.EqualTo(buffer.Length));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));
            }

            Span<byte> payload = buffer.AsSpan(IsVerbose ? 4 : 0);
            int len = unchecked((ushort)BitOperations.To16Shift(payload, !IsBigEndian));
            Assert.That(len, Is.EqualTo(value.Length));
            Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
        }

        [TestCase(TestName = "Encode_RawMaxData")]
        public void EncodeRawMax()
        {
            byte[] data = new byte[65533 - (IsVerbose ? 4 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeRaw(data);
        }

        [TestCase(TestName = "Encode_RawOverSize")]
        public void EncodeRawOverSize()
        {
            byte[] data = new byte[65534 - (IsVerbose ? 4 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            RawDltArg arg = new RawDltArg(data);
            Assert.That(ArgEncode(data, arg), Is.EqualTo(-1));
        }
    }
}
