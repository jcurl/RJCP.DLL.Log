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
    public class SetVerboseModeDecoderTest : ControlDecoderTestBase<SetVerboseModeRequestDecoder, SetVerboseModeResponseDecoder>
    {
        public SetVerboseModeDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x09, typeof(SetVerboseModeRequest), typeof(SetVerboseModeResponse))
        { }

        [TestCase(0x00, "[set_verbose_mode] off")]
        [TestCase(0x01, "[set_verbose_mode] on")]
        [TestCase(0xFF, "[set_verbose_mode] on")]
        public void DecodeRequest(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x09, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x09, status };
            Decode(DltType.CONTROL_REQUEST, payload, $"0x09_SetVerboseModeRequest_{status:x2}", out IControlArg service);

            SetVerboseModeRequest request = (SetVerboseModeRequest)service;
            Assert.That(request.ToString(), Is.EqualTo(result));
        }

        [TestCase(0x00, "[set_verbose_mode ok]")]
        [TestCase(0x01, "[set_verbose_mode not_supported]")]
        [TestCase(0x02, "[set_verbose_mode error]")]
        public void DecodeResponse(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x09, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x09, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x09_SetVerboseModeResponse_{status:x2}", out IControlArg service);

            ControlResponse response = (ControlResponse)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x09));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.ToString(), Is.EqualTo(result));
        }
    }
}
