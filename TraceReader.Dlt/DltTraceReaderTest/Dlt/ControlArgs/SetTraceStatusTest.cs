namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]

    public class SetTraceStatusTest
    {
        [TestCase(SetTraceStatusRequest.LogLevelDefault, "[set_trace_status] default")]
        [TestCase(SetTraceStatusRequest.LogLevelDisabled, "[set_trace_status] off")]
        [TestCase(SetTraceStatusRequest.LogLevelEnabled, "[set_trace_status] on")]
        [TestCase(100, "[set_trace_status] status=100")]
        public void SetTraceStatusReqAllNull(int logLevel, string result)
        {
            SetTraceStatusRequest arg = new SetTraceStatusRequest(null, null, logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.TraceStatus, Is.EqualTo(logLevel));
        }

        [TestCase(SetTraceStatusRequest.LogLevelDefault, "[set_trace_status] default APP1 ()")]
        [TestCase(SetTraceStatusRequest.LogLevelDisabled, "[set_trace_status] off APP1 ()")]
        [TestCase(SetTraceStatusRequest.LogLevelEnabled, "[set_trace_status] on APP1 ()")]
        [TestCase(100, "[set_trace_status] status=100 APP1 ()")]
        public void SetTraceStatusReqAppId(int logLevel, string result)
        {
            SetTraceStatusRequest arg = new SetTraceStatusRequest("APP1", null, logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.TraceStatus, Is.EqualTo(logLevel));
        }

        [TestCase(SetTraceStatusRequest.LogLevelDefault, "[set_trace_status] default")]
        [TestCase(SetTraceStatusRequest.LogLevelDisabled, "[set_trace_status] off")]
        [TestCase(SetTraceStatusRequest.LogLevelEnabled, "[set_trace_status] on")]
        [TestCase(100, "[set_trace_status] status=100")]
        public void SetTraceStatusReqAll(int logLevel, string result)
        {
            SetTraceStatusRequest arg = new SetTraceStatusRequest("", "", logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(arg.ContextId, Is.EqualTo(string.Empty));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.TraceStatus, Is.EqualTo(logLevel));
        }

        [TestCase(SetTraceStatusRequest.LogLevelDefault, "[set_trace_status] default APP1 (CTX1)")]
        [TestCase(SetTraceStatusRequest.LogLevelDisabled, "[set_trace_status] off APP1 (CTX1)")]
        [TestCase(SetTraceStatusRequest.LogLevelEnabled, "[set_trace_status] on APP1 (CTX1)")]
        [TestCase(100, "[set_trace_status] status=100 APP1 (CTX1)")]
        public void SetTraceStatusReq(int logLevel, string result)
        {
            SetTraceStatusRequest arg = new SetTraceStatusRequest("APP1", "CTX1", logLevel);
            Assert.That(arg.ServiceId, Is.EqualTo(0x02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
            Assert.That(arg.TraceStatus, Is.EqualTo(logLevel));
        }

        [TestCase(SetTraceStatusRequest.LogLevelDefault, "[set_trace_status] default APP1 (CTX1) SERB")]
        [TestCase(SetTraceStatusRequest.LogLevelDisabled, "[set_trace_status] off APP1 (CTX1) SERB")]
        [TestCase(SetTraceStatusRequest.LogLevelEnabled, "[set_trace_status] on APP1 (CTX1) SERB")]
        [TestCase(100, "[set_trace_status] status=100 APP1 (CTX1) SERB")]
        public void SetTraceStatusReqComIntf(int logLevel, string result)
        {
            SetTraceStatusRequest arg = new SetTraceStatusRequest("APP1", "CTX1", logLevel, "SERB");
            Assert.That(arg.ServiceId, Is.EqualTo(0x02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(arg.ContextId, Is.EqualTo("CTX1"));
            Assert.That(arg.ComInterface, Is.EqualTo("SERB"));
            Assert.That(arg.TraceStatus, Is.EqualTo(logLevel));
        }

        [TestCase(ControlResponse.StatusOk, "[set_trace_status ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[set_trace_status not_supported]")]
        [TestCase(ControlResponse.StatusError, "[set_trace_status error]")]
        [TestCase(100, "[set_trace_status status=100]")]
        public void SetTraceStatusResp(int status, string result)
        {
            SetTraceStatusResponse arg = new SetTraceStatusResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
