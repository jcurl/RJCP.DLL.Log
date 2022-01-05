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
    public class GetDefaultTraceStatusDecoderTest : ControlDecoderTestBase<GetDefaultTraceStatusRequestDecoder, GetDefaultTraceStatusResponseDecoder>
    {
        public GetDefaultTraceStatusDecoderTest(DecoderType decoderType, Endianness endian)
            : base(decoderType, endian, 0x15, typeof(GetDefaultTraceStatusRequest), typeof(GetDefaultTraceStatusResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x15, 0x00, 0x00, 0x00 } :
                new byte[] { 0x00, 0x00, 0x00, 0x15 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x15_GetDefaultTraceStatusRequest", out IControlArg service);

            GetDefaultTraceStatusRequest request = (GetDefaultTraceStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_default_trace_status]"));
        }

        [TestCase(0x00, 0x00, "[get_default_trace_status ok] off")]
        [TestCase(0x00, 0x01, "[get_default_trace_status ok] on")]
        [TestCase(0x00, 0xFF, "[get_default_trace_status ok] on")]
        [TestCase(0x01, 0x00, "[get_default_trace_status not_supported]")]
        [TestCase(0x01, 0x01, "[get_default_trace_status not_supported]")]
        [TestCase(0x01, 0xFF, "[get_default_trace_status not_supported]")]
        [TestCase(0x02, 0x00, "[get_default_trace_status error]")]
        [TestCase(0x02, 0x01, "[get_default_trace_status error]")]
        [TestCase(0x02, 0xFF, "[get_default_trace_status error]")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = Endian == Endianness.Little ?
                new byte[] { 0x15, 0x00, 0x00, 0x00, status, enabled } :
                new byte[] { 0x00, 0x00, 0x00, 0x15, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x15_GetDefaultTraceStatusResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetDefaultTraceStatusResponse response = (GetDefaultTraceStatusResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }
    }
}
