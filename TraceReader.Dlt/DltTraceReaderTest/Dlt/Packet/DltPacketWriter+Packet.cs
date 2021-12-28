namespace RJCP.Diagnostics.Log.Dlt.Packet
{
    using System;
    using System.Text;
    using RJCP.Core;

    public sealed partial class DltPacketWriter
    {

        private sealed class Packet
        {
            private bool m_HasSerialMarker;
            private bool m_HasStorageHeader;
            private bool m_HasStandardHeader;
            private bool m_HasExtendedHeader;

            private static readonly byte[] SerialMarker = new byte[] { 0x44, 0x4C, 0x53, 0x01 };
            private readonly byte[] m_StorageHeader = new byte[16];
            private readonly byte[] m_Packet = new byte[PacketSize];
            private int m_ExtendedHeaderPos = 0;
            private int m_PayLoadPos = 0;
            private int m_PacketLength = 0;
            private int m_OverrideLength = -1;

            internal Packet() { }

            public void CreateSerialMarker()
            {
                if (m_HasStorageHeader) throw new InvalidOperationException("Storage Header already constructed");
                if (m_HasSerialMarker) throw new InvalidOperationException("Serial Marker already constructed");
                m_HasSerialMarker = true;
            }

            public void CreateStorageHeader(DateTime time, string ecuId)
            {
                if (m_HasStorageHeader) throw new InvalidOperationException("Storage Header already constructed");
                if (m_HasSerialMarker) throw new InvalidOperationException("Serial Marker already constructed");
                m_StorageHeader[0] = (byte)'D';
                m_StorageHeader[1] = (byte)'L';
                m_StorageHeader[2] = (byte)'T';
                m_StorageHeader[3] = 1;

                long seconds = new DateTimeOffset(time).ToUnixTimeSeconds();
                int fractionalSecondTicks = (int)(time.Ticks % TimeSpan.TicksPerSecond);
                int microseconds = fractionalSecondTicks / ((int)TimeSpan.TicksPerMillisecond / 1000);

                BitOperations.Copy32ShiftLittleEndian(seconds, m_StorageHeader, 4);
                BitOperations.Copy32ShiftLittleEndian(microseconds, m_StorageHeader, 8);
                WriteId(m_StorageHeader, 12, ecuId);
                m_HasStorageHeader = true;
            }

            public void CreateStandardHeader(TimeSpan time, string ecuId, int sessionId, int counter)
            {
                if (m_HasStandardHeader) throw new InvalidOperationException("Standard Header already constructed");
                if (counter < 0) throw new ArgumentOutOfRangeException(nameof(counter), "Minimum zero arguments required");
                if (counter > 255) throw new ArgumentOutOfRangeException(nameof(counter), "Maximum 255 arguments allowed");

                int length = 4;
                byte headerType = 0x38;  // WSID + WTMS + VERS(1)
                if (ecuId != null) {
                    headerType |= 0x04;  // WEID
                    WriteId(m_Packet, length, ecuId);
                    length += 4;
                }

                m_Packet[0] = headerType;
                m_Packet[1] = unchecked((byte)counter);

                BitOperations.Copy32ShiftBigEndian(sessionId, m_Packet, length);
                length += 4;

                BitOperations.Copy32ShiftBigEndian(GetDltTimeStamp(time), m_Packet, length);
                length += 4;

                m_PacketLength = length;
                m_HasStandardHeader = true;
            }

            public void WriteStandardHeaderVersion(int version)
            {
                if (!m_HasStandardHeader) throw new InvalidOperationException("Standard Header not yet constructed");
                if (version < 0 || version > 7) throw new ArgumentOutOfRangeException(nameof(version));

                m_Packet[0] = (byte)(m_Packet[0] & 0x1F | (version << 5));
            }

            public void CreateExtendedHeader(DltType msgType, string appId, string ctxId)
            {
                if (!m_HasStandardHeader) throw new InvalidOperationException("Standard Header not yet constructed");
                if (m_HasExtendedHeader) throw new InvalidOperationException("Extended Header already constructed");

                m_ExtendedHeaderPos = m_PacketLength;
                m_Packet[0] |= 0x01;  // UEH
                m_Packet[m_ExtendedHeaderPos] = (byte)((byte)msgType | 0x01);
                m_Packet[m_ExtendedHeaderPos + 1] = 0;
                WriteId(m_Packet, m_ExtendedHeaderPos + 2, appId);
                WriteId(m_Packet, m_ExtendedHeaderPos + 6, ctxId);

                m_PayLoadPos = m_PacketLength + 10;
                m_PacketLength = m_PayLoadPos;
                m_HasExtendedHeader = true;
            }

            public void AddStringArg(string text)
            {
                if (!m_HasExtendedHeader) throw new InvalidOperationException("Arguments must have an extended header");

                BitOperations.Copy32ShiftLittleEndian(0x8200, m_Packet, m_PayLoadPos);
                byte[] payload = Encoding.UTF8.GetBytes(text);
                BitOperations.Copy16ShiftLittleEndian(payload.Length + 1, m_Packet, m_PayLoadPos + 4);
                payload.CopyTo(m_Packet, m_PayLoadPos + 6);
                m_Packet[m_PayLoadPos + 6 + payload.Length] = 0;
                m_Packet[m_ExtendedHeaderPos + 1] += 1;  // Increase number of arguments

                m_PayLoadPos += 7 + payload.Length;
                m_PacketLength = m_PayLoadPos;
            }

            public void AddPayload(int noar, byte[] data)
            {
                if (!m_HasExtendedHeader) throw new InvalidOperationException("Arguments must have an extended header");

                data.CopyTo(m_Packet, m_PayLoadPos);
                m_Packet[m_ExtendedHeaderPos + 1] += (byte)noar;
                m_PayLoadPos += data.Length;
                m_PacketLength = m_PayLoadPos;
            }

            public int Length
            {
                get
                {
                    if (m_HasStorageHeader) return m_PacketLength + m_StorageHeader.Length;
                    if (m_HasSerialMarker) return m_PacketLength + SerialMarker.Length;
                    return m_PacketLength;
                }
            }

            public byte[] GetPacket()
            {
                if (!m_HasExtendedHeader) throw new InvalidOperationException("Incomplete packet, missing extended header");

                if (m_OverrideLength >= 0) {
                    BitOperations.Copy16ShiftBigEndian(m_OverrideLength, m_Packet, 2);
                } else {
                    BitOperations.Copy16ShiftBigEndian(m_PacketLength, m_Packet, 2);
                }

                if (m_HasStorageHeader) {
                    byte[] packet = new byte[m_StorageHeader.Length + m_PacketLength];
                    m_StorageHeader.CopyTo(packet, 0);
                    Array.Copy(m_Packet, 0, packet, m_StorageHeader.Length, m_PacketLength);
                    return packet;
                } else if (m_HasSerialMarker) {
                    byte[] packet = new byte[SerialMarker.Length + m_PacketLength];
                    SerialMarker.CopyTo(packet, 0);
                    Array.Copy(m_Packet, 0, packet, SerialMarker.Length, m_PacketLength);
                    return packet;
                }
                return m_Packet[0..m_PacketLength];
            }

            private static void WriteId(byte[] buffer, int offset, string id)
            {
                int idLen = id.Length;

                buffer[offset] = idLen > 0 ? (byte)(id[0] & 0x7F) : (byte)0;
                buffer[offset + 1] = idLen > 1 ? (byte)(id[1] & 0x7F) : (byte)0;
                buffer[offset + 2] = idLen > 2 ? (byte)(id[2] & 0x7F) : (byte)0;
                buffer[offset + 3] = idLen > 3 ? (byte)(id[3] & 0x7F) : (byte)0;
            }

            private static int GetDltTimeStamp(TimeSpan time)
            {
                return unchecked((int)(time.Ticks / TimeSpan.TicksPerMillisecond * 10));
            }

            public void SetLength(int length)
            {
                if (length < 0) {
                    m_OverrideLength = -1;
                } else if (length <= ushort.MaxValue) {
                    m_OverrideLength = length;
                } else {
                    throw new ArgumentOutOfRangeException(nameof(length), "Length must be 16-bit in size");
                }
            }
        }
    }
}
