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
    public class GetUseSessionIdDecoderTest : ControlDecoderTestBase<GetUseSessionIdRequestDecoder, GetUseSessionIdResponseDecoder>
    {
        public GetUseSessionIdDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x1C, typeof(GetUseSessionIdRequest), typeof(GetUseSessionIdResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x1C, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x1C };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1C_GetUseSessionIdRequest", out IControlArg service);

            GetUseSessionIdRequest request = (GetUseSessionIdRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_use_session_id]"));
        }

        [TestCase(0x00, 0x00, "[get_use_session_id ok] off")]
        [TestCase(0x00, 0x01, "[get_use_session_id ok] on")]
        [TestCase(0x00, 0xFF, "[get_use_session_id ok] on")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x1C, 0x00, 0x00, 0x00, status, enabled } :
                new byte[] { 0x00, 0x00, 0x00, 0x1C, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1C_GetUseSessionIdResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetUseSessionIdResponse response = (GetUseSessionIdResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }

        [TestCase(0x01, "[get_use_session_id not_supported]")]
        [TestCase(0x02, "[get_use_session_id error]")]
        public void DecodeResponseError(byte status, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x1C, 0x00, 0x00, 0x00, status } :
                new byte[] { 0x00, 0x00, 0x00, 0x1C, status };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1C_GetUseSessionIdResponse_{status:x2}_Error", out IControlArg service);

            ControlErrorNotSupported response = (ControlErrorNotSupported)service;
            Assert.That(response.ServiceId, Is.EqualTo(0x1C));
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
        }
    }
}
