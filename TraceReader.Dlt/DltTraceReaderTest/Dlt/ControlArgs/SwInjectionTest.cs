namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class SwInjectionTest
    {
        [TestCase(0xFFF, new byte[] { 0x11, 0x22, 0x33, 0x44 }, "[] 04 00 00 00 11 22 33 44")]
        [TestCase(-1, new byte[] { 0x12, 0x34, 0x56, 0x67, 0x89 }, "[] 05 00 00 00 12 34 56 67 89")]
        public void SwInjectionReq(int serviceId, byte[] payload, string result)
        {
            SwInjectionRequest arg = new(serviceId, payload);
            Assert.That(arg.ServiceId, Is.EqualTo(serviceId));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Payload, Is.EqualTo(payload));
        }

        [TestCase(0xFFF, ControlResponse.StatusOk, "[ ok]")]
        [TestCase(0xFFF, ControlResponse.StatusNotSupported, "[ not_supported]")]
        [TestCase(0xFFF, ControlResponse.StatusError, "[ error]")]
        [TestCase(0xFFF, SwInjectionResponse.StatusPending, "[ pending]")]
        [TestCase(0xFFF, 100, "[ status=100]")]
        public void SwInjectionResp(int serviceId, int status, string result)
        {
            SwInjectionResponse arg = new(serviceId, status);
            Assert.That(arg.ServiceId, Is.EqualTo(serviceId));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Status, Is.EqualTo(status));
        }
    }
}
