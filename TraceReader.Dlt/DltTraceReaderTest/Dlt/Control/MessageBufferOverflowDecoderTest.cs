namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class MessageBufferOverflowDecoderTest : ControlDecoderTestBase<MessageBufferOverflowRequestDecoder, MessageBufferOverflowResponseDecoder>
    {
        public MessageBufferOverflowDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x14, typeof(MessageBufferOverflowRequest), typeof(MessageBufferOverflowResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x14, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x14 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x14_MessageBufferOverflowRequest", out IControlArg service);

            MessageBufferOverflowRequest request = (MessageBufferOverflowRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[message_buffer_overflow]"));
        }

        [TestCase(0x00, 0x00, "[message_buffer_overflow ok] false")]
        [TestCase(0x00, 0x01, "[message_buffer_overflow ok] true")]
        [TestCase(0x00, 0xFF, "[message_buffer_overflow ok] true")]
        public void DecodeResponse(byte status, byte overflow, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x14, 0x00, 0x00, 0x00, status, overflow } :
                new byte[] { 0x00, 0x00, 0x00, 0x14, status, overflow };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x14_MessageBufferOverflowResponse_{status:x2}_{overflow:x2}", out IControlArg service);

            MessageBufferOverflowResponse response = (MessageBufferOverflowResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Overflow, Is.EqualTo(overflow != 0));
        }

        [TestCase(0x01, "[message_buffer_overflow not_supported]")]
        [TestCase(0x02, "[message_buffer_overflow error]")]
        public void DecodeResponseError(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x14, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x14, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x14_MessageBufferOverflowResponse_{status:x2}_Error", out IControlArg service);

            ControlErrorNotSupported response = (ControlErrorNotSupported)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x14));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
        }
    }
}
