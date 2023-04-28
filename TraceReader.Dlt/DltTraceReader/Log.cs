namespace RJCP.Diagnostics.Log
{
    using RJCP.Diagnostics.Trace;

    internal static class Log
    {
        public static readonly LogSource Dlt = new LogSource("RJCP.Diagnostics.Log.Dlt");
        public static readonly LogSource DltNonVerbose = new LogSource("RJCP.Diagnostics.Log.Dlt.NonVerbose");
    }
}
