namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetTraceStatusTest
    {
        [Test]
        public void GetTraceStatusTestReq()
        {
            GetTraceStatusRequest arg = new("APP1", "CTX1");
            Assert.That(arg.ServiceId, Is.EqualTo(0x1F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_trace_status] APP1 (CTX1)"));
        }

        [Test]
        public void GetTraceStatusTestReq_NoAppCtx()
        {
            GetTraceStatusRequest arg = new("", "");
            Assert.That(arg.ServiceId, Is.EqualTo(0x1F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_trace_status]"));
        }

        [Test]
        public void GetTraceStatusTestReq_NullAppCtx()
        {
            GetTraceStatusRequest arg = new(null, null);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_trace_status]"));
        }

        [TestCase(0x00, true, "[get_trace_status ok] on")]
        [TestCase(0x00, false, "[get_trace_status ok] off")]
        [TestCase(0x01, true, "[get_trace_status not_supported]")]
        [TestCase(0x01, false, "[get_trace_status not_supported]")]
        [TestCase(0x02, true, "[get_trace_status error]")]
        [TestCase(0x02, false, "[get_trace_status error]")]
        public void GetTraceStatusRes(int status, bool enabled, string result)
        {
            GetTraceStatusResponse arg = new(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
