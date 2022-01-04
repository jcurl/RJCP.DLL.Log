namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class CustomMarkerTest
    {
        [TestCase(CustomMarkerResponse.StatusOk, "MARKER")]
        [TestCase(CustomMarkerResponse.StatusNotSupported, "MARKER")]
        [TestCase(CustomMarkerResponse.StatusError, "MARKER")]
        [TestCase(100, "MARKER")]
        public void CustomMarkerRes(int status, string result)
        {
            CustomMarkerResponse arg = new CustomMarkerResponse(status);
            Assert.That(arg.ServiceId, Is.EqualTo(0xF04));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
        }
    }
}
