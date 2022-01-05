namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class SwInjectionDecoderTest : ControlDecoderTestBase<SwInjectionRequestDecoder, SwInjectionResponseDecoder>
    {
        public SwInjectionDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x1000, typeof(SwInjectionRequest), typeof(SwInjectionResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] {
                0x00, 0x10, 0x00, 0x00,
                0x05, 0x00, 0x00, 0x00,
                0x11, 0x22, 0x33, 0x44, 0x55
            };
            Decode(DltType.CONTROL_REQUEST, payload, "0xFFF_SwInjectionRequest", out IControlArg service);

            SwInjectionRequest request = (SwInjectionRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[] 05 00 00 00 11 22 33 44 55"));
            Assert.That(request.Payload.Length, Is.EqualTo(5));
            Assert.That(request.Payload, Is.EqualTo(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55 }));
        }
    }
}
