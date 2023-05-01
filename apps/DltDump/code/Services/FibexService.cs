namespace RJCP.App.DltDump.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Resources;
    using RJCP.App.DltDump.View;
    using RJCP.Core.Collections;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;

    public static class FibexService
    {
        public static FibexOptions GetOptions(CmdOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            FibexOptions fibexOptions = FibexOptions.None;
            if (options.NonVerboseMultiEcu) fibexOptions |= FibexOptions.WithEcuId;
            if (options.NonVerboseNoExtHeader) fibexOptions |= FibexOptions.WithoutExtHeader;
            return fibexOptions;
        }

        public static FibexMap LoadFrameMap(IEnumerable<string> paths, FibexOptions options)
        {
            FibexMap map = new FibexMap(options);
            try {
                foreach (string fibexPath in paths) {
                    map.LoadFibex(fibexPath);
                }
                if (map.EventLog.Count > 0) {
                    DumpFibexEventLog(map);
                    return null;
                }
            } catch (Exception ex) {
                Terminal.WriteLine(AppResources.FibexApp_ErrorLoadingFibex, ex.Message);
                DumpFibexEventLog(map);
                return null;
            }
            return map;
        }

        private static void DumpFibexEventLog(FibexMap map)
        {
            foreach (IEvent<FibexLogEntry> fibexEvent in map.EventLog) {
                string fileName = Path.GetFileName(fibexEvent.Identifier.FileName);
                Terminal.WriteLine("{0}: [{1}] {2}", fibexEvent.TimeStamp, fileName, fibexEvent.Identifier.Message);
            }
        }
    }
}
