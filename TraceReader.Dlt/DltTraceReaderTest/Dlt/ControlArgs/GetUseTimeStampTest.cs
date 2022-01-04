namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetUseTimeStampTest
    {
        [Test]
        public void GetUseTimeStampReq()
        {
            GetUseTimeStampRequest arg = new GetUseTimeStampRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x1D));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_use_timestamp]"));
        }

        [TestCase(0x00, true, "[get_use_timestamp ok] on")]
        [TestCase(0x00, false, "[get_use_timestamp ok] off")]
        [TestCase(0x01, true, "[get_use_timestamp not_supported]")]
        [TestCase(0x01, false, "[get_use_timestamp not_supported]")]
        [TestCase(0x02, true, "[get_use_timestamp error]")]
        [TestCase(0x02, false, "[get_use_timestamp error]")]
        public void GetUseTimeStampRes(int status, bool enabled, string result)
        {
            GetUseTimeStampResponse arg = new GetUseTimeStampResponse(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1D));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
