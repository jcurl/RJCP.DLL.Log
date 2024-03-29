﻿namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SetMessageFilteringTest
    {
        [TestCase(true, "[set_message_filtering] on")]
        [TestCase(false, "[set_message_filtering] off")]
        public void SetMessageFilteringReq(bool enabled, string result)
        {
            SetMessageFilteringRequest arg = new(enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0A));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }

        [TestCase(ControlResponse.StatusOk, "[set_message_filtering ok]")]
        [TestCase(ControlResponse.StatusNotSupported, "[set_message_filtering not_supported]")]
        [TestCase(ControlResponse.StatusError, "[set_message_filtering error]")]
        [TestCase(100, "[set_message_filtering status=100]")]
        public void SetMessageFilteringResp(int status, string result)
        {
            SetMessageFilteringResponse arg = new(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0x0A));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
