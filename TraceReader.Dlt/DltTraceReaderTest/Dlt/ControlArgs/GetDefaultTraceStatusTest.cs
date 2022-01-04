namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetDefaultTraceStatusTest
    {
        [Test]
        public void GetDefaultTraceStatusTestReq()
        {
            GetDefaultTraceStatusRequest arg = new GetDefaultTraceStatusRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x15));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_default_trace_status]"));
        }

        [TestCase(0x00, true, "[get_default_trace_status ok] on")]
        [TestCase(0x00, false, "[get_default_trace_status ok] off")]
        [TestCase(0x01, true, "[get_default_trace_status not_supported]")]
        [TestCase(0x01, false, "[get_default_trace_status not_supported]")]
        [TestCase(0x02, true, "[get_default_trace_status error]")]
        [TestCase(0x02, false, "[get_default_trace_status error]")]
        public void GetDefaultTraceStatusRes(int status, bool enabled, string result)
        {
            GetDefaultTraceStatusResponse arg = new GetDefaultTraceStatusResponse(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x15));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
