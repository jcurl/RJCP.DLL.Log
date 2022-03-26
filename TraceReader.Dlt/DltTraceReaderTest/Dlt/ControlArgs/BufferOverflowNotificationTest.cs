namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class BufferOverflowNotificationTest
    {
        [Test]
        public void BufferOverflowNotificationReq()
        {
            BufferOverflowNotificationRequest arg = new BufferOverflowNotificationRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x23));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[buffer_overflow]"));
        }

        [TestCase(ControlResponse.StatusOk, 0, "[buffer_overflow ok] 0")]
        [TestCase(ControlResponse.StatusOk, 10000, "[buffer_overflow ok] 10000")]
        [TestCase(ControlResponse.StatusOk, -1, "[buffer_overflow ok] 4294967295")]
        [TestCase(ControlResponse.StatusNotSupported, 0, "[buffer_overflow not_supported]")]
        [TestCase(ControlResponse.StatusError, 0, "[buffer_overflow error]")]
        [TestCase(ControlResponse.StatusNotSupported, 10000, "[buffer_overflow not_supported]")]
        [TestCase(ControlResponse.StatusError, 10000, "[buffer_overflow error]")]
        [TestCase(ControlResponse.StatusNotSupported, -1, "[buffer_overflow not_supported]")]
        [TestCase(ControlResponse.StatusError, -1, "[buffer_overflow error]")]
        public void BufferOverflowNotificationRes(int status, int counter, string result)
        {
            BufferOverflowNotificationResponse arg = new BufferOverflowNotificationResponse(status, counter);
            Assert.That(arg.ServiceId, Is.EqualTo(0x23));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Counter, Is.EqualTo(counter));
        }
    }
}
