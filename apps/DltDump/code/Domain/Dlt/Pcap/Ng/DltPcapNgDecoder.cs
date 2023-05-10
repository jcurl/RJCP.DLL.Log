namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Resources;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A DLT decoder that extracts DLT from UDP packets in a PCAP-NG file.
    /// </summary>
    public class DltPcapNgDecoder : ITraceDecoder<DltTraceLineBase>
    {
        /// <summary>
        /// The minimum size for a PCAP-NG block.
        /// </summary>
        private const int PcapRecHdrLen = 12;

        private readonly BlockReader m_BlockReader;
        private readonly byte[] m_Packet = new byte[65536];
        private int m_Length;
        private long m_Position;
        private uint m_ExpectedLength;
        private long m_DiscardLength;
        private bool m_FileCorrupted;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNgDecoder" /> class without an output stream or filter.
        /// </summary>
        /// <param name="factory">The factory to return a <see cref="IPcapTraceDecoder"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
        public DltPcapNgDecoder(ITraceDecoderFactory<DltTraceLineBase> factory)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            m_BlockReader = new BlockReader(factory);
        }

        private readonly List<DltTraceLineBase> m_List = new List<DltTraceLineBase>();

        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
        /// <exception cref="UnknownPcapFileFormatException">Indicates that the file is corrupted.</exception>
        /// <remarks>
        /// The <see cref="ITraceDecoder.Decode(ReadOnlySpan{byte},long)"/> method shall accept any number of bytes for
        /// decoding. It should also consume all data that is received, so that data which is not processed is buffered
        /// locally by the decoder.
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// <para>
        /// If there was a problem decoding that decoding can no longer continue, an exception should normally be
        /// raised, that will be propagated to the caller. In case that the caller should see that processing finished
        /// normally, but before the stream is finished, return <see langword="null"/>.
        /// </para>
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(DltPcapNgDecoder));
            if (m_FileCorrupted)
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapBlockCorruption);

            m_List.Clear();

            while (buffer.Length > 0) {
                if (m_DiscardLength > 0) {
                    if (m_DiscardLength >= buffer.Length) {
                        m_DiscardLength -= buffer.Length;
                        return m_List;
                    }
                    int offset = (int)m_DiscardLength;
                    buffer = buffer[offset..];
                    m_DiscardLength = 0;
                    position += offset;
                }

                PcapBlock blockHeader;
                ReadOnlySpan<byte> packet;
                if (m_Length == 0) {
                    if (buffer.Length < PcapRecHdrLen) {
                        buffer.CopyTo(m_Packet);
                        m_Length = buffer.Length;
                        m_Position = position;
                        return m_List;
                    }

                    blockHeader = m_BlockReader.GetHeader(buffer);
                    if (blockHeader.Length == 0) return SetFileCorrupted();

                    m_ExpectedLength = unchecked((uint)blockHeader.Length);
                    if (m_ExpectedLength > m_Packet.Length) {
                        Log.Pcap.TraceEvent(TraceEventType.Verbose,
                            "Skipping excessive packet, position 0x{0:x}, length 0x{1:x} (max 0x{2:x})",
                            position, m_ExpectedLength, m_Packet.Length);

                        // We discard this packet, as it is too big.
                        m_DiscardLength = m_ExpectedLength;
                        m_ExpectedLength = 0;
                        continue;
                    }

                    if (m_ExpectedLength > buffer.Length) {
                        // We copy the partial packet, and collect the rest later.
                        buffer.CopyTo(m_Packet);
                        m_Length = buffer.Length;
                        m_Position = position;
                        return m_List;
                    }

                    // We decode the packet in the buffer
                    packet = buffer[0..(int)m_ExpectedLength];
                    m_Position = position;

                    buffer = buffer[(int)m_ExpectedLength..];
                    position += m_ExpectedLength;
                } else if (m_Length < PcapRecHdrLen) {
                    if (buffer.Length + m_Length < PcapRecHdrLen) {
                        buffer.CopyTo(m_Packet.AsSpan(m_Length));
                        m_Length += buffer.Length;
                        return m_List;
                    }

                    int hdr = PcapRecHdrLen - m_Length;
                    buffer[0..hdr].CopyTo(m_Packet.AsSpan(m_Length));
                    buffer = buffer[hdr..];
                    position += hdr;
                    blockHeader = m_BlockReader.GetHeader(m_Packet);
                    if (blockHeader.Length == 0) return SetFileCorrupted();

                    m_ExpectedLength = unchecked((uint)blockHeader.Length);
                    if (m_ExpectedLength > m_Packet.Length) {
                        Log.Pcap.TraceEvent(TraceEventType.Verbose,
                            "Skipping excessive packet, position 0x{0:x}, length 0x{1:x} (max 0x{2:x})",
                            position - hdr, m_ExpectedLength, m_Packet.Length);

                        // We discard this packet, as it is too big. We've already consumed PcapHdrLen bytes.
                        m_DiscardLength = m_ExpectedLength - PcapRecHdrLen;
                        m_ExpectedLength = 0;
                        m_Length = 0;
                        continue;
                    }

                    m_Length = PcapRecHdrLen;
                    continue;
                } else {
                    if (buffer.Length + m_Length < m_ExpectedLength) {
                        buffer.CopyTo(m_Packet.AsSpan(m_Length));
                        m_Length += buffer.Length;
                        return m_List;
                    }

                    int pkt = (int)m_ExpectedLength - m_Length;
                    buffer[0..pkt].CopyTo(m_Packet.AsSpan(m_Length));
                    buffer = buffer[pkt..];
                    position += pkt;

                    packet = m_Packet.AsSpan(0, m_Length + pkt);
                    blockHeader = m_BlockReader.GetHeader(packet);
                    if (blockHeader.Length == 0) return SetFileCorrupted();

                    m_Length = 0;
                }

                switch (blockHeader.BlockId) {
                case BlockCodes.SectionHeaderBlock:
                case BlockCodes.InterfaceDescriptionBlock:
                    m_BlockReader.GetBlock(packet, m_Position);
                    break;
                case BlockCodes.EnhancedPacketBlock:
                    IEnumerable<DltTraceLineBase> lines = m_BlockReader.DecodeBlock(packet, m_Position);
                    if (lines is null) return SetFileCorrupted();
                    m_List.AddRange(lines);
                    break;
                }
            }

            return m_List;
        }

        /// <summary>
        /// Flags that the stream we're parsing is corrupted.
        /// </summary>
        /// <returns>The currently parsed lines.</returns>
        /// <exception cref="UnknownPcapFileFormatException">Indicates that the file is corrupted.</exception>
        private IEnumerable<DltTraceLineBase> SetFileCorrupted()
        {
            m_FileCorrupted = true;
            return m_List;
        }

        /// <summary>
        /// Flushes any data that is locally cached, and returns any pending trace lines.
        /// </summary>
        /// <returns>A read only collection of the decoded lines.</returns>
        /// <exception cref="UnknownPcapFileFormatException">Corruption was encountered in the PCAP file.</exception>
        /// <remarks>
        /// Flushing a decoder typically happens by the trace reader when the stream is finished, so that any remaining
        /// data the decoder may have can be returned to the user application (including error trace lines).
        /// </remarks>
        public IEnumerable<DltTraceLineBase> Flush()
        {
            if (m_FileCorrupted)
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapBlockCorruption);

            return Array.Empty<DltTraceLineBase>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed or unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Is <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                if (!m_IsDisposed) {
                    m_BlockReader.Dispose();
                }
                m_IsDisposed = true;
            }
        }
    }
}
