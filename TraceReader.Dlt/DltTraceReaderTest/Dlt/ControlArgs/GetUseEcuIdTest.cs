namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetUseEcuIdTest
    {
        [Test]
        public void GetUseEcuIdReq()
        {
            GetUseEcuIdRequest arg = new GetUseEcuIdRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x1B));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_use_ecu_id]"));
        }

        [TestCase(0x00, true, "[get_use_ecu_id ok] on")]
        [TestCase(0x00, false, "[get_use_ecu_id ok] off")]
        [TestCase(0x01, true, "[get_use_ecu_id not_supported]")]
        [TestCase(0x01, false, "[get_use_ecu_id not_supported]")]
        [TestCase(0x02, true, "[get_use_ecu_id error]")]
        [TestCase(0x02, false, "[get_use_ecu_id error]")]
        public void GetUseEcuIdRes(int status, bool enabled, string result)
        {
            GetUseEcuIdResponse arg = new GetUseEcuIdResponse(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1B));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
