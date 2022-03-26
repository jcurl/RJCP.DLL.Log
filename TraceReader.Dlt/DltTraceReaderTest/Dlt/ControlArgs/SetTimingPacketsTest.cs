namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetTimingPacketsTest
    {
        [TestCase(true, "[set_timing_packets] on")]
        [TestCase(false, "[set_timing_packets] off")]
        public void SetTimingPacketsReq(bool enabled, string result)
        {
            SetTimingPacketsRequest arg = new SetTimingPacketsRequest(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0B));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(ControlResponse.StatusOk, "[set_timing_packets ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[set_timing_packets not_supported]")]
        [TestCase(ControlResponse.StatusError, "[set_timing_packets error]")]
        [TestCase(100, "[set_timing_packets status=100]")]
        public void SetVerboseModeResp(int status, string result)
        {
            SetTimingPacketsResponse arg = new SetTimingPacketsResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0B));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
