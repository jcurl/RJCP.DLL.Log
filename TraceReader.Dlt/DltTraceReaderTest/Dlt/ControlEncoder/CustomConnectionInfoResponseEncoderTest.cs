namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(CustomConnectionInfoResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(CustomConnectionInfoResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class CustomConnectionInfoResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public CustomConnectionInfoResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [TestCase(ControlResponse.StatusOk)]
        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        public void Encode(int statusCode)
        {
            CustomConnectionInfoResponse response =
                new CustomConnectionInfoResponse(statusCode, CustomConnectionInfoResponse.Connected);

            Span<byte> buffer = ControlEncode(response, 10);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0xF02));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
            Assert.That(buffer[5], Is.EqualTo(CustomConnectionInfoResponse.Connected));
            Assert.That(buffer[6..10].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 9, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            CustomConnectionInfoResponse response =
                new CustomConnectionInfoResponse(ControlResponse.StatusOk, CustomConnectionInfoResponse.Connected);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
