namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class MessageBufferOverflowTest
    {
        [Test]
        public void MessageBufferOverflowReq()
        {
            MessageBufferOverflowRequest arg = new MessageBufferOverflowRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x14));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[message_buffer_overflow]"));
        }

        [TestCase(0x00, true, "[message_buffer_overflow ok] true")]
        [TestCase(0x00, false, "[message_buffer_overflow ok] false")]
        [TestCase(0x01, true, "[message_buffer_overflow not_supported]")]
        [TestCase(0x01, false, "[message_buffer_overflow not_supported]")]
        [TestCase(0x02, true, "[message_buffer_overflow error]")]
        [TestCase(0x02, false, "[message_buffer_overflow error]")]
        public void MessageBufferOverflowRes(int status, bool overflow, string result)
        {
            MessageBufferOverflowResponse arg = new MessageBufferOverflowResponse(status, overflow);
            Assert.That(arg.ServiceId, Is.EqualTo(0x14));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Overflow, Is.EqualTo(overflow));
        }
    }
}
