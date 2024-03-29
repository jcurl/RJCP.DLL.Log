﻿namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetVerboseModeTest
    {
        [TestCase(true, "[set_verbose_mode] on")]
        [TestCase(false, "[set_verbose_mode] off")]
        public void SetVerboseModeReq(bool enabled, string result)
        {
            SetVerboseModeRequest arg = new(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x09));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(ControlResponse.StatusOk, "[set_verbose_mode ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[set_verbose_mode not_supported]")]
        [TestCase(ControlResponse.StatusError, "[set_verbose_mode error]")]
        [TestCase(100, "[set_verbose_mode status=100]")]
        public void SetVerboseModeResp(int status, string result)
        {
            SetVerboseModeResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x09));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
