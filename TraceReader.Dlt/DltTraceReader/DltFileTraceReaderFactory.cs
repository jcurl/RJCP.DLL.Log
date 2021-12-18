namespace RJCP.Diagnostics.Log
{
    using Decoder;
    using Dlt;

    /// <summary>
    /// A factory for getting a DLT Trace Decoder.
    /// </summary>
    public class DltFileTraceReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceReaderFactory"/> class.
        /// </summary>
        public DltFileTraceReaderFactory() { }

        /// <summary>
        /// In a derived class, return an instance of the decoder for reading the stream.
        /// </summary>
        /// <returns>A <see cref="DltTraceDecoder"/> that knows how to decode the stream.</returns>
        protected override ITraceDecoder<DltTraceLineBase> GetDecoder()
        {
            return new DltFileTraceDecoder();
        }
    }
}
