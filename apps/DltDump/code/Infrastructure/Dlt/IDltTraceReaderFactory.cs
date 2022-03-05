namespace RJCP.App.DltDump.Infrastructure.Dlt
{
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Interface for the DLT Trace Reader Factory.
    /// </summary>
    public interface IDltTraceReaderFactory : ITraceReaderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Gets or sets the input format which is used to decide which decoder to create.
        /// </summary>
        /// <value>The input format that defines the decoder that should be created.</value>
        InputFormat InputFormat { get; set; }
    }
}
