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
    public class GetVerboseModeStatusDecoderTest : ControlDecoderTestBase<GetVerboseModeStatusRequestDecoder, GetVerboseModeStatusResponseDecoder>
    {
        public GetVerboseModeStatusDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x19, typeof(GetVerboseModeStatusRequest), typeof(GetVerboseModeStatusResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x19, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x19 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x19_GetVerboseModeStatusRequest", out IControlArg service);

            GetVerboseModeStatusRequest request = (GetVerboseModeStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_verbose_mode]"));
        }

        [TestCase(0x00, 0x00, "[get_verbose_mode ok] off")]
        [TestCase(0x00, 0x01, "[get_verbose_mode ok] on")]
        [TestCase(0x00, 0xFF, "[get_verbose_mode ok] on")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x19, 0x00, 0x00, 0x00, status, enabled } :
                new byte[] { 0x00, 0x00, 0x00, 0x19, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x19_GetVerboseModeStatusResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetVerboseModeStatusResponse response = (GetVerboseModeStatusResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }

        [TestCase(0x01, "[get_verbose_mode not_supported]")]
        [TestCase(0x02, "[get_verbose_mode error]")]
        public void DecodeResponseError(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x19, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x19, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x19_GetVerboseModeStatusResponse_{status:x2}_Error", out IControlArg service);

            ControlErrorNotSupported response = (ControlErrorNotSupported)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x19));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
        }
    }
}
