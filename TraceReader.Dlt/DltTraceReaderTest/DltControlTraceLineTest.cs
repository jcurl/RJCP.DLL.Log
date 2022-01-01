namespace RJCP.Diagnostics.Log
{
    using System;
    using Dlt;
    using Dlt.ControlArgs;
    using NUnit.Framework;

    [TestFixture]
    public class DltControlTraceLineTest
    {
        [Test]
        public void CustomControlRequest()
        {
            DltControlTraceLine line = new DltControlTraceLine(new CustomControlRequest());

            Assert.That(line.ApplicationId, Is.EqualTo(null));
            Assert.That(line.ContextId, Is.EqualTo(null));
            Assert.That(line.EcuId, Is.EqualTo(null));
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.Type, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(line.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Text, Is.EqualTo("[custom_control_request]"));
            Assert.That(line.Service.ServiceId, Is.EqualTo(0x1000));
            Assert.That(line.Service.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));

            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.Features.ContextId, Is.True);
            Assert.That(line.Features.EcuId, Is.False);
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
            Assert.That(line.Features.TimeStamp, Is.False);
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Features.SessionId, Is.False);
        }

        [Test]
        public void CustomControlResponse()
        {
            DltControlTraceLine line = new DltControlTraceLine(new CustomControlResponse());

            Assert.That(line.ApplicationId, Is.EqualTo(null));
            Assert.That(line.ContextId, Is.EqualTo(null));
            Assert.That(line.EcuId, Is.EqualTo(null));
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.Type, Is.EqualTo(DltType.CONTROL_RESPONSE));
            Assert.That(line.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Text, Is.EqualTo("[custom_control_response]"));
            Assert.That(line.Service.ServiceId, Is.EqualTo(0x1000));
            Assert.That(line.Service.DefaultType, Is.EqualTo(DltType.CONTROL_RESPONSE));

            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.Features.ContextId, Is.True);
            Assert.That(line.Features.EcuId, Is.False);
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
            Assert.That(line.Features.TimeStamp, Is.False);
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Features.SessionId, Is.False);
        }

        [Test]
        public void NullService()
        {
            Assert.That(() => {
                _ = new DltControlTraceLine(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
