namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(GetDefaultTraceStatusResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(GetDefaultTraceStatusResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class GetDefaultTraceStatusResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public GetDefaultTraceStatusResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void EncodeSuccess([Values(false, true)] bool enabled)
        {
            GetDefaultTraceStatusResponse response =
                new GetDefaultTraceStatusResponse(ControlResponse.StatusOk, enabled);

            Span<byte> buffer = ControlEncode(response, 6);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x15));
            Assert.That(buffer[4], Is.EqualTo(ControlResponse.StatusOk));
            Assert.That(buffer[5], Is.EqualTo(enabled ? 1 : 0));
        }

        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        public void EncodeError(int statusCode)
        {
            GetDefaultTraceStatusResponse response =
                new GetDefaultTraceStatusResponse(statusCode, false);

            Span<byte> buffer = ControlEncode(response, 5);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x15));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 5, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            GetDefaultTraceStatusResponse response =
                new GetDefaultTraceStatusResponse(ControlResponse.StatusOk, true);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
