namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetLocalTimeTest
    {
        [Test]
        public void GetLocalTimeReq()
        {
            GetLocalTimeRequest arg = new();
            Assert.That(arg.ServiceId, Is.EqualTo(0x0C));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_local_time]"));
        }

        [TestCase(ControlResponse.StatusOk, "[get_local_time ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[get_local_time not_supported]")]
        [TestCase(ControlResponse.StatusError, "[get_local_time error]")]
        public void GetLocalTimeRes(int status, string result)
        {
            GetLocalTimeResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0C));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
