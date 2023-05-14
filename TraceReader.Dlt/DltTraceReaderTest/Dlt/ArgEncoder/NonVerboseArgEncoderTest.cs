namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(NonVerboseArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(NonVerboseArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(NonVerboseArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(NonVerboseArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Big, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Argument, Endianness.Little, LineType.NonVerbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Big, LineType.Verbose)]
    [TestFixture(typeof(DltArgEncoder), EncoderType.Arguments, Endianness.Little, LineType.Verbose)]
    public class NonVerboseArgEncoderTest<TArgEncoder> : ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        public NonVerboseArgEncoderTest(EncoderType encoderType, Endianness endianness, LineType lineType)
            : base(encoderType, endianness, lineType) { }

        [TestCase(new byte[] { }, TestName = "Encode_NVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, TestName = "Encode_NV")]
        public void EncodeNvBytes(byte[] value)
        {
            byte[] buffer = new byte[(IsVerbose ? 6 : 0) + value.Length];
            NonVerboseDltArg arg = new NonVerboseDltArg(value);
            Assert.That(ArgEncode(buffer, arg), Is.EqualTo(buffer.Length));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));

                Span<byte> payload = buffer.AsSpan(4);
                int len = unchecked((ushort)BitOperations.To16Shift(payload, !IsBigEndian));
                Assert.That(len, Is.EqualTo(value.Length));
                Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
            } else {
                Span<byte> payload = buffer.AsSpan();
                Assert.That(payload.ToArray(), Is.EqualTo(value));
            }
        }

        [TestCase(new byte[] { }, TestName = "Encode_UNVEmpty")]
        [TestCase(new byte[] { 0xFF, 0x01 }, TestName = "Encode_UNV")]
        public void EncodeUNvBytes(byte[] value)
        {
            byte[] buffer = new byte[(IsVerbose ? 6 : 0) + value.Length];
            UnknownNonVerboseDltArg arg = new UnknownNonVerboseDltArg(value);
            Assert.That(ArgEncode(buffer, arg), Is.EqualTo(buffer.Length));

            if (IsVerbose) {
                byte[] typeInfo = IsBigEndian ?
                    new byte[] { 0x00, 0x00, 0x04, 0x00 } :
                    new byte[] { 0x00, 0x04, 0x00, 0x00 };
                Assert.That(buffer[0..4], Is.EqualTo(typeInfo));

                Span<byte> payload = buffer.AsSpan(4);
                int len = unchecked((ushort)BitOperations.To16Shift(payload, !IsBigEndian));
                Assert.That(len, Is.EqualTo(value.Length));
                Assert.That(payload[2..].ToArray(), Is.EqualTo(value));
            } else {
                Span<byte> payload = buffer.AsSpan();
                Assert.That(payload.ToArray(), Is.EqualTo(value));
            }
        }

        [TestCase(TestName = "Encode_NVMaxData")]
        public void EncodeNvBytesMax()
        {
            byte[] data = new byte[65535 - (IsVerbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeNvBytes(data);
        }

        [TestCase(TestName = "Encode_NVOverSize")]
        public void EncodeNvBytesOverSize()
        {
            byte[] data = new byte[65536 - (IsVerbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            NonVerboseDltArg arg = new NonVerboseDltArg(data);
            Assert.That(ArgEncode(data, arg), Is.EqualTo(-1));
        }

        [TestCase(TestName = "Encode_UNVMaxData")]
        public void EncodeUNvBytesMax()
        {
            byte[] data = new byte[65535 - (IsVerbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            EncodeUNvBytes(data);
        }

        [TestCase(TestName = "Encode_UNVOverSize")]
        public void EncodeUNvBytesOverSize()
        {
            byte[] data = new byte[65536 - (IsVerbose ? 6 : 0)];
            Random rnd = new Random();
            rnd.NextBytes(data);

            UnknownNonVerboseDltArg arg = new UnknownNonVerboseDltArg(data);
            Assert.That(ArgEncode(data, arg), Is.EqualTo(-1));
        }
    }
}
