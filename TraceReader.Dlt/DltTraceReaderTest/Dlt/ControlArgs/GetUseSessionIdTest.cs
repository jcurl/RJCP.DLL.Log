namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetUseSessionIdTest
    {
        [Test]
        public void GetUseSessionIdReq()
        {
            GetUseSessionIdRequest arg = new GetUseSessionIdRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x1C));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_use_session_id]"));
        }

        [TestCase(0x00, true, "[get_use_session_id ok] on")]
        [TestCase(0x00, false, "[get_use_session_id ok] off")]
        [TestCase(0x01, true, "[get_use_session_id not_supported]")]
        [TestCase(0x01, false, "[get_use_session_id not_supported]")]
        [TestCase(0x02, true, "[get_use_session_id error]")]
        [TestCase(0x02, false, "[get_use_session_id error]")]
        public void GetUseSessionIdRes(int status, bool enabled, string result)
        {
            GetUseSessionIdResponse arg = new GetUseSessionIdResponse(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1C));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
