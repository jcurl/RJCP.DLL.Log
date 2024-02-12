namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure.IO;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A factory for common operations to instantiate a <see cref="ITraceReader{DltTraceLineBase}"/>.
    /// </summary>
    /// <remarks>
    /// Uses the default implementation of <see cref="TraceReaderFactory{T}"/> for a <see cref="DltTraceLineBase"/>, but
    /// extends a new <see cref="CreateAsync(IPacket)"/> for packet based inputs.
    /// </remarks>
    public class TracePacketReaderFactory : TraceReaderFactory<DltTraceLineBase>
    {
        private readonly ITraceDecoderFactory<DltTraceLineBase> m_DecoderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracePacketReaderFactory{DltTraceLineBase}"/> class.
        /// </summary>
        /// <param name="decoderFactory">The decoder factory.</param>
        /// <exception cref="ArgumentNullException"><paramref name="decoderFactory"/> is <see langword="null"/>.</exception>
        public TracePacketReaderFactory(ITraceDecoderFactory<DltTraceLineBase> decoderFactory)
            : base(decoderFactory)
        {
            m_DecoderFactory = decoderFactory;
        }

        /// <summary>
        /// Creates an <see cref="ITraceReader{DltTraceLineBaseT}" /> from a stream.
        /// </summary>
        /// <param name="packet">The packet reader stream.</param>
        /// <returns>The <see cref="ITraceReader{DltTraceLineBase}"/> object for log file enumeration.</returns>
        public Task<ITraceReader<DltTraceLineBase>> CreateAsync(IPacket packet)
        {
            ArgumentNullException.ThrowIfNull(packet);

            return Task.FromResult<ITraceReader<DltTraceLineBase>>(new TracePacketReader(packet, m_DecoderFactory));
        }
    }
}
