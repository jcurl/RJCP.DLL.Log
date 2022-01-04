namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetUseTimeStampDecoderTest : ControlDecoderTestBase<SetUseTimeStampRequestDecoder, SetUseTimeStampResponseDecoder>
    {
        public SetUseTimeStampDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0F, typeof(SetUseTimeStampRequest), typeof(SetUseTimeStampResponse))
        { }

        [TestCase(0x00, "[use_timestamp] off")]
        [TestCase(0x01, "[use_timestamp] on")]
        [TestCase(0xFF, "[use_timestamp] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0F, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x0F_SetUseTimeStampRequest_{status:x2}", out IControlArg service);

            SetUseTimeStampRequest request = (SetUseTimeStampRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[use_timestamp ok]")]
        [TestCase(0x01, "[use_timestamp not_supported]")]
        [TestCase(0x02, "[use_timestamp error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x0F, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x0F_SetUseTimeStampResponse_{status:x2}", out IControlArg service);

            SetUseTimeStampResponse response = (SetUseTimeStampResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
