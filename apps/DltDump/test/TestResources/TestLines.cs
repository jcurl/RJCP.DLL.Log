namespace RJCP.App.DltDump.TestResources
{
    using System;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Args;
    using RJCP.Diagnostics.Log.Dlt.ControlArgs;

    internal static class TestLines
    {
        public static readonly DltTraceLine Verbose = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow,
            SessionId = 127,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltTraceLine NoEcuId = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow,
            SessionId = 127,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.SessionIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltTraceLine NoEcuIdStorageHeader = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow,
            SessionId = 127,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.SessionIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltTraceLine NoExtHdr = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow,
            SessionId = 127,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            // Has no Extended Header
            Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature
        };

        public static readonly DltTraceLine NoSessionId = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltControlTraceLine Control = new DltControlTraceLine(
            new GetSoftwareVersionResponse(GetSoftwareVersionResponse.StatusOk, "22w10.6")) {
            EcuId = "ECU1",
            ApplicationId = "DA1",
            ContextId = "DC1",
            Position = 4,
            Line = 0,
            Count = 127,
            Type = DltType.CONTROL_RESPONSE,
            TimeStamp = DateTime.UtcNow,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltControlTraceLine ControlVerbose = new DltControlTraceLine(
            new GetSoftwareVersionResponse(GetSoftwareVersionResponse.StatusOk, "22w10.6")) {
            EcuId = "ECU1",
            ApplicationId = "DA1",
            ContextId = "DC1",
            Position = 4,
            Line = 0,
            Count = 127,
            Type = DltType.CONTROL_RESPONSE,
            TimeStamp = DateTime.UtcNow,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };
    }
}
