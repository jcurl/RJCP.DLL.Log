﻿namespace RJCP.App.DltDump.TestResources
{
    using System;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Args;
    using RJCP.Diagnostics.Log.Dlt.ControlArgs;

    internal static class TestLines
    {
        public static readonly DltTraceLine Verbose = new(new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow - TimeSpan.FromSeconds(2),
            SessionId = 127,
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.SessionIdFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.MessageTypeFeature + DltLineFeatures.VerboseFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltTraceLine Verbose2 = new(new[] { new StringDltArg("Message 2") }) {
            EcuId = "ECU1",
            ApplicationId = "APP2",
            ContextId = "CTX2",
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

        public static readonly DltTraceLine NoEcuId = new(new[] { new StringDltArg("Message 1") }) {
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

        public static readonly DltTraceLine NoEcuIdStorageHeader = new(new[] { new StringDltArg("Message 1") }) {
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

        public static readonly DltTraceLine NoExtHdr = new(new[] { new StringDltArg("Message 1") }) {
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

        public static readonly DltTraceLine NoSessionId = new(new[] { new StringDltArg("Message 1") }) {
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

        public static readonly DltControlTraceLine Control = new(
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

        public static readonly DltControlTraceLine ControlVerbose = new(
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

        public static readonly DltNonVerboseTraceLine NonVerboseWithArgs = new(42, new[] { new StringDltArg("Message 1") }) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow - TimeSpan.FromSeconds(2),
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.MessageTypeFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };

        public static readonly DltNonVerboseTraceLine NonVerbose = new(42) {
            EcuId = "ECU1",
            ApplicationId = "APP1",
            ContextId = "CTX1",
            Position = 3,
            Line = 0,
            Count = 127,
            Type = DltType.LOG_INFO,
            TimeStamp = DateTime.UtcNow - TimeSpan.FromSeconds(2),
            DeviceTimeStamp = new TimeSpan(0, 0, 1, 20, 544),
            Features = DltLineFeatures.EcuIdFeature + DltLineFeatures.MessageTypeFeature +
                       DltLineFeatures.DevTimeStampFeature + DltLineFeatures.LogTimeStampFeature +
                       DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature
        };
    }
}
