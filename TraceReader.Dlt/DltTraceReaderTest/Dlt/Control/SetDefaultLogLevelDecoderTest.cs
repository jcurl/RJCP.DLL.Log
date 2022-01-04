namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SetDefaultLogLevelDecoderTest : ControlDecoderTestBase<SetDefaultLogLevelRequestDecoder, SetDefaultLogLevelResponseDecoder>
    {
        public SetDefaultLogLevelDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x11, typeof(SetDefaultLogLevelRequest), typeof(SetDefaultLogLevelResponse))
        { }

        [TestCase(0x00, "[set_default_log_level] block_all")]
        [TestCase(0x01, "[set_default_log_level] fatal")]
        [TestCase(0xFF, "[set_default_log_level] default")]
        public void DecodeRequestNoComId(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x11, 0x00, 0x00, 0x00, logLevel, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x11_SetDefaultLogLevelRequest_NoComId_{logLevel:x2}", out IControlArg service);

            SetDefaultLogLevelRequest request = (SetDefaultLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_default_log_level] block_all eth0")]
        [TestCase(0x01, "[set_default_log_level] fatal eth0")]
        [TestCase(0xFF, "[set_default_log_level] default eth0")]
        public void DecodeRequest(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x11, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x11_SetDefaultLogLevelRequest_{logLevel:x2}", out IControlArg service);

            SetDefaultLogLevelRequest request = (SetDefaultLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_default_log_level] block_all eth")]
        [TestCase(0x01, "[set_default_log_level] fatal eth")]
        [TestCase(0xFF, "[set_default_log_level] default eth")]
        public void DecodeRequest3Char(byte logLevel, string result)
        {
            byte[] payload = new byte[] {
                0x11, 0x00, 0x00, 0x00, logLevel, 0x65, 0x74, 0x68, 0x00
            };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x11_SetDefaultLogLevelRequest_3Char_{logLevel:x2}", out IControlArg service);

            SetDefaultLogLevelRequest request = (SetDefaultLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_default_log_level ok]")]
        [TestCase(0x01, "[set_default_log_level not_supported]")]
        [TestCase(0x02, "[set_default_log_level error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x11, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x11_SetDefaultLogLevelResponse_{status:x2}", out IControlArg service);

            SetDefaultLogLevelResponse response = (SetDefaultLogLevelResponse)service;
            Assert.That(response.Status, Is.EqualTo((int)status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
