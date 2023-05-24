namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(SetDefaultTraceStatusRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(SetDefaultTraceStatusRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    public class SetDefaultTraceStatusRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public SetDefaultTraceStatusRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void Encode()
        {
            SetDefaultTraceStatusRequest request =
                new SetDefaultTraceStatusRequest(true);

            Span<byte> buffer = ControlEncode(request, 9);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x12));
            Assert.That(buffer[4], Is.EqualTo(1));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void EncodeWithCom()
        {
            SetDefaultTraceStatusRequest request =
                new SetDefaultTraceStatusRequest(false, "COM1");

            Span<byte> buffer = ControlEncode(request, 9);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x12));
            Assert.That(buffer[4], Is.EqualTo(0));
            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x4F, 0x4D, 0x31 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 8, 1)] int length)
        {
            SetDefaultTraceStatusRequest request =
                new SetDefaultTraceStatusRequest(true);

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out int result);
            Assert.That(result, Is.EqualTo(-1));
        }
    }
}
