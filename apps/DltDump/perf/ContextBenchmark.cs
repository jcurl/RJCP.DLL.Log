namespace RJCP.App.DltDump
{
    using System;
    using BenchmarkDotNet.Attributes;
    using Domain.OutputStream;
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

        private readonly ContextOutput m_Context;

        public ContextBenchmark()
        {
            Constraint filter = new Constraint().DltAppId("APP2");
            m_Context = new ContextOutput(filter, 4, 2, new NullOutput());
        }

        private void Filter()
        {
            foreach (DltTraceLineBase line in Lines) {
                m_Context.Write(line);
            }
        }

        [Benchmark]
        public void RunContext() => Filter();
    }
}
