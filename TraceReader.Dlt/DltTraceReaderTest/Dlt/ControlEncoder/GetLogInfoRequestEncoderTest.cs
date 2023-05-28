namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(typeof(GetLogInfoRequestEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(GetLogInfoRequestEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Argument, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.Arguments, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceEncoder, Endianness.Little)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Big)]
    [TestFixture(typeof(ControlArgEncoder), EncoderType.TraceWriter, Endianness.Little)]
    public class GetLogInfoRequestEncoderTest<TControlEncoder>
        : ControlEncoderTestBase<TControlEncoder> where TControlEncoder : IControlArgEncoder
    {
        public GetLogInfoRequestEncoderTest(EncoderType encoderType, Endianness endianness)
            : base(encoderType, endianness) { }

        [Test]
        public void Encode()
        {
            GetLogInfoRequest request =
                new GetLogInfoRequest(GetLogInfoRequest.OptionsFullInfo, "APP1", "CTX1");

            Span<byte> buffer = ControlEncode(request, 17);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoRequest.OptionsFullInfo));

            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x50, 0x50, 0x31 }));
            Assert.That(buffer[9..13].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x54, 0x58, 0x31 }));
            Assert.That(buffer[13..17].ToArray(), Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void EncodeWithCom()
        {
            GetLogInfoRequest request =
                new GetLogInfoRequest(GetLogInfoRequest.OptionsFullInfo, "APP1", "CTX1", "COM1");

            Span<byte> buffer = ControlEncode(request, 17);
            Assert.That(BitOperations.To32Shift(buffer[0..4], !IsBigEndian), Is.EqualTo(0x03));
            Assert.That(buffer[4], Is.EqualTo(GetLogInfoRequest.OptionsFullInfo));

            Assert.That(buffer[5..9].ToArray(), Is.EqualTo(new byte[] { 0x41, 0x50, 0x50, 0x31 }));
            Assert.That(buffer[9..13].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x54, 0x58, 0x31 }));
            Assert.That(buffer[13..17].ToArray(), Is.EqualTo(new byte[] { 0x43, 0x4F, 0x4D, 0x31 }));
        }

        [Test]
        public void InsufficientBuffer([Range(3, 16, 1)] int length)
        {
            if (IsWriter) Assert.Inconclusive("Test case is meaningless");

            GetLogInfoRequest request =
                new GetLogInfoRequest(GetLogInfoRequest.OptionsFullInfo, "APP1", "CTX1");

            byte[] buffer = new byte[length];
            _ = ControlEncode(buffer, request, out Result<int> result);
            Assert.That(result.HasValue, Is.False);
        }
    }
}
