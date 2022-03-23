namespace RJCP.Diagnostics.Log
{
    using Dlt;
    using NUnit.Framework;

    [TestFixture]
    public class DltSkippedTraceLineTest
    {
        [Test]
        public void DefaultSkippedTraceLine()
        {
            DltSkippedTraceLine line = new DltSkippedTraceLine(10, "none");
            Assert.That(line.Arguments.Count, Is.EqualTo(4));
            Assert.That(line.ApplicationId, Is.EqualTo("SKIP"));
            Assert.That(line.ContextId, Is.EqualTo("SKIP"));
            Assert.That(line.EcuId, Is.EqualTo(string.Empty));
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
            Assert.That(line.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Text, Is.EqualTo("Skipped: 10 bytes; Reason: none"));
            Assert.That(line.BytesSkipped, Is.EqualTo(10));
            Assert.That(line.Reason, Is.EqualTo("none"));

            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.Features.ContextId, Is.True);
            Assert.That(line.Features.EcuId, Is.True);
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.True);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
            Assert.That(line.Features.TimeStamp, Is.False);
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Features.SessionId, Is.False);

            Assert.That(line.Arguments.IsReadOnly, Is.True);
        }
    }
}
