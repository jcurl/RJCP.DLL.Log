namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class BufferOverflowNotificationDecoderTest : ControlDecoderTestBase<BufferOverflowNotificationRequestDecoder, BufferOverflowNotificationResponseDecoder>
    {
        public BufferOverflowNotificationDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x23, typeof(BufferOverflowNotificationRequest), typeof(BufferOverflowNotificationResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x23, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x23_BufferOverflowNotificationRequest", out IControlArg service);

            BufferOverflowNotificationRequest request = (BufferOverflowNotificationRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[buffer_overflow]"));
        }

        [TestCase(0x00, "[buffer_overflow ok] 0")]
        [TestCase(0x01, "[buffer_overflow not_supported] 0")]
        [TestCase(0x02, "[buffer_overflow error] 0")]
        public void DecodeResponsePositive(byte status, string result)
        {
            byte[] payload = new byte[] { 0x23, 0x00, 0x00, 0x00, status, 0x00, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x23_BufferOverflowNotificationResponse_{status:x2}_00000000", out IControlArg service);

            BufferOverflowNotificationResponse response = (BufferOverflowNotificationResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Counter, Is.EqualTo(0));
        }

        [TestCase(0x00, "[buffer_overflow ok] 10000")]
        [TestCase(0x01, "[buffer_overflow not_supported] 10000")]
        [TestCase(0x02, "[buffer_overflow error] 10000")]
        public void DecodeResponsePositive10000(byte status, string result)
        {
            byte[] payload = new byte[] { 0x23, 0x00, 0x00, 0x00, status, 0x10, 0x27, 0x00, 0x00 };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x23_BufferOverflowNotificationResponse_{status:x2}_00002710", out IControlArg service);

            BufferOverflowNotificationResponse response = (BufferOverflowNotificationResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Counter, Is.EqualTo(10000));
        }

        [TestCase(0x00, "[buffer_overflow ok] 4294967295")]
        [TestCase(0x01, "[buffer_overflow not_supported] 4294967295")]
        [TestCase(0x02, "[buffer_overflow error] 4294967295")]
        public void DecodeResponsePositiveLarge(byte status, string result)
        {
            byte[] payload = new byte[] { 0x23, 0x00, 0x00, 0x00, status, 0xFF, 0xFF, 0xFF, 0xFF };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x23_BufferOverflowNotificationResponse_{status:x2}_ffffffff", out IControlArg service);

            BufferOverflowNotificationResponse response = (BufferOverflowNotificationResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Counter, Is.EqualTo(-1));
        }
    }
}
