namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Resources;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Decodes a single PCAP packet.
    /// </summary>
    public sealed class PacketDecoder : IDisposable
    {
        private readonly int m_LinkType;
        private readonly DltPcapNetworkTraceFilterDecoder m_Decoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDecoder"/> class.
        /// </summary>
        /// <param name="linkType">Type of the link, as defined in the PCAP file.</param>
        /// <exception cref="UnknownPcapFileFormatException">The link type is not supported.</exception>
        public PacketDecoder(int linkType) : this(linkType, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDecoder"/> class.
        /// </summary>
        /// <param name="linkType">Type of the link, as defined in the PCAP file.</param>
        /// <param name="outputStream">The output stream, where to write to.</param>
        /// <exception cref="UnknownPcapFileFormatException">The link type is not supported.</exception>
        public PacketDecoder(int linkType, IOutputStream outputStream)
        {
            switch (linkType) {
            case LinkTypes.LINKTYPE_ETHERNET:
            case LinkTypes.LINKTYPE_LINUX_SLL:
                m_LinkType = linkType;
                break;
            default:
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }

            m_Decoder = new DltPcapNetworkTraceFilterDecoder(outputStream);
        }

        /// <summary>
        /// Decodes the packet.
        /// </summary>
        /// <param name="buffer">The buffer where the packet starts, length being the captured data.</param>
        /// <param name="position">The position.</param>
        /// <returns>An enumerable list of lines that were decoded.</returns>
        /// <remarks>
        /// If a packet is decoded as having errors, or not being a proper UDP packet with the correct destination port,
        /// it is ignored (no errors are returned).
        /// </remarks>
        public IEnumerable<DltTraceLineBase> DecodePacket(ReadOnlySpan<byte> buffer, DateTime timeStamp, long position)
        {
            int offset;
            switch (m_LinkType) {
            case LinkTypes.LINKTYPE_ETHERNET:
                offset = GetIpHdrEthernetOffset(buffer);
                break;
            case LinkTypes.LINKTYPE_LINUX_SLL:
                offset = GetIpHdrSllOffset(buffer);
                break;
            default:
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }
            if (offset == -1) return Array.Empty<DltTraceLineBase>();

            int ipLen = BitOperations.To16ShiftBigEndian(buffer[(offset + 2)..]);
            if (ipLen < 20 || ipLen > buffer.Length - offset) return Array.Empty<DltTraceLineBase>();

            // The IPv4 packet, see https://datatracker.ietf.org/doc/html/rfc791
            ReadOnlySpan<byte> ipPacket = buffer[offset..(offset + ipLen)];

            // Check IPv4 fields that UDP is present.
            if ((ipPacket[0] & 0xF0) != 0x40) return Array.Empty<DltTraceLineBase>();    // Not IPv4
            if (ipPacket[9] != 17) return Array.Empty<DltTraceLineBase>();               // Not UDP

            int ihl = (ipPacket[0] & 0x0F) << 2;
            int flags = (ipPacket[6] & 0xE0) >> 5;
            int fragOffset = (((ipPacket[6] & 0x1F) << 8) + ipPacket[7]) * 8;
            if (fragOffset != 0) return Array.Empty<DltTraceLineBase>();                 // IP fragmented, MF = x
            if (ipLen < ihl + 16) return Array.Empty<DltTraceLineBase>();                // Corrupted packet

            // Check UDP fields, https://datatracker.ietf.org/doc/html/rfc768
            int dstPort = BitOperations.To16ShiftBigEndian(ipPacket[(ihl + 2)..]);
            if (dstPort != 3490) return Array.Empty<DltTraceLineBase>();                 // Not destination port 3490
            int udpLen = BitOperations.To16ShiftBigEndian(ipPacket[(ihl + 4)..]);

            if ((flags & 0x01) != 0) {                                                   // IP fragmented, MF = 1
                // This is the first packet of a fragmented UDP datagram, and more packets are to follow. The rest are
                // discarded above when the packet fragmentation offset is non-zero.
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Discarded fragmented UDP packet (MF=1), position 0x{0:x}, Data Length {1}",
                    position, udpLen - 8);
                return Array.Empty<DltTraceLineBase>();
            }

            if (ipLen < ihl + udpLen) {
                // The packet is corrupted.
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Discarded invalid UDP packet, position 0x{0:x}, Data Length {1}, actual data size {2}",
                    position, udpLen - 8, ipLen - ihl - 8);
                return Array.Empty<DltTraceLineBase>();
            }

            position += offset + ihl + 8;
            ReadOnlySpan<byte> dltPacket = ipPacket.Slice(ihl + 8, udpLen - 8);

            // We decode and flush, as we expect each DLT packet to be complete in a UDP packet. If a DLT packet spans
            // across multiple UDP packets, then this will cause the packet to likely be discarded.
            m_Decoder.PacketTimeStamp = timeStamp;
            IEnumerable<DltTraceLineBase> lines = m_Decoder.Decode(dltPacket, position, true);
            return lines;
        }

        private static int GetIpHdrEthernetOffset(ReadOnlySpan<byte> buffer)
        {
            // Layer 2 Ethernet frame

            // An Ethernet frame must be at least 64 bytes. The FCS (4 bytes) might not be present. So our buffer must
            // be a minimum of 60 bytes. See https://wiki.wireshark.org/Ethernet. We choose to ignore this constraint,
            // as WireShark ignores it also.

            // In the Ethernet payload, we expect a IPv4 packet, with DLT header (no 802.1q tag)
            // * Eth Header = 6 (src) + 6 (dst) + 2 (proto) = 14 bytes
            // * IPv4 Header = 20 bytes (src, dest, proto, length)
            // * UDP Header = 8 bytes (src port, dest port, length, checksum)
            // * DLT Header + Message ID = 8 bytes
            // -> Total = 50 bytes (minimum size for a minimum DLT packet in a UDP frame)

            if (buffer.Length < 50) return -1;

            int offset;
            short proto = BitOperations.To16ShiftBigEndian(buffer[12..]);
            if (proto != unchecked((short)0x8100)) {
                offset = 14;
            } else {
                proto = BitOperations.To16ShiftBigEndian(buffer[16..]);
                offset = 18;
            }

            // This is not an IPv4 prototype packet.
            if (proto != 0x0800) return -1;

            return offset;
        }

        private static int GetIpHdrSllOffset(ReadOnlySpan<byte> buffer)
        {
            // Layer 2 Linux Self Cooked. See https://www.tcpdump.org/linktypes/LINKTYPE_LINUX_SLL.html

            // In the Ethernet payload, we expect a IPv4 packet, with DLT header (no 802.1q tag)
            // * SLL Header = 16 bytes (no 802.1q header)
            // * IPv4 Header = 20 bytes (src, dest, proto, length)
            // * UDP Header = 8 bytes (src port, dest port, length, checksum)
            // * DLT Header + Message ID = 8 bytes
            // -> Total = 52 bytes (minimum size for a minimum DLT packet in a UDP frame)

            if (buffer.Length < 52) return -1;

            int offset;
            int proto = BitOperations.To16ShiftBigEndian(buffer[14..]);
            if (proto != unchecked((short)0x8100)) {
                offset = 16;
            } else {
                proto = BitOperations.To16ShiftBigEndian(buffer[18..]);
                offset = 20;
            }

            // This is not an IPv4 prototype packet.
            if (proto != 0x0800) return -1;

            return offset;
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed) {
                m_Decoder.Dispose();
            }
            m_IsDisposed = true;
        }
    }
}
