namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class StoreConfigurationTest
    {
        [Test]
        public void StoreConfigurationReq()
        {
            StoreConfigurationRequest arg = new();
            Assert.That(arg.ServiceId, Is.EqualTo(0x05));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[store_config]"));
        }

        [TestCase(ControlResponse.StatusOk, "[store_config ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[store_config not_supported]")]
        [TestCase(ControlResponse.StatusError, "[store_config error]")]
        public void StoreConfigurationRes(int status, string result)
        {
            StoreConfigurationResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x05));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
