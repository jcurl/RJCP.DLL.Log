namespace RJCP.Diagnostics.Log
{
    using Encoder;

    /// <summary>
    /// A DLT Writer factory for writing DLT streams with a storage header.
    /// </summary>
    public sealed class DltFileTraceWriterFactory : DltTraceWriterFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceWriterFactory"/> class.
        /// </summary>
        public DltFileTraceWriterFactory() : base(new DltFileTraceEncoderFactory()) { }
    }
}
