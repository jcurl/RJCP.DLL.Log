namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class ResetFactoryDefaultTest
    {
        [Test]
        public void ResetFactoryDefaultReq()
        {
            ResetFactoryDefaultRequest arg = new ResetFactoryDefaultRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x06));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[reset_to_factory_default]"));
        }

        [TestCase(ResetFactoryDefaultResponse.StatusOk, "[reset_to_factory_default ok]")]
        [TestCase(ResetFactoryDefaultResponse.StatusNotSupported, "[reset_to_factory_default not_supported]")]
        [TestCase(ResetFactoryDefaultResponse.StatusError, "[reset_to_factory_default error]")]
        public void ResetFactoryDefaultRes(int status, string result)
        {
            ResetFactoryDefaultResponse arg = new ResetFactoryDefaultResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x06));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
