namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetUseExtendedHeaderTest
    {
        [TestCase(true, "[use_extended_header] on")]
        [TestCase(false, "[use_extended_header] off")]
        public void SetUseExtendedHeaderReq(bool enabled, string result)
        {
            SetUseExtendedHeaderRequest arg = new SetUseExtendedHeaderRequest(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x10));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(SetUseExtendedHeaderResponse.StatusOk, "[use_extended_header ok]")]
        [TestCase(SetUseExtendedHeaderResponse.StatusNotSupported, "[use_extended_header not_supported]")]
        [TestCase(SetUseExtendedHeaderResponse.StatusError, "[use_extended_header error]")]
        [TestCase(100, "[use_extended_header status=100]")]
        public void SetUseExtendedHeaderResp(int status, string result)
        {
            SetUseExtendedHeaderResponse arg = new SetUseExtendedHeaderResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x10));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
