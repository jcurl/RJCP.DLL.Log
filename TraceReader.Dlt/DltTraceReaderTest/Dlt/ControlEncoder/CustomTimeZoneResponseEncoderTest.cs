namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(CustomTimeZoneResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(CustomTimeZoneResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    public class CustomTimeZoneResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public CustomTimeZoneResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void EncodeNegativeTz(
            [Values(ControlResponse.StatusOk, ControlResponse.StatusError, ControlResponse.StatusNotSupported)] int statusCode,
            [Values(false, true)] bool isDst)
        {
            CustomTimeZoneResponse response =
                new CustomTimeZoneResponse(statusCode, new TimeSpan(-6, 0, 0), isDst);

            byte[] expectedTzOffset = IsBigEndian ?
                new byte[] { 0xFF, 0xFF, 0xAB, 0xA0 } :
                new byte[] { 0xA0, 0xAB, 0xFF, 0xFF };

            Span<byte> buffer = ControlEncode(response, 10);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0xF03));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(expectedTzOffset));
            Assert.That(buffer[9], Is.EqualTo(isDst ? 1 : 0));
        }

        [Test]
        public void EncodePositiveTz(
            [Values(ControlResponse.StatusOk, ControlResponse.StatusError, ControlResponse.StatusNotSupported)] int statusCode,
            [Values(false, true)] bool isDst)
        {
            CustomTimeZoneResponse response =
                new CustomTimeZoneResponse(statusCode, new TimeSpan(9, 30, 0), isDst);

            byte[] expectedTzOffset = IsBigEndian ?
                new byte[] { 0x00, 0x00, 0x85, 0x98 } :
                new byte[] { 0x98, 0x85, 0x00, 0x00 };

            Span<byte> buffer = ControlEncode(response, 10);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0xF03));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(expectedTzOffset));
            Assert.That(buffer[9], Is.EqualTo(isDst ? 1 : 0));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 9, 1)] int length)
        {
            CustomConnectionInfoResponse response =
                new CustomConnectionInfoResponse(ControlResponse.StatusOk, CustomConnectionInfoResponse.Connected);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
