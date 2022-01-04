namespace RJCP.Diagnostics.Log.Dlt.Control
{
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture(DecoderType.Line)]
    [TestFixture(DecoderType.Packet)]
    [TestFixture(DecoderType.Specialized)]
    public class GetTraceStatusDecoderTest : ControlDecoderTestBase<GetTraceStatusRequestDecoder, GetTraceStatusResponseDecoder>
    {
        public GetTraceStatusDecoderTest(DecoderType decoderType)
            : base(decoderType, 0x1F, typeof(GetTraceStatusRequest), typeof(GetTraceStatusResponse))
        { }

        [Test]
        public void DecodeRequest()
        {
            byte[] payload = new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x41, 0x50, 0x50, 0x31, 0x43, 0x54, 0x58, 0x31 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1F_GetTraceStatusRequest", out IControlArg service);

            GetTraceStatusRequest request = (GetTraceStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_trace_status] APP1 (CTX1)"));
            Assert.That(request.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(request.ContextId, Is.EqualTo("CTX1"));
        }

        [Test]
        public void DecodeRequest_NoAppCtx()
        {
            byte[] payload = new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Decode(DltType.CONTROL_REQUEST, payload, "0x1F_GetTraceStatusRequest", out IControlArg service);

            GetTraceStatusRequest request = (GetTraceStatusRequest)service;
            Assert.That(request.ToString(), Is.EqualTo("[get_trace_status]"));
            Assert.That(request.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(request.ContextId, Is.EqualTo(string.Empty));
        }

        [TestCase(0x00, 0x00, "[get_trace_status ok] off")]
        [TestCase(0x00, 0x01, "[get_trace_status ok] on")]
        [TestCase(0x00, 0xFF, "[get_trace_status ok] on")]
        [TestCase(0x01, 0x00, "[get_trace_status not_supported]")]
        [TestCase(0x01, 0x01, "[get_trace_status not_supported]")]
        [TestCase(0x01, 0xFF, "[get_trace_status not_supported]")]
        [TestCase(0x02, 0x00, "[get_trace_status error]")]
        [TestCase(0x02, 0x01, "[get_trace_status error]")]
        [TestCase(0x02, 0xFF, "[get_trace_status error]")]
        public void DecodeResponse(byte status, byte enabled, string result)
        {
            byte[] payload = new byte[] { 0x1F, 0x00, 0x00, 0x00, status, enabled };
            Decode(DltType.CONTROL_RESPONSE, payload, $"0x1F_GetDefaultTraceStatusResponse_{status:x2}_{enabled:x2}", out IControlArg service);

            GetTraceStatusResponse response = (GetTraceStatusResponse)service;
            Assert.That(response.ToString(), Is.EqualTo(result));
            Assert.That(response.Status, Is.EqualTo(status));
            Assert.That(response.Enabled, Is.EqualTo(enabled != 0));
        }
    }
}
