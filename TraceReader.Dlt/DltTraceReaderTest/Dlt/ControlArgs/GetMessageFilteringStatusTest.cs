namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetMessageFilteringStatusTest
    {
        [Test]
        public void GetMessageFilteringStatusReq()
        {
            GetMessageFilteringStatusRequest arg = new();
            Assert.That(arg.ServiceId, Is.EqualTo(0x1A));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_message_filtering]"));
        }

        [TestCase(0x00, true, "[get_message_filtering ok] on")]
        [TestCase(0x00, false, "[get_message_filtering ok] off")]
        [TestCase(0x01, true, "[get_message_filtering not_supported]")]
        [TestCase(0x01, false, "[get_message_filtering not_supported]")]
        [TestCase(0x02, true, "[get_message_filtering error]")]
        [TestCase(0x02, false, "[get_message_filtering error]")]
        public void GetMessageFilteringStatusRes(int status, bool enabled, string result)
        {
            GetMessageFilteringStatusResponse arg = new(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1A));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
