namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Resources;
    using RJCP.Core.Collections;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;
    using Services;

    public class NonVerboseApp
    {
        private readonly FibexMap m_Map;

        public NonVerboseApp(FibexOptions options)
        {
            m_Map = new FibexMap(options);
        }

        public ExitCode Run(IEnumerable<string> fibexPaths)
        {
            try {
                foreach (string fibexPath in fibexPaths) {
                    m_Map.LoadFibex(fibexPath);
                }
                if (m_Map.EventLog.Count > 0) {
                    DumpFibexEventLog(m_Map);
                    return ExitCode.FibexError;
                }
            } catch (Exception ex) {
                Terminal.WriteLine(AppResources.FibexApp_ErrorLoadingFibex, ex.Message);
                DumpFibexEventLog(m_Map);
                return ExitCode.FibexError;
            }

            Terminal.WriteLine(AppResources.FibexApp_FibexLoaded);
            return ExitCode.Success;
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
