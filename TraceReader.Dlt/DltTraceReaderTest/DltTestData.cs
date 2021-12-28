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

        public static void IsSkippedLine(this DltFactory factory, DltTraceLineBase line, DateTime time, TimeSpan deviceTime, string ecuId)
        {
            DateTime expectedTime = factory.ExpectedTimeStamp(time);

            Assert.That(line.TimeStamp, Is.EqualTo(expectedTime));
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.EcuId, Is.EqualTo(ecuId));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.DeviceTimeStamp, Is.EqualTo(deviceTime));
            Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
            Assert.That(line.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(line.ContextId, Is.EqualTo(string.Empty));
            // TODO: When line arguments are passed: Assert.That(line.Text, Is.EqualTo("Skipped"));
            Assert.That(line.Features.TimeStamp, Is.EqualTo(expectedTime != DltTime.Default));
            Assert.That(line.Features.EcuId, Is.False);
            Assert.That(line.Features.SessionId, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.EqualTo(deviceTime.Ticks != 0));
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.True);
            Assert.That(line.Features.MessageType, Is.False);
            Assert.That(line.Features.ApplicationId, Is.False);
            Assert.That(line.Features.ContextId, Is.False);
        }

        public static void IsLine1(this DltFactory factory, DltTraceLineBase line, int linenum, int count)
        {
            Assert.That(line, Is.TypeOf<DltTraceLine>());

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
            Assert.That(dltLine.Arguments.Count, Is.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Message 1"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2310 {count} ECU1 APP1 CTX1 50 log info verbose Message 1"));
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
            Assert.That(dltLine.Arguments.Count, Is.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Warning"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2320 {count} ECU1 APP1 CTX1 50 log warn verbose Warning"));
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
            Assert.That(dltLine.Arguments.Count, Is.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Message 3"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.2330 {count} ECU1 APP1 CTX1 50 log info verbose Message 3"));
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
            Assert.That(dltLine.Arguments.Count, Is.EqualTo(1));
            Assert.That(dltLine.Text, Is.EqualTo("Message 2"));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(expectedTime)} 1.3000 {count} ECU1 APP1 CTX1 50 log info verbose Message 2"));
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
    }
}
