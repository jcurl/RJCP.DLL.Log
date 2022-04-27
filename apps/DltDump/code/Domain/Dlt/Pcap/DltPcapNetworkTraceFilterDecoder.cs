namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A DLT decoder for a network stream that can filter output and set the logging timestamp per packet when
    /// decoding.
    /// </summary>
    public sealed class DltPcapNetworkTraceFilterDecoder : DltTraceDecoder
    {
        private readonly IOutputStream m_OutputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNetworkTraceFilterDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentException"><paramref name="outputStream"/> doesn't support binary mode.</exception>
        public DltPcapNetworkTraceFilterDecoder(IOutputStream outputStream)
        {
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
            if (m_OutputStream != null) {
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

            if (m_OutputStream != null) {
                m_OutputStream.Write(line);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets or sets the packet time stamp.
        /// </summary>
        /// <value>The packet time stamp.</value>
        public DateTime PacketTimeStamp { get; set; }

        /// <summary>
        /// Parses any headers at the start of the packet.
        /// </summary>
        /// <param name="dltPacket">
        /// The DLT packet where offset zero is the start of the packet found, including the marker as returned by
        /// <see cref="ScanStartFrame"/>.
        /// </param>
        /// <param name="lineBuilder">The line builder.</param>
        /// <returns>
        /// Returns <see langword="true"/> always, as there is no prefix header that needs to be parsed.
        /// </returns>
        protected override bool ParsePrefixHeader(ReadOnlySpan<byte> dltPacket, IDltLineBuilder lineBuilder)
        {
            lineBuilder.SetTimeStamp(PacketTimeStamp);
            return true;
        }
    }
}
