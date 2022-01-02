namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class StoreConfigurationDecoderTest : ControlDecoderTestBase<StoreConfigurationRequestDecoder, StoreConfigurationResponseDecoder>
    {
        public StoreConfigurationDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x05, typeof(StoreConfigurationRequest), typeof(StoreConfigurationResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x05, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x05_StoreConfigurationRequest", out IControlArg service);

            StoreConfigurationRequest request = (StoreConfigurationRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[store_config]"));
        }

        [TestCase(0x00, "[store_config ok]")]
        [TestCase(0x01, "[store_config not_supported]")]
        [TestCase(0x02, "[store_config error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = new byte[] { 0x05, 0x00, 0x00, 0x00, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x05_StoreConfigurationResponse_{status:x2}", out IControlArg service);

            StoreConfigurationResponse response = (StoreConfigurationResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
