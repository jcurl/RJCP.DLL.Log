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
    public class SetUseExtendedHeaderDecoderTest : ControlDecoderTestBase<SetUseExtendedHeaderRequestDecoder, SetUseExtendedHeaderResponseDecoder>
    {
        public SetUseExtendedHeaderDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x10, typeof(SetUseExtendedHeaderRequest), typeof(SetUseExtendedHeaderResponse))
        { }

        [TestCase(0x00, "[use_extended_header] off")]
        [TestCase(0x01, "[use_extended_header] on")]
        [TestCase(0xFF, "[use_extended_header] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x10, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x10, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x10_SetUseExtendedHeaderRequest_{status:x2}", out IControlArg service);

            SetUseExtendedHeaderRequest request = (SetUseExtendedHeaderRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[use_extended_header ok]")]
        [TestCase(0x01, "[use_extended_header not_supported]")]
        [TestCase(0x02, "[use_extended_header error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x10, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x10, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x10_SetUseExtendedHeaderResponse_{status:x2}", out IControlArg service);

            SetUseExtendedHeaderResponse response = (SetUseExtendedHeaderResponse)service;
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
