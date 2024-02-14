namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetDefaultLogLevelTest
    {
        [Test]
        public void GetDefaultLogLevelReq()
        {
            GetDefaultLogLevelRequest arg = new();
            Assert.That(arg.ServiceId, Is.EqualTo(0x04));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_default_log_level]"));
        }

        [TestCase(ControlResponse.StatusOk, "[get_default_log_level ok] block_all")]
        [TestCase(ControlResponse.StatusNotSupported, "[get_default_log_level not_supported]")]
        [TestCase(ControlResponse.StatusError, "[get_default_log_level error]")]
        [TestCase(100, "[get_default_log_level status=100]")]
        public void GetDefaultLogLevelResp(int status, string result)
        {
            GetDefaultLogLevelResponse arg = new(status, 0);
            Assert.That(arg.ServiceId, Is.EqualTo(0x04));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
