namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetDefaultLogLevelTest
    {
        [Test]
        public void GetDefaultLogLevelReq()
        {
            GetDefaultLogLevelRequest arg = new GetDefaultLogLevelRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x04));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_default_log_level]"));
        }

        [TestCase(GetSoftwareVersionResponse.StatusOk, "[get_default_log_level ok] block_all")]
        [TestCase(GetSoftwareVersionResponse.StatusNotSupported, "[get_default_log_level not_supported] block_all")]
        [TestCase(GetSoftwareVersionResponse.StatusError, "[get_default_log_level error] block_all")]
        [TestCase(100, "[get_default_log_level status=100] block_all")]
        public void GetDefaultLogLevelResp(int status, string result)
        {
            GetDefaultLogLevelResponse arg = new GetDefaultLogLevelResponse(status, 0);
            Assert.That(arg.ServiceId, Is.EqualTo(0x04));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
