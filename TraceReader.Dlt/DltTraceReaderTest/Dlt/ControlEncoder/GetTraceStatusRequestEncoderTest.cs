namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(GetTraceStatusRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(GetTraceStatusRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    public class GetTraceStatusRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public GetTraceStatusRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void Encode()
        {
            GetTraceStatusRequest request =
                new GetTraceStatusRequest("APP1", "CTX1");

            Span<byte> buffer = ControlEncode(request, 12);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x1F));
            Assert.That(buffer[4..8].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x50, 0x50, 0x31 }));
            Assert.That(buffer[8..12].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x54, 0x58, 0x31 }));
        }

        [Test]
        public void EncodeEmpty()
        {
            GetTraceStatusRequest request =
                new GetTraceStatusRequest("", "");

            Span<byte> buffer = ControlEncode(request, 12);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x1F));
            Assert.That(buffer[4..8].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
            Assert.That(buffer[8..12].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 11, 1)] int length)
        {
            GetTraceStatusRequest request =
                new GetTraceStatusRequest("APP1", "CTX1");

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
