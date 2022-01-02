namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetDefaultLogLevelDecoderTest : ControlDecoderTestBase<GetDefaultLogLevelRequestDecoder, GetDefaultLogLevelResponseDecoder>
    {
        public GetDefaultLogLevelDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x04, typeof(GetDefaultLogLevelRequest), typeof(GetDefaultLogLevelResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x04, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x04_GetDefaultLogLevelRequest", out IControlArg service);

            GetDefaultLogLevelRequest request = (GetDefaultLogLevelRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_default_log_level]"));
        }

        [TestCase(0x00, 0x00, "[get_default_log_level ok] block_all")]
        [TestCase(0x01, 0x00, "[get_default_log_level not_supported] block_all")]
        [TestCase(0x02, 0x00, "[get_default_log_level error] block_all")]
        [TestCase(0x00, 0x01, "[get_default_log_level ok] fatal")]
        [TestCase(0x01, 0x01, "[get_default_log_level not_supported] fatal")]
        [TestCase(0x02, 0x01, "[get_default_log_level error] fatal")]
        [TestCase(0x00, 0xFF, "[get_default_log_level ok] log_level=255")]
        [TestCase(0x01, 0xFF, "[get_default_log_level not_supported] log_level=255")]
        [TestCase(0x02, 0xFF, "[get_default_log_level error] log_level=255")]
        public void DecodeResponse(byte status, byte logLevel, string result)
        {
            byte[] payload = new byte[] { 0x04, 0x00, 0x00, 0x00, status, logLevel };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x04_GetDefaultLogLevelResponse_{logLevel:x2}_{status:x2}", out IControlArg service);

            GetDefaultLogLevelResponse response = (GetDefaultLogLevelResponse)service;
            Assert.That(response.LogLevel, Is.EqualTo(logLevel));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
