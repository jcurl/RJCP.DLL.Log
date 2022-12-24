namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Resources;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Decodes a single PCAP packet.
    /// </summary>
    public sealed class PacketDecoder : IDisposable
    {
        private readonly int m_LinkType;
        private readonly IOutputStream m_OutputStream;
        private readonly Dictionary<ConnectionKey, Connection> m_Connections = new Dictionary<ConnectionKey, Connection>();

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
            m_OutputStream = outputStream;
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
                offset = ScanEthernetHeader(buffer);
                break;
            case LinkTypes.LINKTYPE_LINUX_SLL:
                offset = ScanSslHeader(buffer);
                break;
            default:
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }
            if (offset < 0) return Array.Empty<DltTraceLineBase>();

            ushort proto;
            proto = GetProto(buffer[offset..]);
            if (proto == 0) return Array.Empty<DltTraceLineBase>();

            if (proto == 0x8100) {
                // Outer VLAN
                offset += 4;
                proto = GetProto(buffer[offset..]);
                if (proto == 0) return Array.Empty<DltTraceLineBase>();

                if (proto == 0x8100) {
                    // Inner VLAN
                    offset += 4;
                    proto = GetProto(buffer[offset..]);
                    if (proto == 0) return Array.Empty<DltTraceLineBase>();
                }
            }

            if (proto == 0x0800) {
                return ScanIpHeader(buffer[(offset + 2)..], timeStamp, position + offset + 2);
            }
            return Array.Empty<DltTraceLineBase>();
        }

        private static int ScanEthernetHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 14) return -1;
            return 12;
        }

        private static int ScanSslHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 16) return -1;
            return 14;
        }

        private static ushort GetProto(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 4) return 0;

            ushort proto = unchecked((ushort)BitOperations.To16ShiftBigEndian(buffer));
            return proto;
        }

        private IEnumerable<DltTraceLineBase> ScanIpHeader(ReadOnlySpan<byte> buffer, DateTime timeStamp, long position)
        {
            // * IPv4 Header = 20 bytes (src, dest, proto, length)
            // * UDP Header = 8 bytes (src port, dest port, length, checksum)
            // * DLT Header + Message ID = 8 bytes
            // -> Total = 36 bytes (minimum size for a minimum DLT packet in a UDP frame)
            if (buffer.Length < 36) return Array.Empty<DltTraceLineBase>();

            int ipLen = BitOperations.To16ShiftBigEndian(buffer[2..]);
            if (ipLen < 20 || ipLen > buffer.Length) return Array.Empty<DltTraceLineBase>();

            // The IPv4 packet, see https://datatracker.ietf.org/doc/html/rfc791
            ReadOnlySpan<byte> ipPacket = buffer[..ipLen];

            // Check IPv4 fields that UDP is present.
            if ((ipPacket[0] & 0xF0) != 0x40) return Array.Empty<DltTraceLineBase>();    // Not IPv4
            if (ipPacket[9] != 17) return Array.Empty<DltTraceLineBase>();               // Not UDP

            int ihl = (ipPacket[0] & 0x0F) << 2;
            if (ipLen < ihl + 16) return Array.Empty<DltTraceLineBase>();                // Corrupted packet

            int flags = (ipPacket[6] & 0xE0) >> 5;
            int fragOffset = (((ipPacket[6] & 0x1F) << 8) + ipPacket[7]) * 8;
            bool mf = (flags & 0x01) != 0;

            int srcAddr = BitOperations.To32ShiftBigEndian(ipPacket[12..]);
            int dstAddr = BitOperations.To32ShiftBigEndian(ipPacket[16..]);

            if (mf || fragOffset != 0) {
                // This is a fragmented IP packet
                int fragId = BitOperations.To16ShiftBigEndian(ipPacket[4..]);
                int hcs = BitOperations.To16ShiftBigEndian(ipPacket[10..]);
                return DecodeDltFragments(srcAddr, dstAddr, fragOffset, fragId, mf, hcs, ipPacket[ihl..ipLen], timeStamp, position + ihl);
            } else {
                // This is a single UDP packet
                return DecodeDltPacket(srcAddr, dstAddr, ipPacket[ihl..ipLen], timeStamp, position + ihl);
            }
        }

        private Connection GetConnection(int srcAddr, int dstAddr)
        {
            ConnectionKey key = new ConnectionKey(srcAddr, dstAddr);
            if (!m_Connections.TryGetValue(key, out Connection connection)) {
                connection = new Connection(srcAddr, dstAddr, m_OutputStream);
                m_Connections.Add(key, connection);
            }
            return connection;
        }

        private DltPcapNetworkTraceFilterDecoder GetDecoder(int srcAddr, short srcPort, int dstAddr, short dstPort)
        {
            Connection connection = GetConnection(srcAddr, dstAddr);
            return connection.GetDltDecoder(srcPort, dstPort);
        }

        private IEnumerable<DltTraceLineBase> DecodeDltFragments(int srcAddr, int dstAddr, int fragOffset, int fragId, bool mf, int hcs, ReadOnlySpan<byte> udpBuffer, DateTime timeStamp, long position)
        {
            bool retry = false;
            Connection connection = GetConnection(srcAddr, dstAddr);

            do {
                IpFragments ipFragments = connection.GetIpFragments(fragId);
                IpFragmentResult result = ipFragments.AddFragment(fragOffset, mf, hcs, udpBuffer, timeStamp, position);

                switch (result) {
                case IpFragmentResult.Incomplete:
                    return Array.Empty<DltTraceLineBase>();
                case IpFragmentResult.InvalidDuplicate:
                    // A duplicate packet is ignored, so that we don't discard properly decoded packets previously.
                    // Because we discard this packet, it won't be decoded, and the output would not contain a duplicate
                    // DLT packet. Best is if the user previously removed duplicates from the input, but at least we
                    // don't discard data.
                    return Array.Empty<DltTraceLineBase>();
                case IpFragmentResult.Reassembled:
                    IEnumerable<DltTraceLineBase> lines = DecodeDltFragments(connection, ipFragments);
                    connection.DiscardFragments(fragId);
                    return lines;
                default:
                    if (retry) {
                        // Shouldn't ever get here, because the array is empty. But just in case.
                        Log.Pcap.TraceEvent(TraceEventType.Error,
                            "Discarded fragmented UDP packets on second attempt, reason {0}, fragment identifier {1}, Timestamp {2:u}",
                            result, fragId, timeStamp);
                        return Array.Empty<DltTraceLineBase>();
                    }
                    retry = true;

                    if (Log.Pcap.ShouldTrace(TraceEventType.Warning)) {
                        StringBuilder sb = new StringBuilder();
                        foreach (IpFragment fragment in ipFragments.GetFragments()) {
                            if (sb.Length != 0) sb.Append(", ");
                            sb.AppendFormat("(0x{0:x}, 0x{1:x})", fragment.Position, fragment.FragmentOffset);
                        }
                        Log.Pcap.TraceEvent(TraceEventType.Warning,
                            "Discarded fragmented UDP packets, reason {0}, fragment identifier {1}, Timestamp {2:u}. Discarded: Packet Offset, Frag Offset {3}",
                            result, fragId, timeStamp, sb.ToString());
                    }
                    connection.DiscardFragments(fragId);
                    break;
                }
            } while (true);
        }

        private IEnumerable<DltTraceLineBase> DecodeDltFragments(Connection connection, IpFragments fragments)
        {
            bool first = true;
            short srcPort;
            short dstPort;
            int udpLen;

            ReadOnlySpan<byte> dltBuffer;
            DltPcapNetworkTraceFilterDecoder decoder = null;
            List<DltTraceLineBase> lines = new List<DltTraceLineBase>();

            foreach (IpFragment fragment in fragments.GetFragments()) {
                long position;
                if (first) {
                    srcPort = BitOperations.To16ShiftBigEndian(fragment.GetArray().AsSpan());
                    dstPort = BitOperations.To16ShiftBigEndian(fragment.GetArray().AsSpan(2));
                    if (dstPort != 3490) {
                        connection.DiscardFragments(fragments.FragmentId);
                        return Array.Empty<DltTraceLineBase>();
                    }

                    udpLen = BitOperations.To16ShiftBigEndian(fragment.GetArray().AsSpan(4));
                    if (fragments.Length < udpLen) {
                        // The packet is corrupted.
                        Log.Pcap.TraceEvent(TraceEventType.Warning,
                            "Discarded truncated fragmented UDP packet, position 0x{0:x} (fragment offset 0), UDP Length {1}, actual data size {2}, Timestamp {3:u}",
                            fragment.Position, udpLen, fragments.Length, fragments.TimeStamp);
                        return Array.Empty<DltTraceLineBase>();
                    }
                    dltBuffer = fragment.GetArray().AsSpan(8);
                    decoder = GetDecoder(connection.SourceAddress, srcPort, connection.DestinationAddress, dstPort);
                    decoder.PacketTimeStamp = fragments.TimeStamp;
                    position = fragment.Position + 8;
                    first = false;
                } else {
                    dltBuffer = fragment.GetArray().AsSpan();
                    position = fragment.Position;
                }

                lines.AddRange(decoder.Decode(dltBuffer, position, false));
            }
            return lines;
        }

        private IEnumerable<DltTraceLineBase> DecodeDltPacket(int srcAddr, int dstAddr, ReadOnlySpan<byte> udpBuffer, DateTime timeStamp, long position)
        {
            // Check UDP fields, https://datatracker.ietf.org/doc/html/rfc768
            short srcPort = BitOperations.To16ShiftBigEndian(udpBuffer);
            short dstPort = BitOperations.To16ShiftBigEndian(udpBuffer[2..]);
            if (dstPort != 3490) return Array.Empty<DltTraceLineBase>();                 // Not destination port 3490
            int udpLen = BitOperations.To16ShiftBigEndian(udpBuffer[4..]);

            if (udpBuffer.Length < udpLen) {
                // The packet is corrupted.
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "Discarded truncated UDP packet, position 0x{0:x}, UDP Length {1}, actual data size {2}, Timestamp {3:u}",
                    position, udpLen, udpBuffer.Length, timeStamp);
                return Array.Empty<DltTraceLineBase>();
            }

            DltPcapNetworkTraceFilterDecoder decoder = GetDecoder(srcAddr, srcPort, dstAddr, dstPort);
            decoder.PacketTimeStamp = timeStamp;

            // We decode but do not flush, as we handle each src:port, dst:port pair as it's own connection which is
            // expected to be a continuous stream. Lost packets will result in corruption.
            return decoder.Decode(udpBuffer[8..udpLen], position + 8, false);
        }

        private bool m_IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed) return;

            foreach (Connection connection in m_Connections.Values) {
                connection.Dispose();
            }
            m_IsDisposed = true;
        }
    }
}
