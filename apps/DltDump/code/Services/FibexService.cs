namespace RJCP.App.DltDump.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Resources;
    using RJCP.Core.Collections;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;
    using View;

    public static class FibexService
    {
        public static FibexOptions GetOptions(CmdOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            FibexOptions fibexOptions = FibexOptions.None;
            if (options.NonVerboseMultiEcu) fibexOptions |= FibexOptions.WithEcuId;
            if (options.NonVerboseNoExtHeader) fibexOptions |= FibexOptions.WithoutExtHeader;
            return fibexOptions;
        }

        public static FibexMap LoadFrameMap(IEnumerable<string> paths, FibexOptions options)
        {
            FibexMap map = new(options);
            try {
                foreach (string fibexPath in paths) {
                    map.LoadFibex(fibexPath);
                }
                if (map.EventLog.Count > 0) {
                    DumpFibexEventLog(map);
                    return null;
                }
            } catch (Exception ex) {
                Global.Instance.Terminal.StdOut.WrapLine(AppResources.FibexApp_ErrorLoadingFibex, ex.Message);
                DumpFibexEventLog(map);
                return null;
            }
            return map;
        }

        private static void DumpFibexEventLog(FibexMap map)
        {
            foreach (IEvent<FibexLogEntry> fibexEvent in map.EventLog) {
                string fileName = Path.GetFileName(fibexEvent.Identifier.FileName);
                Global.Instance.Terminal.StdOut.WrapLine("{0}: [{1}] {2}", fibexEvent.TimeStamp, fileName, fibexEvent.Identifier.Message);
            }
        }
    }
}
