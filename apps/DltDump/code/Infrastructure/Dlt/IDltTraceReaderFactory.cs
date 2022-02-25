namespace RJCP.App.DltDump.Infrastructure.Dlt
{
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Interface for the DLT Trace Reader Factory.
    /// </summary>
    public interface IDltTraceReaderFactory : ITraceReaderFactory<DltTraceLineBase> { }
}
