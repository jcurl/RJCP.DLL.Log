namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetUseEcuIdDecoderTest : ControlDecoderTestBase<SetUseEcuIdRequestDecoder, SetUseEcuIdResponseDecoder>
    {
        public SetUseEcuIdDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0D, typeof(SetUseEcuIdRequest), typeof(SetUseEcuIdResponse))
        { }

        [TestCase(0x00, "[use_ecu_id] off")]
        [TestCase(0x01, "[use_ecu_id] on")]
        [TestCase(0xFF, "[use_ecu_id] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0D, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0D_SetUseEcuIdRequest_{status:x2}", out IControlArg service);

            SetUseEcuIdRequest request = (SetUseEcuIdRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[use_ecu_id ok]")]
        [TestCase(0x01, "[use_ecu_id not_supported]")]
        [TestCase(0x02, "[use_ecu_id error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0D, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0D_SetUseEcuIdResponse_{status:x2}", out IControlArg service);

            SetUseEcuIdResponse response = (SetUseEcuIdResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
