namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(CustomUnregisterContextResponseEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(CustomUnregisterContextResponseEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class CustomUnregisterContextResponseEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public CustomUnregisterContextResponseEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [TestCase(ControlResponse.StatusOk)]
        [TestCase(ControlResponse.StatusError)]
        [TestCase(ControlResponse.StatusNotSupported)]
        public void Encode(int statusCode)
        {
            CustomUnregisterContextResponse response =
                new CustomUnregisterContextResponse(statusCode, "APP1", "CTX1", null);

            Span<byte> buffer = ControlEncode(response, 17);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0xF01));
            Assert.That(buffer[4], Is.EqualTo(statusCode));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x50, 0x50, 0x31 }));
            Assert.That(buffer[9..13].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x54, 0x58, 0x31 }));
            Assert.That(buffer[13..17].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 16, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            CustomUnregisterContextResponse response =
                new CustomUnregisterContextResponse(ControlResponse.StatusOk, "APP1", "CTX1", null);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, response, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
