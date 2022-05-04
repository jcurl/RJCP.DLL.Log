namespace RJCP.App.DltCount
{
    using System;
    using System.Threading.Tasks;
    using System.Globalization;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt.Args;

    public static class Program
    {
        public async static Task<int> Main(string[] args)
        {
            ITraceReaderFactory<DltTraceLineBase> factory = new DltFileTraceReaderFactory();

            foreach (string file in args) {
                using (ITraceReader<DltTraceLineBase> reader = await factory.CreateAsync(file)) {
                    DltTraceLineBase line;
                    do {
                        line = await reader.GetLineAsync();
                        if (line != null) {
                            DateTime time = line.Features.TimeStamp ? line.TimeStamp : new DateTime();
                            string ecuid = line.Features.EcuId ? line.EcuId : "----";
                            string appid = line.Features.ApplicationId ? line.ApplicationId : "----";
                            string ctxid = line.Features.ContextId ? line.ContextId : "----";

                            Constraint verbose = new Constraint().DltIsControl().Or.DltIsVerbose(true).End();

                            if (verbose.Check(line)) {
                                Console.WriteLine("{0} {1,4} {2,4} {3,4} : {4}",
                                    time.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture),
                                    ecuid, appid, ctxid, line.Length);
                            } else {
                                DltTraceLine traceLine = (DltTraceLine)line;
                                Console.WriteLine("{0} {1,4} {2,4} {3,4} : {4} (message id = {5})",
                                    time.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture),
                                    ecuid, appid, ctxid, traceLine.Length,
                                    ((NonVerboseDltArg)traceLine.Arguments[0]).MessageId);
                            }
                        }
                    } while (line != null);
                }
            }
            return 0;
        }
    }
}
