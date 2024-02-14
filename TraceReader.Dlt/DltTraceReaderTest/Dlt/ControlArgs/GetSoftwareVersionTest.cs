namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetSoftwareVersionTest
    {
        [Test]
        public void GetSoftwareVersionReq()
        {
            GetSoftwareVersionRequest arg = new();
            Assert.That(arg.ServiceId, Is.EqualTo(0x13));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_software_version]"));
        }

        [TestCase(ControlResponse.StatusOk, "[get_software_version ok] Version")]
        [TestCase(ControlResponse.StatusNotSupported, "[get_software_version not_supported]")]
        [TestCase(ControlResponse.StatusError, "[get_software_version error]")]
        [TestCase(100, "[get_software_version status=100]")]
        public void GetSoftwareVersionResp(int status, string result)
        {
            GetSoftwareVersionResponse arg = new(status, "Version");
            Assert.That(arg.ServiceId, Is.EqualTo(0x13));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.SwVersion, Is.EqualTo("Version"));
        }
    }
}
