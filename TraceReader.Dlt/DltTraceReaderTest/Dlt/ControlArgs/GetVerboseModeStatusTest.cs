namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetVerboseModeStatusTest
    {
        [Test]
        public void GetVerboseModeStatusReq()
        {
            GetVerboseModeStatusRequest arg = new GetVerboseModeStatusRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x19));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_verbose_mode]"));
        }

        [TestCase(0x00, true, "[get_verbose_mode ok] on")]
        [TestCase(0x00, false, "[get_verbose_mode ok] off")]
        [TestCase(0x01, true, "[get_verbose_mode not_supported]")]
        [TestCase(0x01, false, "[get_verbose_mode not_supported]")]
        [TestCase(0x02, true, "[get_verbose_mode error]")]
        [TestCase(0x02, false, "[get_verbose_mode error]")]
        public void GetVerboseModeStatusRes(int status, bool enabled, string result)
        {
            GetVerboseModeStatusResponse arg = new GetVerboseModeStatusResponse(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x19));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
