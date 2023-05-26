namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(BufferOverflowNotificationResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(BufferOverflowNotificationResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class BufferOverflowNotificationResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public BufferOverflowNotificationResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void EncodeSuccess()
        {
            BufferOverflowNotificationResponse response =
                new BufferOverflowNotificationResponse(ControlResponse.StatusOk, 10);

            Span<byte> buffer = ControlEncode(response, 9);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x23));
            Assert.That(buffer[4], Is.EqualTo(ControlResponse.StatusOk));
            Assert.That(BitOperations.To32Shift(buffer[5..9], !IsBigEndian), Is.EqualTo(10));
        }

        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        public void EncodeError(int statusCode)
        {
            BufferOverflowNotificationResponse response =
                new BufferOverflowNotificationResponse(statusCode, 15);

            Span<byte> buffer = ControlEncode(response, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x23));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
        }

        [Test]
        public void InsufficientBuffer([Range(0, 8, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            BufferOverflowNotificationResponse response =
                new BufferOverflowNotificationResponse(ControlResponse.StatusOk, 10);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
