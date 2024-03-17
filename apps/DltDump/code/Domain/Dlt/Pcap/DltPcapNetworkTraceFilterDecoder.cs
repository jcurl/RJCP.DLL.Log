namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.Control;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;

    /// <summary>
    /// A DLT decoder for a network stream that can filter output and set the logging timestamp per packet when
    /// decoding.
    /// </summary>
    public sealed class DltPcapNetworkTraceFilterDecoder : DltTraceDecoder, IPcapTraceDecoder
    {
        private readonly IOutputStream m_OutputStream;
        private readonly IDltLineBuilder m_LineBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNetworkTraceFilterDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentException"><paramref name="outputStream"/> doesn't support binary mode.</exception>
        public DltPcapNetworkTraceFilterDecoder(IOutputStream outputStream)
            : this(outputStream, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNetworkTraceFilterDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="map">The map for decoding non-verbose messages.</param>
        /// <exception cref="ArgumentException"><paramref name="outputStream"/> doesn't support binary mode.</exception>
        public DltPcapNetworkTraceFilterDecoder(IOutputStream outputStream, IFrameMap map)
            : this(outputStream, map, new DltLineBuilder(false)) { }

        private DltPcapNetworkTraceFilterDecoder(IOutputStream outputStream, IFrameMap map, IDltLineBuilder lineBuilder)
            : base(GetVerboseDecoder(), GetNonVerboseDecoder(map), new ControlDltDecoder(), lineBuilder)
        {
            m_LineBuilder = lineBuilder;
            m_OutputStream = outputStream;
        }

        /// <summary>
        /// Checks the line before adding to the list of data that can be parsed.
        /// </summary>
        /// <param name="line">The line that should be checked.</param>
        /// <param name="packet">The raw packet data.</param>
        /// <returns>Returns <see langword="true" /> if the line should be added, <see langword="false" /> otherwise.</returns>
        protected override bool CheckLine(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            if (m_OutputStream is not null) {
                m_OutputStream.Write(line, packet);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks the skipped line before adding to the list of data that can be parsed.
        /// </summary>
        /// <param name="line">The skipped line that should be checked.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the line should be added, <see langword="false"/> otherwise.
        /// </returns>
        protected override bool CheckSkippedLine(DltTraceLineBase line)
        {
            // The base decoder doesn't set the time stamp for skipped data, as it assumes a single stream. In our case
            // to get here, we know the PCAP is correct, and so the time stamp is correct, but there is data corruption
            // in the UDP frame itself. So set the time stamp.
            line.TimeStamp = PacketTimeStamp;

            if (m_OutputStream is not null) {
                m_OutputStream.Write(line);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets or sets the packet time stamp.
        /// </summary>
        /// <value>The packet time stamp.</value>
        public DateTime PacketTimeStamp
        {
            get { return m_LineBuilder.TimeStamp; }
            set { m_LineBuilder.SetTimeStamp(value); }
        }
    }
}
