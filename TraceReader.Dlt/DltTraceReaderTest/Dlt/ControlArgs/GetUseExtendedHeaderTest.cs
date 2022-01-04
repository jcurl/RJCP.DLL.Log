namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class GetUseExtendedHeaderTest
    {
        [Test]
        public void GetUseExtendedHeaderReq()
        {
            GetUseExtendedHeaderRequest arg = new GetUseExtendedHeaderRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x1E));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[get_use_extended_header]"));
        }

        [TestCase(0x00, true, "[get_use_extended_header ok] on")]
        [TestCase(0x00, false, "[get_use_extended_header ok] off")]
        [TestCase(0x01, true, "[get_use_extended_header not_supported]")]
        [TestCase(0x01, false, "[get_use_extended_header not_supported]")]
        [TestCase(0x02, true, "[get_use_extended_header error]")]
        [TestCase(0x02, false, "[get_use_extended_header error]")]
        public void GetUseExtendedHeaderRes(int status, bool enabled, string result)
        {
            GetUseExtendedHeaderResponse arg = new GetUseExtendedHeaderResponse(status, enabled);
            Assert.That(arg.ServiceId, Is.EqualTo(0x1E));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.Enabled, Is.EqualTo(enabled));
        }
    }
}
