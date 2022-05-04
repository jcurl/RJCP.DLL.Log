namespace RJCP.Diagnostics.Log.Dlt
{
    using System;
    using System.Threading;
    using Args;
    using ControlArgs;
    using NUnit.Framework;

    [TestFixture]
    public class DltLineBuilderTest
    {
        [Test]
        public void DefaultBuilder()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            Assert.That(builder.EcuId, Is.Null);
            Assert.That(builder.ApplicationId, Is.Null);
            Assert.That(builder.ContextId, Is.Null);
            Assert.That(builder.Count, Is.EqualTo(DltTraceLineBase.InvalidCounter));
            Assert.That(builder.DltType, Is.EqualTo(DltType.UNKNOWN));
            Assert.That(builder.DeviceTimeStamp, Is.EqualTo(new TimeSpan(0)));
            Assert.That(builder.IsVerbose, Is.False);
            Assert.That(builder.SessionId, Is.EqualTo(0));
            Assert.That(builder.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(builder.BigEndian, Is.False);
            Assert.That(builder.Position, Is.EqualTo(0));
            Assert.That(builder.NumberOfArgs, Is.EqualTo(0));
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultLine()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);   // Must set this to know what to build

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            // The features shouldn't be set, as nothing was assigned to the builder
            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(0));
            Assert.That(dltLine.Position, Is.EqualTo(0));
            Assert.That(dltLine.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 0.0000 -1    0 log info non-verbose 0 "));
            Assert.That(dltLine.Text, Is.EqualTo(string.Empty));
            Assert.That(dltLine.Features.EcuId, Is.False);
            Assert.That(dltLine.EcuId, Is.EqualTo(string.Empty));
            Assert.That(dltLine.Features.ApplicationId, Is.False);
            Assert.That(dltLine.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(dltLine.Features.ContextId, Is.False);
            Assert.That(dltLine.ContextId, Is.EqualTo(string.Empty));
            Assert.That(dltLine.Count, Is.EqualTo(DltTraceLineBase.InvalidCounter));
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.Features.SessionId, Is.False);
            Assert.That(dltLine.SessionId, Is.EqualTo(0));
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.False);
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(new TimeSpan(0)));
            Assert.That(dltLine.Features.TimeStamp, Is.False);
            Assert.That(dltLine.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.False);
            Assert.That(dltLine.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void ConstructFullVerboseLine()
        {
            DateTime time = DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634);

            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(time);
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetSessionId(1435);
            builder.SetDeviceTimeStamp(DltTime.DeviceTime(5.5352).Ticks);
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetIsVerbose(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(10);
            builder.SetNumberOfArgs(1);
            builder.AddArgument(new CustomArg());

            Assert.That(builder.EcuId, Is.EqualTo("ECU1"));
            Assert.That(builder.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(builder.ContextId, Is.EqualTo("CTX1"));
            Assert.That(builder.Count, Is.EqualTo(127));
            Assert.That(builder.DltType, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(builder.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.5352)));
            Assert.That(builder.IsVerbose, Is.True);
            Assert.That(builder.SessionId, Is.EqualTo(1435));
            Assert.That(builder.TimeStamp, Is.EqualTo(time));
            Assert.That(builder.BigEndian, Is.False);
            Assert.That(builder.Position, Is.EqualTo(10));

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(0));
            Assert.That(dltLine.Position, Is.EqualTo(10));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(time)} 5.5352 127 ECU1 APP1 CTX1 1435 log info verbose 1 custom"));
            Assert.That(dltLine.Text, Is.EqualTo("custom"));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.Features.ContextId, Is.True);
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Count, Is.EqualTo(127));
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.Features.SessionId, Is.True);
            Assert.That(dltLine.SessionId, Is.EqualTo(1435));
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.5352)));
            Assert.That(dltLine.Features.TimeStamp, Is.True);
            Assert.That(dltLine.TimeStamp, Is.EqualTo(time));
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.True);
        }

        [Test]
        public void ConstructFullVerboseLine2Args()
        {
            DateTime time = DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634);

            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(time);
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetSessionId(1435);
            builder.SetDeviceTimeStamp(DltTime.DeviceTime(5.5352).Ticks);
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetIsVerbose(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(10);
            builder.SetNumberOfArgs(2);
            builder.AddArgument(new CustomArg());
            builder.AddArgument(new CustomArg("foo"));

            Assert.That(builder.EcuId, Is.EqualTo("ECU1"));
            Assert.That(builder.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(builder.ContextId, Is.EqualTo("CTX1"));
            Assert.That(builder.Count, Is.EqualTo(127));
            Assert.That(builder.DltType, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(builder.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.5352)));
            Assert.That(builder.IsVerbose, Is.True);
            Assert.That(builder.SessionId, Is.EqualTo(1435));
            Assert.That(builder.TimeStamp, Is.EqualTo(time));
            Assert.That(builder.BigEndian, Is.False);
            Assert.That(builder.Position, Is.EqualTo(10));

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(0));
            Assert.That(dltLine.Position, Is.EqualTo(10));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(time)} 5.5352 127 ECU1 APP1 CTX1 1435 log info verbose 2 custom foo"));
            Assert.That(dltLine.Text, Is.EqualTo("custom foo"));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.Features.ContextId, Is.True);
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Count, Is.EqualTo(127));
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.Features.SessionId, Is.True);
            Assert.That(dltLine.SessionId, Is.EqualTo(1435));
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.5352)));
            Assert.That(dltLine.Features.TimeStamp, Is.True);
            Assert.That(dltLine.TimeStamp, Is.EqualTo(time));
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.True);
        }

        [Test]
        public void ConstructVerboseLineNoSessionId()
        {
            DateTime time = DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634);

            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(time);
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetDeviceTimeStamp(55352000);     // 5.5352s
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetIsVerbose(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(10);

            Assert.That(builder.EcuId, Is.EqualTo("ECU1"));
            Assert.That(builder.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(builder.ContextId, Is.EqualTo("CTX1"));
            Assert.That(builder.Count, Is.EqualTo(127));
            Assert.That(builder.DltType, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(builder.DeviceTimeStamp, Is.EqualTo(new TimeSpan(55352000)));
            Assert.That(builder.IsVerbose, Is.True);
            Assert.That(builder.SessionId, Is.EqualTo(0));
            Assert.That(builder.TimeStamp, Is.EqualTo(time));
            Assert.That(builder.BigEndian, Is.False);
            Assert.That(builder.Position, Is.EqualTo(10));

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltTraceLine>());

            DltTraceLine dltLine = (DltTraceLine)line;
            Assert.That(dltLine.Line, Is.EqualTo(0));
            Assert.That(dltLine.Position, Is.EqualTo(10));
            Assert.That(dltLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(time)} 5.5352 127 ECU1 APP1 CTX1 0 log info verbose 0 "));
            Assert.That(dltLine.Text, Is.EqualTo(string.Empty));
            Assert.That(dltLine.Features.EcuId, Is.True);
            Assert.That(dltLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(dltLine.Features.ApplicationId, Is.True);
            Assert.That(dltLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(dltLine.Features.ContextId, Is.True);
            Assert.That(dltLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(dltLine.Count, Is.EqualTo(127));
            Assert.That(dltLine.Features.MessageType, Is.True);
            Assert.That(dltLine.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(dltLine.Features.SessionId, Is.False);
            Assert.That(dltLine.SessionId, Is.EqualTo(0));
            Assert.That(dltLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(dltLine.DeviceTimeStamp, Is.EqualTo(new TimeSpan(55352000)));
            Assert.That(dltLine.Features.TimeStamp, Is.True);
            Assert.That(dltLine.TimeStamp, Is.EqualTo(time));
            Assert.That(dltLine.Features.BigEndian, Is.False);
            Assert.That(dltLine.Features.IsVerbose, Is.True);
        }

        [Test]
        public void GenerateMultipleLines()
        {
            DateTime time = DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634);
            DateTime time2 = DltTime.FileTime(2021, 12, 4, 17, 56, 24.1221);

            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(time);
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetSessionId(1435);
            builder.SetDeviceTimeStamp(55352000);     // 5.5352s
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetIsVerbose(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(10);

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            builder.Reset();
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(10));
            Assert.That(line.ToString(), Is.EqualTo($"{DltTime.LocalTime(time)} 5.5352 127 ECU1 APP1 CTX1 1435 log info verbose 0 "));
            Assert.That(line.Text, Is.EqualTo(string.Empty));
            Assert.That(line.Features.EcuId, Is.True);
            Assert.That(line.EcuId, Is.EqualTo("ECU1"));
            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(line.Features.ContextId, Is.True);
            Assert.That(line.ContextId, Is.EqualTo("CTX1"));
            Assert.That(line.Count, Is.EqualTo(127));
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(line.Features.SessionId, Is.True);
            Assert.That(line.SessionId, Is.EqualTo(1435));
            Assert.That(line.Features.DeviceTimeStamp, Is.True);
            Assert.That(line.DeviceTimeStamp, Is.EqualTo(new TimeSpan(55352000)));
            Assert.That(line.Features.TimeStamp, Is.True);
            Assert.That(line.TimeStamp, Is.EqualTo(time));
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.True);

            builder.SetEcuId("FCU1");
            builder.SetTimeStamp(time2);
            builder.SetCount(128);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetDeviceTimeStamp(55356000);     // 5.5356s
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetIsVerbose(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(60);

            line = (DltTraceLine)builder.GetResult();
            builder.Reset();
            Assert.That(line.Line, Is.EqualTo(1));                     // This number must be incremented by 1
            Assert.That(line.Position, Is.EqualTo(60));
            Assert.That(line.ToString(), Is.EqualTo($"{DltTime.LocalTime(time2)} 5.5356 128 ECU1 APP1 CTX1 0 log info verbose 0 "));
            Assert.That(line.Text, Is.EqualTo(string.Empty));
            Assert.That(line.Features.EcuId, Is.True);
            Assert.That(line.EcuId, Is.EqualTo("ECU1"));
            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(line.Features.ContextId, Is.True);
            Assert.That(line.ContextId, Is.EqualTo("CTX1"));
            Assert.That(line.Count, Is.EqualTo(128));
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Type, Is.EqualTo(DltType.LOG_INFO));
            Assert.That(line.Features.SessionId, Is.False);            // After builder.Reset(), this is cleared.
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.Features.DeviceTimeStamp, Is.True);
            Assert.That(line.DeviceTimeStamp, Is.EqualTo(new TimeSpan(55356000)));
            Assert.That(line.Features.TimeStamp, Is.True);
            Assert.That(line.TimeStamp, Is.EqualTo(time2));
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.True);
        }

        [Test]
        public void GenerateEcuId()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetStorageHeaderEcuId("FCU1");
            Assert.That(builder.Features.EcuId, Is.False);
            Assert.That(builder.EcuId, Is.EqualTo("FCU1"));

            builder.SetEcuId("ECU1");
            Assert.That(builder.Features.EcuId, Is.True);
            Assert.That(builder.EcuId, Is.EqualTo("ECU1"));

            // Shouldn't change, because the ECU ID is already set.
            builder.SetStorageHeaderEcuId("XXXX");
            Assert.That(builder.Features.EcuId, Is.True);
            Assert.That(builder.EcuId, Is.EqualTo("ECU1"));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.EcuId, Is.EqualTo("ECU1"));
            Assert.That(line.Features.EcuId, Is.True);

            builder.Reset();
            Assert.That(builder.Features.EcuId, Is.False);
            Assert.That(builder.EcuId, Is.Null);
        }

        [Test]
        public void GenerateEcuIdStorageOnly()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetStorageHeaderEcuId("FCU1");
            Assert.That(builder.Features.EcuId, Is.False);
            Assert.That(builder.EcuId, Is.EqualTo("FCU1"));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.EcuId, Is.EqualTo("FCU1"));
            Assert.That(line.Features.EcuId, Is.False);

            builder.Reset();
            Assert.That(builder.Features.EcuId, Is.False);
            Assert.That(builder.EcuId, Is.Null);
        }

        [Test]
        public void GenerateDeviceTimeStamp()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetDeviceTimeStamp(DltTime.DeviceTime(5.345).Ticks);
            Assert.That(builder.Features.DeviceTimeStamp, Is.True);
            Assert.That(builder.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.345)));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.EcuId, Is.EqualTo(string.Empty));
            Assert.That(line.Features.EcuId, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.True);
            Assert.That(line.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.345)));

            // The Device Time Stamp is *not* reset, only the feature flag.
            builder.Reset();
            Assert.That(builder.Features.DeviceTimeStamp, Is.False);
            Assert.That(builder.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.345)));
        }

        [Test]
        public void GenerateLogTimeStamp()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetTimeStamp(DltTime.FileTime(2021, 12, 4, 21, 33, 14.3449));
            Assert.That(builder.Features.TimeStamp, Is.True);
            Assert.That(builder.TimeStamp, Is.EqualTo(DltTime.FileTime(2021, 12, 4, 21, 33, 14.3449)));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.TimeStamp, Is.True);
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.FileTime(2021, 12, 4, 21, 33, 14.3449)));

            // The Time Stamp is *not* reset, only the feature flag.
            builder.Reset();
            Assert.That(builder.Features.TimeStamp, Is.False);
            Assert.That(builder.TimeStamp, Is.EqualTo(DltTime.FileTime(2021, 12, 4, 21, 33, 14.3449)));
        }

        [Test]
        public void GenerateSessionId()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetSessionId(1245);
            Assert.That(builder.Features.SessionId, Is.True);
            Assert.That(builder.SessionId, Is.EqualTo(1245));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.SessionId, Is.True);
            Assert.That(line.SessionId, Is.EqualTo(1245));

            builder.Reset();
            Assert.That(builder.Features.SessionId, Is.False);
            Assert.That(builder.SessionId, Is.EqualTo(0));
        }

        [Test]
        public void GenerateApplicationId()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetApplicationId("APP1");
            Assert.That(builder.Features.ApplicationId, Is.True);
            Assert.That(builder.ApplicationId, Is.EqualTo("APP1"));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.ApplicationId, Is.EqualTo("APP1"));

            builder.Reset();
            Assert.That(builder.Features.ApplicationId, Is.False);
            Assert.That(builder.ApplicationId, Is.Null);
        }

        [Test]
        public void GenerateContextId()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetContextId("CTX1");
            Assert.That(builder.Features.ContextId, Is.True);
            Assert.That(builder.ContextId, Is.EqualTo("CTX1"));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.ContextId, Is.True);
            Assert.That(line.ContextId, Is.EqualTo("CTX1"));

            builder.Reset();
            Assert.That(builder.Features.ContextId, Is.False);
            Assert.That(builder.ContextId, Is.Null);
        }

        [Test]
        public void GenerateMessageType()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_DEBUG);
            Assert.That(builder.Features.MessageType, Is.True);
            Assert.That(builder.DltType, Is.EqualTo(DltType.LOG_DEBUG));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Type, Is.EqualTo(DltType.LOG_DEBUG));

            builder.Reset();
            Assert.That(builder.Features.MessageType, Is.False);
            Assert.That(builder.DltType, Is.EqualTo(DltType.UNKNOWN));
        }

        [Test]
        public void GenerateCount()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetCount(127);
            Assert.That(builder.Count, Is.EqualTo(127));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Count, Is.EqualTo(127));

            builder.Reset();
            Assert.That(builder.Count, Is.EqualTo(DltTraceLineBase.InvalidCounter));
        }

        [Test]
        public void GenerateIsVerbose()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.Features += DltLineFeatures.VerboseFeature;
            Assert.That(builder.Features.IsVerbose, Is.True);

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.IsVerbose, Is.True);

            builder.Reset();
            Assert.That(builder.Features.IsVerbose, Is.False);
        }

        [Test]
        public void GenerateIsBigEndian()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.Features += DltLineFeatures.BigEndianFeature;
            Assert.That(builder.Features.BigEndian, Is.True);

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Features.BigEndian, Is.True);

            // The BigEndian is *not* reset.
            builder.Reset();
            Assert.That(builder.Features.BigEndian, Is.True);
        }

        [Test]
        public void GeneratePosition()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(10);
            Assert.That(builder.Position, Is.EqualTo(10));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Position, Is.EqualTo(10));

            // Reset doesn't reset the position, it resets so that a new line can be created. This is an optimization
            // used in the decoder where it needs to reset and keep the position.
            builder.Reset();
            Assert.That(builder.Position, Is.EqualTo(10));
        }

        [Test]
        public void GenerateNumberOfArguments()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetNumberOfArgs(2);
            Assert.That(builder.NumberOfArgs, Is.EqualTo(2));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Arguments.Count, Is.EqualTo(0));       // The number of arguments isn't used.

            builder.Reset();
            Assert.That(builder.NumberOfArgs, Is.EqualTo(0));
        }

        [Test]
        public void AddArgument()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetNumberOfArgs(1);
            builder.AddArgument(new CustomArg());
            Assert.That(builder.Arguments.Count, Is.EqualTo(1));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Arguments.Count, Is.EqualTo(1));
            Assert.That(line.Arguments[0], Is.TypeOf<CustomArg>());
            Assert.That(line.Arguments[0].ToString(), Is.EqualTo("custom"));

            // Ensure the argument list is copied, not referenced. Note, the arguments themselves are copied as a
            // reference.
            builder.AddArgument(new CustomArg("foo"));
            Assert.That(line.Arguments.Count, Is.EqualTo(1));

            builder.Reset();
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddArgumentTwice()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetNumberOfArgs(2);
            builder.AddArgument(new CustomArg());
            builder.AddArgument(new CustomArg("foo"));
            Assert.That(builder.Arguments.Count, Is.EqualTo(2));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Arguments.Count, Is.EqualTo(2));
            Assert.That(line.Arguments[0], Is.TypeOf<CustomArg>());
            Assert.That(line.Arguments[0].ToString(), Is.EqualTo("custom"));
            Assert.That(line.Arguments[1], Is.TypeOf<CustomArg>());
            Assert.That(line.Arguments[1].ToString(), Is.EqualTo("foo"));

            // Ensure the argument list is copied, not referenced. Note, the arguments themselves are copied as a
            // reference.
            builder.AddArgument(new CustomArg("bar"));
            Assert.That(line.Arguments.Count, Is.EqualTo(2));

            builder.Reset();
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddArguments()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetNumberOfArgs(2);
            builder.AddArguments(new IDltArg[] {
                new CustomArg(),
                new CustomArg("foo")
            });
            Assert.That(builder.Arguments.Count, Is.EqualTo(2));

            DltTraceLine line = (DltTraceLine)builder.GetResult();
            Assert.That(line.Arguments.Count, Is.EqualTo(2));
            Assert.That(line.Arguments[0], Is.TypeOf<CustomArg>());
            Assert.That(line.Arguments[0].ToString(), Is.EqualTo("custom"));
            Assert.That(line.Arguments[1], Is.TypeOf<CustomArg>());
            Assert.That(line.Arguments[1].ToString(), Is.EqualTo("foo"));

            // Ensure the argument list is copied, not referenced. Note, the arguments themselves are copied as a
            // reference.
            builder.AddArgument(new CustomArg("bar"));
            Assert.That(line.Arguments.Count, Is.EqualTo(2));

            builder.Reset();
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddArgumentsNull()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetNumberOfArgs(0);
            builder.AddArguments(null);
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddArgumentsEmpty()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetNumberOfArgs(0);
            builder.AddArguments(Array.Empty<IDltArg>());
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddArgumentNone()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetNumberOfArgs(0);
            builder.AddArgument(null);
            Assert.That(builder.Arguments.Count, Is.EqualTo(0));
        }

        [Test]
        public void GenerateOnlineBuilder()
        {
            IDltLineBuilder builder = new DltLineBuilder(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetStorageHeaderEcuId("FCU1");

            DltTraceLine line1 = (DltTraceLine)builder.GetResult();
            Assert.That(line1.Line, Is.EqualTo(0));
            Assert.That(line1.TimeStamp, Is.GreaterThan(DateTime.Now.AddSeconds(-30)));
            Assert.That(line1.Features.TimeStamp, Is.True);
            builder.Reset();

            Thread.Sleep(100);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetStorageHeaderEcuId("FCU1");
            DltTraceLine line2 = (DltTraceLine)builder.GetResult();
            Assert.That(line2.Line, Is.EqualTo(1));
            Assert.That(line2.TimeStamp, Is.GreaterThanOrEqualTo(line1.TimeStamp));
            Assert.That(line2.Features.TimeStamp, Is.True);
        }

        [Test]
        public void GenerateSkippedLine()
        {
            IDltLineBuilder builder = new DltLineBuilder();

            builder.AddSkippedBytes(25, "header");
            DltSkippedTraceLine line = (DltSkippedTraceLine)builder.GetSkippedResult();
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(line.Text, Is.EqualTo("Skipped: 25 bytes; Reason: header"));
            Assert.That(line.BytesSkipped, Is.EqualTo(25));
            Assert.That(line.Reason, Is.EqualTo("header"));
            Assert.That(line.Features.IsVerbose, Is.True);
            Assert.That(line.Features.TimeStamp, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
        }

        [Test]
        public void GenerateSkippedLineWithTimeStamp()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetTimeStamp(DltTime.FileTime(2021, 12, 5, 10, 39, 23.0456));

            builder.AddSkippedBytes(25, "header");
            DltSkippedTraceLine line = (DltSkippedTraceLine)builder.GetSkippedResult();
            Assert.That(line.Line, Is.EqualTo(0));
            Assert.That(line.Position, Is.EqualTo(0));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.FileTime(2021, 12, 5, 10, 39, 23.0456)));
            Assert.That(line.Text, Is.EqualTo("Skipped: 25 bytes; Reason: header"));
            Assert.That(line.BytesSkipped, Is.EqualTo(25));
            Assert.That(line.Reason, Is.EqualTo("header"));
            Assert.That(line.Features.IsVerbose, Is.True);
            Assert.That(line.Features.TimeStamp, Is.True);        // Use last set time stamp, as otherwise not available
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
        }

        [Test]
        public void SkippedLineAfterNormaLine()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634));
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetSessionId(1435);
            builder.SetDeviceTimeStamp(55352000);     // 5.5352s
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetIsVerbose(true);
            builder.SetDltType(DltType.LOG_INFO);
            builder.SetPosition(10);

            _ = builder.GetResult();
            builder.Reset();

            builder.AddSkippedBytes(25, "header");
            DltSkippedTraceLine line = (DltSkippedTraceLine)builder.GetSkippedResult();
            Assert.That(line.Line, Is.EqualTo(1));
            Assert.That(line.Position, Is.EqualTo(10));
            Assert.That(line.TimeStamp, Is.EqualTo(DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634)));
            Assert.That(line.Count, Is.EqualTo(-1));
            Assert.That(line.EcuId, Is.EqualTo(string.Empty));
            Assert.That(line.SessionId, Is.EqualTo(0));
            Assert.That(line.DeviceTimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(line.Type, Is.EqualTo(DltType.LOG_WARN));
            Assert.That(line.ApplicationId, Is.EqualTo("SKIP"));
            Assert.That(line.ContextId, Is.EqualTo("SKIP"));
            Assert.That(line.Text, Is.EqualTo("Skipped: 25 bytes; Reason: header"));
            Assert.That(line.BytesSkipped, Is.EqualTo(25));
            Assert.That(line.Reason, Is.EqualTo("header"));
            Assert.That(line.Features.TimeStamp, Is.True);    // Use last set time stamp, as otherwise not available
            Assert.That(line.Features.EcuId, Is.True);
            Assert.That(line.Features.SessionId, Is.False);
            Assert.That(line.Features.DeviceTimeStamp, Is.False);
            Assert.That(line.Features.BigEndian, Is.False);
            Assert.That(line.Features.IsVerbose, Is.True);
            Assert.That(line.Features.MessageType, Is.True);
            Assert.That(line.Features.ApplicationId, Is.True);
            Assert.That(line.Features.ContextId, Is.True);
        }

        [Test]
        public void DefaultControlLineWhenServiceNull()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.CONTROL_REQUEST);

            Assert.That(() => {
                _ = builder.GetResult();
            }, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DefaultControlLine()
        {
            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetDltType(DltType.CONTROL_REQUEST);
            builder.SetControlPayload(new CustomControlRequest());

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltControlTraceLine>());

            DltControlTraceLine controlLine = (DltControlTraceLine)line;
            Assert.That(controlLine.Line, Is.EqualTo(0));
            Assert.That(controlLine.Position, Is.EqualTo(0));
            Assert.That(controlLine.ToString(), Is.EqualTo("1970/01/01 00:00:00.000000 0.0000 -1    0 control request non-verbose [custom_control_request]"));
            Assert.That(controlLine.Text, Is.EqualTo("[custom_control_request]"));
            Assert.That(controlLine.Features.EcuId, Is.False);
            Assert.That(controlLine.EcuId, Is.EqualTo(string.Empty));
            Assert.That(controlLine.Features.ApplicationId, Is.True);
            Assert.That(controlLine.ApplicationId, Is.EqualTo(string.Empty));
            Assert.That(controlLine.Features.ContextId, Is.True);
            Assert.That(controlLine.ContextId, Is.EqualTo(string.Empty));
            Assert.That(controlLine.Count, Is.EqualTo(DltTraceLineBase.InvalidCounter));
            Assert.That(controlLine.Features.MessageType, Is.True);
            Assert.That(controlLine.Type, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(controlLine.Features.SessionId, Is.False);
            Assert.That(controlLine.SessionId, Is.EqualTo(0));
            Assert.That(controlLine.Features.DeviceTimeStamp, Is.False);
            Assert.That(controlLine.DeviceTimeStamp, Is.EqualTo(new TimeSpan(0)));
            Assert.That(controlLine.Features.TimeStamp, Is.False);
            Assert.That(controlLine.TimeStamp, Is.EqualTo(DltTime.Default));
            Assert.That(controlLine.Features.BigEndian, Is.False);
            Assert.That(controlLine.Features.IsVerbose, Is.False);
            Assert.That(controlLine.Service.ServiceId, Is.EqualTo(0x1000));
            Assert.That(controlLine.Service.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
        }

        [Test]
        public void ConstructFullControlRequest()
        {
            DateTime time = DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634);

            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(time);
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetSessionId(1435);
            builder.SetDeviceTimeStamp(DltTime.DeviceTime(5.5352).Ticks);
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetDltType(DltType.CONTROL_REQUEST);
            builder.SetIsVerbose(false);
            builder.SetPosition(10);
            builder.SetControlPayload(new CustomControlRequest());

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltControlTraceLine>());

            DltControlTraceLine controlLine = (DltControlTraceLine)line;
            Assert.That(controlLine.Line, Is.EqualTo(0));
            Assert.That(controlLine.Position, Is.EqualTo(10));
            Assert.That(controlLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(time)} 5.5352 127 ECU1 APP1 CTX1 1435 control request non-verbose [custom_control_request]"));
            Assert.That(controlLine.Text, Is.EqualTo("[custom_control_request]"));
            Assert.That(controlLine.Features.EcuId, Is.True);
            Assert.That(controlLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(controlLine.Features.ApplicationId, Is.True);
            Assert.That(controlLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(controlLine.Features.ContextId, Is.True);
            Assert.That(controlLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(controlLine.Count, Is.EqualTo(127));
            Assert.That(controlLine.Features.MessageType, Is.True);
            Assert.That(controlLine.Type, Is.EqualTo(DltType.CONTROL_REQUEST));
            Assert.That(controlLine.Features.SessionId, Is.True);
            Assert.That(controlLine.SessionId, Is.EqualTo(1435));
            Assert.That(controlLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(controlLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.5352)));
            Assert.That(controlLine.Features.TimeStamp, Is.True);
            Assert.That(controlLine.TimeStamp, Is.EqualTo(time));
            Assert.That(controlLine.Features.BigEndian, Is.False);
            Assert.That(controlLine.Features.IsVerbose, Is.False);
            Assert.That(controlLine.Service.ServiceId, Is.EqualTo(0x1000));
            Assert.That(controlLine.Service.DefaultType, Is.EqualTo(DltType.CONTROL_REQUEST));
        }

        [Test]
        public void BuildControlDltTimeMarker()
        {
            DateTime time = DltTime.FileTime(2021, 12, 4, 17, 56, 23.5634);

            IDltLineBuilder builder = new DltLineBuilder();
            builder.SetStorageHeaderEcuId("FCU1");
            builder.SetTimeStamp(time);
            builder.SetCount(127);
            builder.SetBigEndian(false);
            builder.SetEcuId("ECU1");
            builder.SetSessionId(1435);
            builder.SetDeviceTimeStamp(DltTime.DeviceTime(5.5352).Ticks);
            builder.SetApplicationId("APP1");
            builder.SetContextId("CTX1");
            builder.SetDltType(DltType.CONTROL_TIME);
            builder.SetIsVerbose(false);
            builder.SetPosition(10);
            builder.SetControlPayload(new DltTimeMarker());

            DltTraceLineBase line = builder.GetResult();
            Assert.That(line, Is.TypeOf<DltControlTraceLine>());

            DltControlTraceLine controlLine = (DltControlTraceLine)line;
            Assert.That(controlLine.Line, Is.EqualTo(0));
            Assert.That(controlLine.Position, Is.EqualTo(10));
            Assert.That(controlLine.ToString(), Is.EqualTo($"{DltTime.LocalTime(time)} 5.5352 127 ECU1 APP1 CTX1 1435 control time non-verbose []"));
            Assert.That(controlLine.Text, Is.EqualTo("[]"));
            Assert.That(controlLine.Features.EcuId, Is.True);
            Assert.That(controlLine.EcuId, Is.EqualTo("ECU1"));
            Assert.That(controlLine.Features.ApplicationId, Is.True);
            Assert.That(controlLine.ApplicationId, Is.EqualTo("APP1"));
            Assert.That(controlLine.Features.ContextId, Is.True);
            Assert.That(controlLine.ContextId, Is.EqualTo("CTX1"));
            Assert.That(controlLine.Count, Is.EqualTo(127));
            Assert.That(controlLine.Features.MessageType, Is.True);
            Assert.That(controlLine.Type, Is.EqualTo(DltType.CONTROL_TIME));
            Assert.That(controlLine.Features.SessionId, Is.True);
            Assert.That(controlLine.SessionId, Is.EqualTo(1435));
            Assert.That(controlLine.Features.DeviceTimeStamp, Is.True);
            Assert.That(controlLine.DeviceTimeStamp, Is.EqualTo(DltTime.DeviceTime(5.5352)));
            Assert.That(controlLine.Features.TimeStamp, Is.True);
            Assert.That(controlLine.TimeStamp, Is.EqualTo(time));
            Assert.That(controlLine.Features.BigEndian, Is.False);
            Assert.That(controlLine.Features.IsVerbose, Is.False);
            Assert.That(controlLine.Service.ServiceId, Is.EqualTo(-1));
            Assert.That(controlLine.Service.DefaultType, Is.EqualTo(DltType.CONTROL_TIME));
        }
    }
}
