namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetMessageFilteringStatusDecoderTest : ControlDecoderTestBase<GetMessageFilteringStatusRequestDecoder, GetMessageFilteringStatusResponseDecoder>
    {
        public GetMessageFilteringStatusDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x1A, typeof(GetMessageFilteringStatusRequest), typeof(GetMessageFilteringStatusResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x1A, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1A_GetMessageFilteringStatusRequest", out IControlArg service);

            GetMessageFilteringStatusRequest request = (GetMessageFilteringStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_message_filtering]"));
        }

        [TestCase(0x00, 0x00, "[get_message_filtering ok] off")]
        [TestCase(0x00, 0x01, "[get_message_filtering ok] on")]
        [TestCase(0x00, 0xFF, "[get_message_filtering ok] on")]
        [TestCase(0x01, 0x00, "[get_message_filtering not_supported]")]
        [TestCase(0x01, 0x01, "[get_message_filtering not_supported]")]
        [TestCase(0x01, 0xFF, "[get_message_filtering not_supported]")]
        [TestCase(0x02, 0x00, "[get_message_filtering error]")]
        [TestCase(0x02, 0x01, "[get_message_filtering error]")]
        [TestCase(0x02, 0xFF, "[get_message_filtering error]")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = new byte[] { 0x1A, 0x00, 0x00, 0x00, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1A_GetMessageFilteringStatusResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetMessageFilteringStatusResponse response = (GetMessageFilteringStatusResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }
    }
}
