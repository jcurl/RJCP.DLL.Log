namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetUseSessionIdDecoderTest : ControlDecoderTestBase<SetUseSessionIdRequestDecoder, SetUseSessionIdResponseDecoder>
    {
        public SetUseSessionIdDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0E, typeof(SetUseSessionIdRequest), typeof(SetUseSessionIdResponse))
        { }

        [TestCase(0x00, "[use_session_id] off")]
        [TestCase(0x01, "[use_session_id] on")]
        [TestCase(0xFF, "[use_session_id] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0E, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0E_SetUseSessionIdRequest_{status:x2}", out IControlArg service);

            SetUseSessionIdRequest request = (SetUseSessionIdRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[use_session_id ok]")]
        [TestCase(0x01, "[use_session_id not_supported]")]
        [TestCase(0x02, "[use_session_id error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0E, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0E_SetUseSessionIdResponse_{status:x2}", out IControlArg service);

            SetUseSessionIdResponse response = (SetUseSessionIdResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
