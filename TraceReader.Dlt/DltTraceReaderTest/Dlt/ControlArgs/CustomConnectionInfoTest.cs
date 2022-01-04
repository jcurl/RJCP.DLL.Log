namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class CustomConnectionInfoTest
    {
        [TestCase(CustomConnectionInfoResponse.StatusOk, 0x00, "[connection_info ok] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0x00, "[connection_info not_supported] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0x00, "[connection_info error] unknown eth0")]
        [TestCase(100, 0x00, "[connection_info status=100] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusOk, 0x01, "[connection_info ok] disconnected eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0x01, "[connection_info not_supported] disconnected eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0x01, "[connection_info error] disconnected eth0")]
        [TestCase(100, 0x01, "[connection_info status=100] disconnected eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusOk, 0x02, "[connection_info ok] connected eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0x02, "[connection_info not_supported] connected eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0x02, "[connection_info error] connected eth0")]
        [TestCase(100, 0x02, "[connection_info status=100] connected eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusOk, 0xFF, "[connection_info ok] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0xFF, "[connection_info not_supported] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0xFF, "[connection_info error] unknown eth0")]
        [TestCase(100, 0xFF, "[connection_info status=100] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusOk, -1, "[connection_info ok] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, -1, "[connection_info not_supported] unknown eth0")]
        [TestCase(CustomConnectionInfoResponse.StatusError, -1, "[connection_info error] unknown eth0")]
        [TestCase(100, -1, "[connection_info status=100] unknown eth0")]
        public void CustomConnectionInfoRes(int status, int state, string result)
        {
            CustomConnectionInfoResponse arg = new CustomConnectionInfoResponse(status, state, "eth0");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ConnectionState, Is.EqualTo(state));
            Assert.That(arg.ComInterface, Is.EqualTo("eth0"));
        }

        [TestCase(CustomConnectionInfoResponse.StatusOk, 0x00, "[connection_info ok] unknown")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0x00, "[connection_info not_supported] unknown")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0x00, "[connection_info error] unknown")]
        [TestCase(100, 0x00, "[connection_info status=100] unknown")]
        [TestCase(CustomConnectionInfoResponse.StatusOk, 0x01, "[connection_info ok] disconnected")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0x01, "[connection_info not_supported] disconnected")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0x01, "[connection_info error] disconnected")]
        [TestCase(100, 0x01, "[connection_info status=100] disconnected")]
        [TestCase(CustomConnectionInfoResponse.StatusOk, 0x02, "[connection_info ok] connected")]
        [TestCase(CustomConnectionInfoResponse.StatusNotSupported, 0x02, "[connection_info not_supported] connected")]
        [TestCase(CustomConnectionInfoResponse.StatusError, 0x02, "[connection_info error] connected")]
        [TestCase(100, 0x02, "[connection_info status=100] connected")]
        public void CustomConnectionInfoRes_NoComId(int status, int state, string result)
        {
            CustomConnectionInfoResponse arg = new CustomConnectionInfoResponse(status, state, "");
            Assert.That(arg.ServiceId, Is.EqualTo(0xF02));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.ConnectionState, Is.EqualTo(state));
            Assert.That(arg.ComInterface, Is.EqualTo(string.Empty));
        }
    }
}
