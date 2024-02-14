namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetUseEcuIdTest
    {
        [TestCase(true, "[use_ecu_id] on")]
        [TestCase(false, "[use_ecu_id] off")]
        public void SetUseEcuIdReq(bool enabled, string result)
        {
            SetUseEcuIdRequest arg = new(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0D));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(ControlResponse.StatusOk, "[use_ecu_id ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[use_ecu_id not_supported]")]
        [TestCase(ControlResponse.StatusError, "[use_ecu_id error]")]
        [TestCase(100, "[use_ecu_id status=100]")]
        public void SetUseEcuIdResp(int status, string result)
        {
            SetUseEcuIdResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0D));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
