namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class ResetFactoryDefaultsDecoderTest : ControlDecoderTestBase<ResetFactoryDefaultRequestDecoder, ResetFactoryDefaultResponseDecoder>
    {
        public ResetFactoryDefaultsDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x06, typeof(ResetFactoryDefaultRequest), typeof(ResetFactoryDefaultResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x06, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x06_ResetFactoryDefaultsRequest", out IControlArg service);

            ResetFactoryDefaultRequest request = (ResetFactoryDefaultRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[reset_to_factory_default]"));
        }

        [TestCase(0x00, "[reset_to_factory_default ok]")]
        [TestCase(0x01, "[reset_to_factory_default not_supported]")]
        [TestCase(0x02, "[reset_to_factory_default error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x06, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x06_ResetFactoryDefaultsResponse_{status:x2}", out IControlArg service);

            ResetFactoryDefaultResponse response = (ResetFactoryDefaultResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
