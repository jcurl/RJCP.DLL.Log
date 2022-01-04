namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetUseEcuIdDecoderTest : ControlDecoderTestBase<GetUseEcuIdRequestDecoder, GetUseEcuIdResponseDecoder>
    {
        public GetUseEcuIdDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x1B, typeof(GetUseEcuIdRequest), typeof(GetUseEcuIdResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x1B, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1B_GetUseEcuIdRequest", out IControlArg service);

            GetUseEcuIdRequest request = (GetUseEcuIdRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_use_ecu_id]"));
        }

        [TestCase(0x00, 0x00, "[get_use_ecu_id ok] off")]
        [TestCase(0x00, 0x01, "[get_use_ecu_id ok] on")]
        [TestCase(0x00, 0xFF, "[get_use_ecu_id ok] on")]
        [TestCase(0x01, 0x00, "[get_use_ecu_id not_supported]")]
        [TestCase(0x01, 0x01, "[get_use_ecu_id not_supported]")]
        [TestCase(0x01, 0xFF, "[get_use_ecu_id not_supported]")]
        [TestCase(0x02, 0x00, "[get_use_ecu_id error]")]
        [TestCase(0x02, 0x01, "[get_use_ecu_id error]")]
        [TestCase(0x02, 0xFF, "[get_use_ecu_id error]")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = new byte[] { 0x1B, 0x00, 0x00, 0x00, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1B_GetUseEcuIdResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetUseEcuIdResponse response = (GetUseEcuIdResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }
    }
}
