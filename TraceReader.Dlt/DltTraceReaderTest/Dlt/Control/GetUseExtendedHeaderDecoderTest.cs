namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetUseExtendedHeaderDecoderTest : ControlDecoderTestBase<GetUseExtendedHeaderRequestDecoder, GetUseExtendedHeaderResponseDecoder>
    {
        public GetUseExtendedHeaderDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x1E, typeof(GetUseExtendedHeaderRequest), typeof(GetUseExtendedHeaderResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x1E, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1E_GetUseExtendedHeaderRequest", out IControlArg service);

            GetUseExtendedHeaderRequest request = (GetUseExtendedHeaderRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_use_extended_header]"));
        }

        [TestCase(0x00, 0x00, "[get_use_extended_header ok] off")]
        [TestCase(0x00, 0x01, "[get_use_extended_header ok] on")]
        [TestCase(0x00, 0xFF, "[get_use_extended_header ok] on")]
        [TestCase(0x01, 0x00, "[get_use_extended_header not_supported]")]
        [TestCase(0x01, 0x01, "[get_use_extended_header not_supported]")]
        [TestCase(0x01, 0xFF, "[get_use_extended_header not_supported]")]
        [TestCase(0x02, 0x00, "[get_use_extended_header error]")]
        [TestCase(0x02, 0x01, "[get_use_extended_header error]")]
        [TestCase(0x02, 0xFF, "[get_use_extended_header error]")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = new byte[] { 0x1E, 0x00, 0x00, 0x00, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1E_GetUseExtendedHeader_{status:x2}_{enabled:x2}", out IControlArg service);

            GetUseExtendedHeaderResponse response = (GetUseExtendedHeaderResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }
    }
}
