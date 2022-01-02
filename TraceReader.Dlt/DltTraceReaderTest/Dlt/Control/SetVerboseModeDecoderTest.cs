namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetVerboseModeDecoderTest : ControlDecoderTestBase<SetVerboseModeRequestDecoder, SetVerboseModeResponseDecoder>
    {
        public SetVerboseModeDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x09, typeof(SetVerboseModeRequest), typeof(SetVerboseModeResponse))
        { }

        [TestCase(0x00, "[set_verbose_mode] off")]
        [TestCase(0x01, "[set_verbose_mode] on")]
        [TestCase(0xFF, "[set_verbose_mode] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = new byte[] { 0x09, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x09_SetVerboseModeRequest_{status:x2}", out IControlArg service);

            SetVerboseModeRequest request = (SetVerboseModeRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_verbose_mode ok]")]
        [TestCase(0x01, "[set_verbose_mode not_supported]")]
        [TestCase(0x02, "[set_verbose_mode error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x09, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x09_SetVerboseModeResponse_{status:x2}", out IControlArg service);

            SetVerboseModeResponse response = (SetVerboseModeResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
