﻿namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Resources;
    using RJCP.Core;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Decodes a single PCAP packet.
    /// </summary>
    public sealed class PacketDecoder : IDisposable
    {
        private readonly int m_LinkType;
        private readonly ITraceDecoderFactory<DltTraceLineBase> m_TraceDecoderFactory;
        private readonly Dictionary<ConnectionKey, Connection> m_Connections = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDecoder"/> class.
        /// </summary>
        /// <param name="linkType">Type of the link, as defined in the PCAP file.</param>
        /// <param name="factory">The factory to return a <see cref="IPcapTraceDecoder"/>.</param>
        /// <exception cref="UnknownPcapFileFormatException">The link type is not supported.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
        public PacketDecoder(int linkType, ITraceDecoderFactory<DltTraceLineBase> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);

            switch (linkType) {
            case LinkTypes.LINKTYPE_ETHERNET:
            case LinkTypes.LINKTYPE_LINUX_SLL:
                m_LinkType = linkType;
                break;
            default:
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }
            m_TraceDecoderFactory = factory;
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
                if (!ScanEthernetHeader(buffer).TryGet(out offset))
                    return Array.Empty<DltTraceLineBase>();
                break;
            case LinkTypes.LINKTYPE_LINUX_SLL:
                if (!ScanSllHeader(buffer).TryGet(out offset))
                    return Array.Empty<DltTraceLineBase>();
                break;
            default:
                throw new UnknownPcapFileFormatException(AppResources.DomainPcapUnknownLinkFormat);
            }

            // Offset now points to the 'proto' field.
            if (!ScanVlanHeader(buffer[offset..]).TryGet(out int vlanOffset))
                return Array.Empty<DltTraceLineBase>();
            offset += vlanOffset;

            ushort proto = unchecked((ushort)BitOperations.To16ShiftBigEndian(buffer[offset..]));
            switch (proto) {
            case 0x800:
                return ScanIpHeader(buffer[(offset + 2)..], timeStamp, position + offset + 2);
            case 0x99FE:
            case 0x2090:
                return ScanTecmpHeader(buffer[(offset + 2)..], timeStamp, position + offset + 2);
            default:
                return Array.Empty<DltTraceLineBase>();
            }
        }

        /// <summary>
        /// Parses the header in the "Ethernet Frame" format.
        /// </summary>
        /// <param name="buffer">The buffer with the first byte being in the payload.</param>
        /// <returns>The size of the header, which is 12 bytes.</returns>
        /// <remarks>The Ethernet Frame header contains the destination MAC, source MAC fields.</remarks>
        private static Result<int> ScanEthernetHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 14) return Result.FromException<int>(new UnknownPcapPacketException("Ethernet Buffer too small"));
            return 12;
        }

        /// <summary>
        /// Parses the header in the "Linux Cooked Frame" format.
        /// </summary>
        /// <param name="buffer">The buffer with the first byte being in the payload.</param>
        /// <returns>The size of the header, which is 14 bytes.</returns>
        /// <remarks>The Linux Cooked Frame header contains the packet type, and essentially only the sender.</remarks>
        private static Result<int> ScanSllHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 16) return Result.FromException<int>(new UnknownPcapPacketException("Ethernet SSL Buffer too small"));
            return 14;
        }

        /// <summary>
        /// Scans the header, stripping away any VLAN headers.
        /// </summary>
        /// <param name="buffer">The buffer after the frame, pointing to the protocol field.</param>
        /// <returns>The number of bytes to skip to get the next protocol field.</returns>
        /// <remarks>
        /// Scans the header of 0x88a8/0x9100 (802.1ad header), then 0x8100 (802.1q header), and returns an offset of
        /// either 0 (no VLAN headers); 4 (one VLAN header); or 8 (Q-in-Q header and VLAN header). It will ensure that
        /// there is enough bytes so that one can safely check for the non-VLAN proto field afterwards.
        /// </remarks>
        private static Result<int> ScanVlanHeader(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 6) return Result.FromException<int>(new UnknownPcapPacketException("PCAP packet too small"));

            int proto = unchecked((ushort)BitOperations.To16ShiftBigEndian(buffer));
            if (proto is not 0x8100 and not 0x9100 and not 0x88A8) return 0;

            if (buffer.Length < 10) return Result.FromException<int>(new UnknownPcapPacketException("PCAP packet too small"));
            proto = unchecked((ushort)BitOperations.To16ShiftBigEndian(buffer[4..]));
            if (proto != 0x8100) return 4;

            return 8;
        }

        /// <summary>
        /// Scans the payload looking for DLT data starting at the first byte of the IPv4 header (skipping over the
        /// 0x800 proto field).
        /// </summary>
        /// <param name="buffer">The buffer where the IPv4 packet starts.</param>
        /// <param name="timeStamp">The time stamp obtained from the Wireshark Packet.</param>
        /// <param name="position">The position where the IPv4 header starts.</param>
        /// <returns>A collection of decoded DLT frames.</returns>
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

        private bool m_TecmpVersionError = false;
        private int m_TecmpVersionCount = 0;

        /// <summary>
        /// Scans the payload looking for DLT data starting at the first byte of the TECMP header.
        /// </summary>
        /// <param name="buffer">The buffer where the TECMP packet starts.</param>
        /// <param name="timeStamp">The time stamp obtained from the Wireshark Packet.</param>
        /// <param name="position">The position where the TECMP header starts.</param>
        /// <returns>A collection of decoded DLT frames.</returns>
        /// <remarks>
        /// This method will decode the inner Ethernet frame looking for the IPv4 packet to extract the DLT payload.
        /// </remarks>
        private IEnumerable<DltTraceLineBase> ScanTecmpHeader(ReadOnlySpan<byte> buffer, DateTime timeStamp, long position)
        {
            // TECMP Header is 12 bytes, Payload is 16 bytes.
            if (buffer.Length < 28) return Array.Empty<DltTraceLineBase>();

            int version = buffer[4];
            if (version != 3) {
                if (!m_TecmpVersionError && m_TecmpVersionCount < 5) {
                    // We log the first error only, not to spam the log output.
                    m_TecmpVersionError = true;
                    m_TecmpVersionCount++;
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "TECMP unknown version {0} at offset 0x{1:x}", version, position);
                    return Array.Empty<DltTraceLineBase>();
                }
            } else {
                m_TecmpVersionError = false;
            }

            // TECMP_MSG_TYPE_LOG_STREAM = 0x03
            if (buffer[5] != 0x03) return Array.Empty<DltTraceLineBase>();

            // TECMP_DATA_TYPE_ETH = 0x0080
            if (BitOperations.To16ShiftBigEndian(buffer[6..]) != 0x0080) return Array.Empty<DltTraceLineBase>();

            ushort cmflags = unchecked((ushort)BitOperations.To16ShiftBigEndian(buffer[10..]));
            if ((cmflags & 0x03) != 0x03) {
                Log.Pcap.TraceEvent(TraceEventType.Warning,
                    "TECMP packet segmented (0x{0}) at offset 0x{1:x}, not supported", cmflags, position);
                return Array.Empty<DltTraceLineBase>();
            }
            if ((cmflags & 0x8000) != 0) {
                Log.Pcap.TraceEvent(TraceEventType.Information,
                    "TECMP packet capture overflow flag set at offset 0x{0:x}", position);
            }

            return ScanTecmpPayload(buffer[12..], timeStamp, position + 12);
        }

        private IEnumerable<DltTraceLineBase> ScanTecmpPayload(ReadOnlySpan<byte> buffer, DateTime timeStamp, long position)
        {
            List<DltTraceLineBase> lines = null;
            IEnumerable<DltTraceLineBase> payload = null;

            while (true) {
                // The TECMP payload header, version 0x03 is 16 bytes in length.
                if (buffer.Length < 14) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "TECMP packet partial payload at 0x{0}, TECMP packet too small, length {1}", position, buffer.Length);
                    return TecmpResult(lines, payload);
                }

                int length = unchecked((ushort)BitOperations.To16ShiftBigEndian(buffer[12..]));
                if (buffer.Length < 16 + length) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "TECMP packet partial payload at 0x{0}, TECMP payload length {1} greater than packet length {2}", position, length, buffer.Length);
                    return TecmpResult(lines, payload);
                }

                // This is the captured Ethernet packet. The format is expected to be Version 0x03,
                // TECMP_MSG_TYPE_LOG_STREAM, TECMP_DATA_TYPE_ETH.
                ReadOnlySpan<byte> ethBuff = buffer[16..(16 + length)];
                if (ethBuff.Length < 14) {
                    Log.Pcap.TraceEvent(TraceEventType.Warning,
                        "TECMP packet partial payload at 0x{0}, TECMP payload too small, length {1}", position, ethBuff.Length);
                } else {
                    if (!ScanVlanHeader(ethBuff[12..]).TryGet(out int poffset))
                        return TecmpResult(lines, payload);

                    ushort proto = unchecked((ushort)BitOperations.To16ShiftBigEndian(ethBuff[(poffset + 12)..]));
                    switch (proto) {
                    case 0x800:
                        // We have to be careful here, the output of the ScanIpHeader returns the same collection as it did
                        // last time, just the content may now be different. Thus, we must do this check before scanning the
                        // payload, else, we'll discard the last result.
                        if (payload is not null && lines is null)
                            lines = new List<DltTraceLineBase>(payload);

                        // 12 bytes for MAC; 2 bytes for proto; poffset for VLAN header.
                        payload = ScanIpHeader(ethBuff[(poffset + 14)..], timeStamp, position + 16 + 14 + poffset);
                        lines?.AddRange(payload);
                        break;
                    default:
                        break;
                    }
                }

                buffer = buffer[(16 + length)..];
                position += length + 16;
                if (buffer.Length == 0) return TecmpResult(lines, payload);
            }
        }

        private static IEnumerable<DltTraceLineBase> TecmpResult(List<DltTraceLineBase> lines, IEnumerable<DltTraceLineBase> packet)
        {
            // A small optimisation, that we don't create a list unless we need to, and when we return, we return the
            // list only if it was created. Otherwise the list of packets that we decoded.
            if (lines is not null) return lines;
            if (packet is not null) return packet;
            return Array.Empty<DltTraceLineBase>();
        }

        private Connection GetConnection(int srcAddr, int dstAddr)
        {
            ConnectionKey key = new(srcAddr, dstAddr);
            if (!m_Connections.TryGetValue(key, out Connection connection)) {
                connection = new Connection(srcAddr, dstAddr, m_TraceDecoderFactory);
                m_Connections.Add(key, connection);
            }
            return connection;
        }

        private IPcapTraceDecoder GetDecoder(int srcAddr, short srcPort, int dstAddr, short dstPort)
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
                            "Discarded fragmented UDP packets on second attempt, reason {0}, fragment identifier {1:x4}, Timestamp {2:u}",
                            result, unchecked((ushort)fragId), timeStamp);
                        return Array.Empty<DltTraceLineBase>();
                    }
                    retry = true;

                    if (Log.Pcap.ShouldTrace(TraceEventType.Warning)) {
                        StringBuilder sb = new();
                        foreach (IpFragment fragment in ipFragments.GetFragments()) {
                            if (sb.Length != 0) sb.Append(", ");
                            sb.AppendFormat("(0x{0:x}, 0x{1:x})", fragment.Position, fragment.FragmentOffset);
                        }
                        Log.Pcap.TraceEvent(TraceEventType.Warning,
                            "Discarded fragmented UDP packets, reason {0}, fragment identifier {1:x4}, Timestamp {2:u}. Discarded: Packet Offset, Frag Offset {3}",
                            result, unchecked((ushort)fragId), timeStamp, sb.ToString());
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
            IPcapTraceDecoder decoder = null;
            List<DltTraceLineBase> lines = new();

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

                lines.AddRange(decoder.Decode(dltBuffer, position));
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

            IPcapTraceDecoder decoder = GetDecoder(srcAddr, srcPort, dstAddr, dstPort);
            decoder.PacketTimeStamp = timeStamp;
            return decoder.Decode(udpBuffer[8..udpLen], position + 8);
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
