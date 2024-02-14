namespace RJCP.Diagnostics.Log
{
    using RJCP.Diagnostics.Trace;

    internal static class Log
    {
        public static readonly LogSource Dlt = new("RJCP.Diagnostics.Log.Dlt");
        public static readonly LogSource Encoder = new("RJCP.Diagnostics.Log.Encoder");
        public static readonly LogSource DltNonVerbose = new("RJCP.Diagnostics.Log.Dlt.NonVerbose");
    }
}
