namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class SyncTimeStampTest
    {
        [Test]
        public void SyncTimeStampReq()
        {
            SyncTimeStampRequest arg = new SyncTimeStampRequest();
            Assert.That(arg.ServiceId, Is.EqualTo(0x24));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(arg.ToString(), Is.EqualTo("[sync_timestamp]"));
        }

        [TestCase(ControlResponse.StatusOk, "[sync_timestamp ok] 2022-01-05 10:28:34.55600Z")]
        [TestCase(ControlResponse.StatusNotSupported, "[sync_timestamp not_supported]")]
        [TestCase(ControlResponse.StatusError, "[sync_timestamp error]")]
        public void GetLocalTimeRes(int status, string result)
        {
            SyncTimeStampResponse arg = new SyncTimeStampResponse(status, new DateTime(2022, 1, 5, 10, 28, 34, 556, DateTimeKind.Utc));
            Assert.That(arg.ServiceId, Is.EqualTo(0x24));
            Assert.That(arg.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(arg.ToString(), Is.EqualTo(result));
            Assert.That(arg.TimeStamp, Is.EqualTo(new DateTime(2022, 1, 5, 10, 28, 34, 556, DateTimeKind.Utc)));
        }
    }
}
