namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetMessageFilteringDecoderTest : ControlDecoderTestBase<SetMessageFilteringRequestDecoder, SetMessageFilteringResponseDecoder>
    {
        public SetMessageFilteringDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0A, typeof(SetMessageFilteringRequest), typeof(SetMessageFilteringResponse))
        { }

        [TestCase(0x00, "[set_message_filtering] off")]
        [TestCase(0x01, "[set_message_filtering] on")]
        [TestCase(0xFF, "[set_message_filtering] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0A, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0A_SetMessageFilteringRequest_{status:x2}", out IControlArg service);

            SetMessageFilteringRequest request = (SetMessageFilteringRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_message_filtering ok]")]
        [TestCase(0x01, "[set_message_filtering not_supported]")]
        [TestCase(0x02, "[set_message_filtering error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0A, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0A_SetMessageFilteringResponse_{status:x2}", out IControlArg service);

            SetMessageFilteringResponse response = (SetMessageFilteringResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
