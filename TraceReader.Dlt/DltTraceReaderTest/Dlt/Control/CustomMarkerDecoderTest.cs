namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class CustomMarkerDecoderTest : ControlDecoderTestBase<NoDecoder, CustomMarkerResponseDecoder>
    {
        public CustomMarkerDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x0F04, null, typeof(CustomMarkerResponse))
        { }

        [TestCase(0x00, "MARKER")]
        [TestCase(0x01, "MARKER")]
        [TestCase(0x02, "MARKER")]
        public void DecodeResponsePositiveTz(byte status, string result)
        {
            byte[] payload = new byte[] {
                0x04, 0x0F, 0x00, 0x00, status,
            };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0xF04_CustomMarker_{status:x2}", out IControlArg service);

            CustomMarkerResponse response = (CustomMarkerResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
