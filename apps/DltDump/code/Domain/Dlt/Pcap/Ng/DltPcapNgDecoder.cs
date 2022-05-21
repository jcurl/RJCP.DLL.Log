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

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNgDecoder"/> class without an output stream or filter.
        /// </summary>
        public DltPcapNgDecoder()
        {
            m_BlockReader = new BlockReader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltPcapNgDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputStream"/> is <see langword="null"/>.
        /// </exception>
        public DltPcapNgDecoder(IOutputStream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            m_BlockReader = new BlockReader(outputStream);
        }

        private readonly List<DltTraceLineBase> m_List = new List<DltTraceLineBase>();

        public IEnumerable<DltTraceLineBase> Decode(ReadOnlySpan<byte> buffer, long position)
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(nameof(DltPcapNgDecoder));

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
                    if (blockHeader.Length == 0) throw new UnknownPcapFileFormatException(AppResources.DomainPcapBlockCorruption);

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
                    if (blockHeader.Length == 0) throw new UnknownPcapFileFormatException(AppResources.DomainPcapBlockCorruption);

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
                    if (blockHeader.Length == 0) throw new UnknownPcapFileFormatException(AppResources.DomainPcapBlockCorruption);

                    m_Length = 0;
                }

                switch (blockHeader.BlockId) {
                case BlockCodes.SectionHeaderBlock:
                case BlockCodes.InterfaceDescriptionBlock:
                    m_BlockReader.GetBlock(packet, m_Position);
                    break;
                case BlockCodes.EnhancedPacketBlock:
                    m_List.AddRange(m_BlockReader.DecodeBlock(packet, m_Position));
                    break;
                }
            }

            return m_List;
        }

        public IEnumerable<DltTraceLineBase> Flush()
        {
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
