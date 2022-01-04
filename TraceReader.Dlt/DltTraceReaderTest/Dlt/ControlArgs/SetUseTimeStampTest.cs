namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetUseTimeStampTest
    {
        [TestCase(true, "[use_timestamp] on")]
        [TestCase(false, "[use_timestamp] off")]
        public void SetUseTimeStampReq(bool enabled, string result)
        {
            SetUseTimeStampRequest arg = new SetUseTimeStampRequest(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(SetUseTimeStampResponse.StatusOk, "[use_timestamp ok]")]
        [TestCase(SetUseTimeStampResponse.StatusNotSupported, "[use_timestamp not_supported]")]
        [TestCase(SetUseTimeStampResponse.StatusError, "[use_timestamp error]")]
        [TestCase(100, "[use_timestamp status=100]")]
        public void SetUseTimeStampResp(int status, string result)
        {
            SetUseTimeStampResponse arg = new SetUseTimeStampResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
