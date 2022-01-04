namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class CustomConnectionInfoDecoderTest : ControlDecoderTestBase<NoDecoder, CustomConnectionInfoResponseDecoder>
    {
        public CustomConnectionInfoDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0F02, null, typeof(CustomConnectionInfoResponse))
        { }

        [TestCase(0x00, 0x00, "[connection_info ok] unknown eth0")]
        [TestCase(0x01, 0x00, "[connection_info not_supported] unknown eth0")]
        [TestCase(0x02, 0x00, "[connection_info error] unknown eth0")]
        [TestCase(0x00, 0x01, "[connection_info ok] disconnected eth0")]
        [TestCase(0x01, 0x01, "[connection_info not_supported] disconnected eth0")]
        [TestCase(0x02, 0x01, "[connection_info error] disconnected eth0")]
        [TestCase(0x00, 0x02, "[connection_info ok] connected eth0")]
        [TestCase(0x01, 0x02, "[connection_info not_supported] connected eth0")]
        [TestCase(0x02, 0x02, "[connection_info error] connected eth0")]
        public void DecodeResponse(byte status, byte state, string result)
        {
            byte[] payload = new byte[] {
                0x02, 0x0F, 0x00, 0x00, status, state, 0x65, 0x74, 0x68, 0x30
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF02_CustomConnectionInfoResponse_{status:x2}_{state:x2}", out IControlArg service);

            CustomConnectionInfoResponse response = (CustomConnectionInfoResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.ConnectionState, Is.EqualTo(state));
            Assert.That(response.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(0x00, 0x00, "[connection_info ok] unknown")]
        [TestCase(0x01, 0x00, "[connection_info not_supported] unknown")]
        [TestCase(0x02, 0x00, "[connection_info error] unknown")]
        [TestCase(0x00, 0x01, "[connection_info ok] disconnected")]
        [TestCase(0x01, 0x01, "[connection_info not_supported] disconnected")]
        [TestCase(0x02, 0x01, "[connection_info error] disconnected")]
        [TestCase(0x00, 0x02, "[connection_info ok] connected")]
        [TestCase(0x01, 0x02, "[connection_info not_supported] connected")]
        [TestCase(0x02, 0x02, "[connection_info error] connected")]
        public void DecodeResponseNoComId(byte status, byte state, string result)
        {
            byte[] payload = new byte[] {
                0x02, 0x0F, 0x00, 0x00, status, state, 0x00, 0x00, 0x00, 0x00
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF02_CustomConnectionInfoResponse_NoComId_{status:x2}_{state:x2}", out IControlArg service);

            CustomConnectionInfoResponse response = (CustomConnectionInfoResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.ConnectionState, Is.EqualTo(state));
            Assert.That(response.ComInterface, Is.EqualTo(string.Empty));
        }
    }
}
