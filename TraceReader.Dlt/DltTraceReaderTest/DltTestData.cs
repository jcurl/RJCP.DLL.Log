namespace RJCP.Diagnostics.Log
{
    using System;
    using Dlt;
    using NUnit.Framework;

    internal static class DltTestData
    {
        public static readonly DateTime Time1 = DltTime.FileTime(2021, 12, 16, 20, 59, 35.2376);
        public static readonly DateTime Time2 = DltTime.FileTime(2021, 12, 16, 20, 59, 35.2377);
        public static readonly DateTime Time3 = DltTime.FileTime(2021, 12, 16, 20, 59, 35.2378);
        public static readonly DateTime Time4 = DltTime.FileTime(2021, 12, 16, 20, 59, 35.556);
        public static readonly DateTime Time5 = DltTime.FileTime(2021, 12, 16, 20, 59, 35.5767);

        public static void IsSkippedLine(this DltFactory factory, DltTraceLineBase line, DateTime time)
        {
            IsSkippedLine(factory, line, time, -1);
        }

        public static void IsSkippedLine(this DltFactory factory, DltTraceLineBase line, DateTime time, long bytes)
        {
            Assert.That(line, Is.TypeOf<DltSkippedTraceLine>());

            DateTime expectedTime = factory.ExpectedTimeStamp(time);
            DltSkippedTraceLine skipLine = (DltSkippedTraceLine)line;
            Assert.That(skipLine.TimeStamp, Is.EqualTo(expectedTime));
            Assert.That(skipLine.Count, Is.EqualTo(-1));
            Assert.That(skipLine.EcuId, Is.EqualTo(string.Empty));
            Assert.That(skipLine.SessionId, Is.EqualTo(0));
            Assert.That(skipLine.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(skipLine.Type, Is.EqualTo(DltType.LOG_WARN));
            Assert.That(skipLine.ApplicationId, Is.EqualTo("SKIP"));
            Assert.That(skipLine.ContextId, Is.EqualTo("SKIP"));
            Assert.That(skipLine.Arguments, Has.Count.EqualTo(4));
            Assert.That(skipLine.Reason, Is.Not.Null);
            if (bytes > 0) Assert.That(skipLine.BytesSkipped, Is.EqualTo(bytes));
            Assert.That(skipLine.Text, Is.EqualTo($"Skipped: {skipLine.BytesSkipped} bytes; Reason: {skipLine.Reason}"));
            Assert.That(skipLine.Features.TimeStamp, Is.EqualTo(expectedTime != DltTime.Default));
            Assert.That(skipLine.Features.EcuId, Is.True);
            Assert.That(skipLine.Features.SessionId, Is.False);
            Assert.That(skipLine.Features.DeviceTimeStamp, Is.False);
            Assert.That(skipLine.Features.BigEndian, Is.False);
            Assert.That(skipLine.Features.IsVerbose, Is.True);
            Assert.That(skipLine.Features.MessageType, Is.True);
            Assert.That(skipLine.Features.ApplicationId, Is.True);
            Assert.That(skipLine.Features.ContextId, Is.True);
        }

        public static void IsLine1(this DltFactory factory, DltTraceLineBase line, int linenum, int count)
        {
            factory.IsLine1(line, linenum, count, false);
        }

        public static void IsLine1(this DltFactory factory, DltTraceLineBase line, int linenum, int count, bool nv)
        {
            if (!nv) {
                Assert.That(line, Is.TypeOf<DltTraceLine>());
            } else {
                Assert.That(line, Is.TypeOf<DltNonVerboseTraceLine>());
            }

            DateTime expectedTime = factory.ExpectedTimeStamp(Time1);
            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(linenum));
            Assert.That(dltLine.TimeStamp, Is.EqualTo(expectedTime));
            Assert.That(dltLine.Count, Is.EqualTo(count));
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.SessionId, Is.EqualTo(50));
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.231)));
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
            if (!nv) {
                Assert.That(dltLine.Text, Is.EqualTo("Message 1"));
                Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2310 {count} ECU1 APP1 CTX1 50 log info verbose 1 Message 1"));
            } else {
                Assert.That(dltLine.Text, Is.EqualTo("[1] Message 1"));
                Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2310 {count} ECU1 APP1 CTX1 50 log info non-verbose 1 [1] Message 1"));
            }
            Assert.That(dltLine.Features.TimeStamp, Is.EqualTo(factory.FactoryType == DltFactoryType.File));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.Features.SessionId, Is.True);
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.Not.EqualTo(nv));
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.Features.ContextId, Is.True);
        }

        public static void IsLine2(this DltFactory factory, DltTraceLineBase line, int linenum, int count)
        {
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            DateTime expectedTime = factory.ExpectedTimeStamp(Time2);
            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(linenum));
            Assert.That(dltLine.TimeStamp, Is.EqualTo(expectedTime));
            Assert.That(dltLine.Count, Is.EqualTo(count));
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.SessionId, Is.EqualTo(50));
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.232)));
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_WARN));
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Warning"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2320 {count} ECU1 APP1 CTX1 50 log warn verbose 1 Warning"));
            Assert.That(dltLine.Features.TimeStamp, Is.EqualTo(factory.FactoryType == DltFactoryType.File));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.Features.SessionId, Is.True);
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.True);
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.Features.ContextId, Is.True);
        }

        public static void IsLine3(this DltFactory factory, DltTraceLineBase line, int lineNum, int count)
        {
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            DateTime expectedTime = factory.ExpectedTimeStamp(Time3);
            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(lineNum));
            Assert.That(dltLine.TimeStamp, Is.EqualTo(expectedTime));
            Assert.That(dltLine.Count, Is.EqualTo(count));
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.SessionId, Is.EqualTo(50));
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.233)));
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Message 3"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2330 {count} ECU1 APP1 CTX1 50 log info verbose 1 Message 3"));
            Assert.That(dltLine.Features.TimeStamp, Is.EqualTo(factory.FactoryType == DltFactoryType.File));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.Features.SessionId, Is.True);
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.True);
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.Features.ContextId, Is.True);
        }

        public static void IsLine5(this DltFactory factory, DltTraceLineBase line, int lineNum, int count)
        {
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            DateTime expectedTime = factory.ExpectedTimeStamp(Time5);
            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(lineNum));
            Assert.That(dltLine.TimeStamp, Is.EqualTo(expectedTime));
            Assert.That(dltLine.Count, Is.EqualTo(count));
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.SessionId, Is.EqualTo(50));
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(1.3)));
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Arguments, Has.Count.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Message 2"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.3000 {count} ECU1 APP1 CTX1 50 log info verbose 1 Message 2"));
            Assert.That(dltLine.Features.TimeStamp, Is.EqualTo(factory.FactoryType == DltFactoryType.File));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.Features.SessionId, Is.True);
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.True);
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.Features.ContextId, Is.True);
        }

        private static DateTime ExpectedTimeStamp(this DltFactory factory, DateTime storageTime)
        {
            switch (factory.FactoryType) {
            case DltFactoryType.Standard:
            case DltFactoryType.Serial:
                return DateTime.UnixEpoch;
            case DltFactoryType.File:
                return storageTime;
            default:
                throw new InvalidOperationException($"Unknown Factory {factory.FactoryType}");
            }
        }
    }
}
