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
    public class GetUseExtendedHeaderDecoderTest : ControlDecoderTestBase<GetUseExtendedHeaderRequestDecoder, GetUseExtendedHeaderResponseDecoder>
    {
        public GetUseExtendedHeaderDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x1E, typeof(GetUseExtendedHeaderRequest), typeof(GetUseExtendedHeaderResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x1E, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x1E };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1E_GetUseExtendedHeaderRequest", out IControlArg service);

            GetUseExtendedHeaderRequest request = (GetUseExtendedHeaderRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_use_extended_header]"));
        }

        [TestCase(0x00, 0x00, "[get_use_extended_header ok] off")]
        [TestCase(0x00, 0x01, "[get_use_extended_header ok] on")]
        [TestCase(0x00, 0xFF, "[get_use_extended_header ok] on")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x1E, 0x00, 0x00, 0x00, status, enabled } :
                new byte[] { 0x00, 0x00, 0x00, 0x1E, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1E_GetUseExtendedHeader_{status:x2}_{enabled:x2}", out IControlArg service);

            GetUseExtendedHeaderResponse response = (GetUseExtendedHeaderResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }

        [TestCase(0x01, "[get_use_extended_header not_supported]")]
        [TestCase(0x02, "[get_use_extended_header error]")]
        public void DecoderResponseError(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x1E, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x1E, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1E_GetUseExtendedHeader_{status:x2}_Error", out IControlArg service);

            ControlErrorNotSupported response = (ControlErrorNotSupported)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x1E));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
        }
    }
}
