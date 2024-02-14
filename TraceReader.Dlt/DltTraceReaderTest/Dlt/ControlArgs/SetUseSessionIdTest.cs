namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetUseSessionIdTest
    {
        [TestCase(true, "[use_session_id] on")]
        [TestCase(false, "[use_session_id] off")]
        public void SetUseSessionIdReq(bool enabled, string result)
        {
            SetUseSessionIdRequest arg = new(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0E));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(ControlResponse.StatusOk, "[use_session_id ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[use_session_id not_supported]")]
        [TestCase(ControlResponse.StatusError, "[use_session_id error]")]
        [TestCase(100, "[use_session_id status=100]")]
        public void SetUseSessionIdResp(int status, string result)
        {
            SetUseSessionIdResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0E));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
