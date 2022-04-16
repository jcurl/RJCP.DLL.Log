namespace RJCP.App.DltDump.Domain.Dlt
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

        /// <summary>
        /// Gets or sets a value indicating if in online mode.
        /// </summary>
        /// <value>
        /// If <see langword="true"/> this is in online mode, where time stamps are obtained by the local host, else
        /// <see langword="false"/>. Formats with a storage header ignore this field.
        /// </value>
        bool OnlineMode { get; set; }

        /// <summary>
        /// Gets or sets the output stream to use when instantiating.
        /// </summary>
        /// <value>The output stream.</value>
        /// <remarks>
        /// When instantiating via <see cref="CreateAsync(string)"/>, the <see cref="IOutputStream.SupportsBinary"/> is
        /// used to determine if this object should be injected or not. If this object is <see langword="null"/>, then
        /// no <see cref="IOutputStream"/> is used.
        /// </remarks>
        IOutputStream OutputStream { get; set; }
    }
}
