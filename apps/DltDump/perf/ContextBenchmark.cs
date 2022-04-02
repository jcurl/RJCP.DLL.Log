namespace RJCP.App.DltDump
{
    using System;
    using BenchmarkDotNet.Attributes;
    using Domain;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Args;

    public class ContextBenchmark
    {
        private static readonly DltTraceLine Verbose = new DltTraceLine(new[] { new StringDltArg("Message 1") }) {
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

        private static readonly DltTraceLine Verbose2 = new DltTraceLine(new[] { new StringDltArg("Message 2") }) {
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

        private static readonly DltTraceLineBase[] Lines = new[] {
            Verbose,
            Verbose,
            Verbose,
            Verbose,
            Verbose,
            Verbose2,
            Verbose,
            Verbose,
            Verbose,
            Verbose,
            Verbose
        };

        private readonly Context m_Context;

        public ContextBenchmark()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            m_Context = new Context(filter, 4, 2);
        }

        private void Filter()
        {
            int matches = 0;
            foreach (DltTraceLineBase line in Lines) {
                if (m_Context.Check(line)) {
                    foreach (ContextPacket beforeLine in m_Context.GetBeforeContext()) {
                        matches++;
                    }
                    matches++;
                } else if (m_Context.IsAfterContext()) {
                    matches++;
                }
            }

            if (matches != 7)
                throw new InvalidOperationException($"Matches is {matches}, expected 7");
        }

        [Benchmark]
        public void RunContext() => Filter();
    }
}
