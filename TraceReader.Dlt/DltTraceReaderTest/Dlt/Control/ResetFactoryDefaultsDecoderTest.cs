namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line, Endianness.Little)]
    [TestFixture(DecoderType.Packet, Endianness.Little)]
    [TestFixture(DecoderType.Specialized, Endianness.Little)]
    [TestFixture(DecoderType.Line, Endianness.Big)]
    [TestFixture(DecoderType.Packet, Endianness.Big)]
    [TestFixture(DecoderType.Specialized, Endianness.Big)]
    public class ResetFactoryDefaultsDecoderTest : ControlDecoderTestBase<ResetFactoryDefaultRequestDecoder, ResetFactoryDefaultResponseDecoder>
    {
        public ResetFactoryDefaultsDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x06, typeof(ResetFactoryDefaultRequest), typeof(ResetFactoryDefaultResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x06, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x06 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x06_ResetFactoryDefaultsRequest", out IControlArg service);

            ResetFactoryDefaultRequest request = (ResetFactoryDefaultRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[reset_to_factory_default]"));
        }

        [TestCase(0x00, "[reset_to_factory_default ok]")]
        [TestCase(0x01, "[reset_to_factory_default not_supported]")]
        [TestCase(0x02, "[reset_to_factory_default error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x06, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x06, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x06_ResetFactoryDefaultsResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x06));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
