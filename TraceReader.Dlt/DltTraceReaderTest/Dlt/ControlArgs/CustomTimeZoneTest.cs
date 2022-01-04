namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using NUnit.Framework;

    [TestFixture]
    public class CustomTimeZoneTest
    {
        [TestCase(CustomTimeZoneResponse.StatusOk, 34200, false, "[timezone ok] +09:30")]
        [TestCase(CustomTimeZoneResponse.StatusNotSupported, 34200, false, "[timezone not_supported] +09:30")]
        [TestCase(CustomTimeZoneResponse.StatusError, 34200, false, "[timezone error] +09:30")]
        [TestCase(100, 34200, false, "[timezone status=100] +09:30")]
        [TestCase(CustomTimeZoneResponse.StatusOk, 37800, true, "[timezone ok] +10:30 DST")]
        [TestCase(CustomTimeZoneResponse.StatusNotSupported, 37800, true, "[timezone not_supported] +10:30 DST")]
        [TestCase(CustomTimeZoneResponse.StatusError, 37800, true, "[timezone error] +10:30 DST")]
        [TestCase(100, 37800, true, "[timezone status=100] +10:30 DST")]
        [TestCase(CustomTimeZoneResponse.StatusOk, -21600, false, "[timezone ok] -06:00")]
        [TestCase(CustomTimeZoneResponse.StatusNotSupported, -21600, false, "[timezone not_supported] -06:00")]
        [TestCase(CustomTimeZoneResponse.StatusError, -21600, false, "[timezone error] -06:00")]
        [TestCase(100, -21600, false, "[timezone status=100] -06:00")]
        [TestCase(CustomTimeZoneResponse.StatusOk, -18000, true, "[timezone ok] -05:00 DST")]
        [TestCase(CustomTimeZoneResponse.StatusNotSupported, -18000, true, "[timezone not_supported] -05:00 DST")]
        [TestCase(CustomTimeZoneResponse.StatusError, -18000, true, "[timezone error] -05:00 DST")]
        [TestCase(100, -18000, true, "[timezone status=100] -05:00 DST")]
        public void CustomTimeZoneRes(int status, int timeZone, bool isDst, string result)
        {
            CustomTimeZoneResponse arg = new CustomTimeZoneResponse(status, timeZone, isDst);
            Assert.That(arg.ServiceId, Is.EqualTo(0xF03));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.TimeZone.TotalSeconds, Is.EqualTo(timeZone));
            Assert.That(arg.IsDst, Is.EqualTo(isDst));
        }
    }
}
