namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetDefaultTraceStatusTest
    {
        [TestCase(true, "[set_default_trace_status] on")]
        [TestCase(false, "[set_default_trace_status] off")]
        public void SetDefaultTraceStatusReqNoComId(bool enabled, string result)
        {
            SetDefaultTraceStatusRequest arg = new SetDefaultTraceStatusRequest(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x12));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(true, "[set_default_trace_status] on eth0")]
        [TestCase(false, "[set_default_trace_status] off eth0")]
        public void SetDefaultTraceStatusReq(bool enabled, string result)
        {
            SetDefaultTraceStatusRequest arg = new SetDefaultTraceStatusRequest(enabled, "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0x12));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(SetDefaultTraceStatusResponse.StatusOk, "[set_default_trace_status ok]")]
        [TestCase(SetDefaultTraceStatusResponse.StatusNotSupported, "[set_default_trace_status not_supported]")]
        [TestCase(SetDefaultTraceStatusResponse.StatusError, "[set_default_trace_status error]")]
        [TestCase(100, "[set_default_trace_status status=100]")]
        public void SetTraceStatusResp(int status, string result)
        {
            SetDefaultTraceStatusResponse arg = new SetDefaultTraceStatusResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x12));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
