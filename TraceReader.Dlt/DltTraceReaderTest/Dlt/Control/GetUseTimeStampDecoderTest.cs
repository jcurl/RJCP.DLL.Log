namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetUseTimeStampDecoderTest : ControlDecoderTestBase<GetUseTimeStampRequestDecoder, GetUseTimeStampResponseDecoder>
    {
        public GetUseTimeStampDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x1D, typeof(GetUseTimeStampRequest), typeof(GetUseTimeStampResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x1D, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1D_GetUseTimeStampRequest", out IControlArg service);

            GetUseTimeStampRequest request = (GetUseTimeStampRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_use_timestamp]"));
        }

        [TestCase(0x00, 0x00, "[get_use_timestamp ok] off")]
        [TestCase(0x00, 0x01, "[get_use_timestamp ok] on")]
        [TestCase(0x00, 0xFF, "[get_use_timestamp ok] on")]
        [TestCase(0x01, 0x00, "[get_use_timestamp not_supported]")]
        [TestCase(0x01, 0x01, "[get_use_timestamp not_supported]")]
        [TestCase(0x01, 0xFF, "[get_use_timestamp not_supported]")]
        [TestCase(0x02, 0x00, "[get_use_timestamp error]")]
        [TestCase(0x02, 0x01, "[get_use_timestamp error]")]
        [TestCase(0x02, 0xFF, "[get_use_timestamp error]")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = new byte[] { 0x1D, 0x00, 0x00, 0x00, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1D_GetUseTimeStampResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetUseTimeStampResponse response = (GetUseTimeStampResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }
    }
}
