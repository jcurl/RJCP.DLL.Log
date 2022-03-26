namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;
    using RJCP.Core;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class BufferOverflowNotificationDecoderTest
        : ControlDecoderTestBase<BufferOverflowNotificationRequestDecoder, BufferOverflowNotificationResponseDecoder>
    {
        public BufferOverflowNotificationDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x23, typeof(BufferOverflowNotificationRequest), typeof(BufferOverflowNotificationResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x23, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x23 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x23_BufferOverflowNotificationRequest", out IControlArg service);

            BufferOverflowNotificationRequest request = (BufferOverflowNotificationRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[buffer_overflow]"));
        }

        [TestCase(0x00, new byte[] { 0x00, 0x00, 0x00, 0x00 }, "[buffer_overflow ok] 0", TestName = "DecodeResponsePositive")]
        [TestCase(0x00, new byte[] { 0x00, 0x00, 0x27, 0x10 }, "[buffer_overflow ok] 10000", TestName = "DecodeResponsePositive10000")]
        [TestCase(0x00, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, "[buffer_overflow ok] 4294967295", TestName = "DecodeResponsePositiveLarge")]
        public void DecodeResponse(byte status, byte[] data, string result)
        {
            int counter = BitOperations.To32ShiftBigEndian(data, 0);
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x23, 0x00, 0x00, 0x00, status, data[3], data[2], data[1], data[0] } :
                new byte[] { 0x00, 0x00, 0x00, 0x23, status, data[0], data[1], data[2], data[3] };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x23_BufferOverflowNotificationResponse_{status:x2}_{counter:x08}", out IControlArg service);

            BufferOverflowNotificationResponse response = (BufferOverflowNotificationResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Counter, Is.EqualTo(counter));
        }

        [TestCase(0x01, "[buffer_overflow not_supported]")]
        [TestCase(0x02, "[buffer_overflow error]")]
        public void DecodeResponseError(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x23, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x23, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x23_BufferOverflowNotificationResponse_{status:x2}_Error", out IControlArg service);

            ControlErrorNotSupported response = (ControlErrorNotSupported)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x23));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
