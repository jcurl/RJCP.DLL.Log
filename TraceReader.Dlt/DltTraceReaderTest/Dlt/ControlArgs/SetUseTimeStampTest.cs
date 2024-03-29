﻿namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetUseTimeStampTest
    {
        [TestCase(true, "[use_timestamp] on")]
        [TestCase(false, "[use_timestamp] off")]
        public void SetUseTimeStampReq(bool enabled, string result)
        {
            SetUseTimeStampRequest arg = new(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(ControlResponse.StatusOk, "[use_timestamp ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[use_timestamp not_supported]")]
        [TestCase(ControlResponse.StatusError, "[use_timestamp error]")]
        [TestCase(100, "[use_timestamp status=100]")]
        public void SetUseTimeStampResp(int status, string result)
        {
            SetUseTimeStampResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0F));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
