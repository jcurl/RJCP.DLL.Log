namespace RJCP.App.DltDump.Application
{
    using System.Collections.Generic;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;
    using Services;

    public class NonVerboseApp
    {
        private readonly FibexOptions m_Options;

        public NonVerboseApp(FibexOptions options)
        {
            m_Options = options;
        }

        public ExitCode Run(IEnumerable<string> fibexPaths)
        {
            IFrameMap map = FibexService.LoadFrameMap(fibexPaths, m_Options);
            if (map == null) return ExitCode.FibexError;
            return ExitCode.Success;
        }
    }
}
