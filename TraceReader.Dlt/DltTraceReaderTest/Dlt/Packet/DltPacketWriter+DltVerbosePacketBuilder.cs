namespace RJCP.Diagnostics.Log.Dlt.Packet
{
    using System;

    public sealed partial class DltPacketWriter
    {
        /// <summary>
        /// Defines instructions on how to build a verbose DLT packet.
        /// </summary>
        public sealed class DltVerbosePacketBuilder
        {
            private readonly int m_Count;
            private readonly Packet m_Packet = new Packet();
            private readonly DltPacketWriter m_DltPacketWriter;

            internal DltVerbosePacketBuilder(DltPacketWriter packet)
            {
                m_DltPacketWriter = packet;
                m_Count = m_DltPacketWriter.Counter;
                m_DltPacketWriter.Counter = (byte)((m_DltPacketWriter.Counter + 1) % 256);
            }

            /// <summary>
            /// Prepares a Verbose packet.
            /// </summary>
            /// <param name="time">The device time stamp.</param>
            /// <param name="msgType">The type of the message.</param>
            /// <param name="text">The text message argument.</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder Line(TimeSpan time, DltType msgType, string text)
            {
                m_Packet.CreateStandardHeader(time, m_DltPacketWriter.EcuId, m_DltPacketWriter.SessionId, m_Count);
                m_Packet.CreateExtendedHeader(msgType, m_DltPacketWriter.AppId, m_DltPacketWriter.CtxId);
                if (text != null) m_Packet.AddStringArg(text);
                return this;
            }

            /// <summary>
            /// Prepares a Verbose packet.
            /// </summary>
            /// <param name="time">The device time stamp.</param>
            /// <param name="msgType">The type of the message.</param>
            /// <param name="noar">The number of verbose arguments in the payload.</param>
            /// <param name="payload">The raw verbose payload data.</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder Line(TimeSpan time, DltType msgType, int noar, byte[] payload)
            {
                m_Packet.CreateStandardHeader(time, m_DltPacketWriter.EcuId, m_DltPacketWriter.SessionId, m_Count);
                m_Packet.CreateExtendedHeader(msgType, m_DltPacketWriter.AppId, m_DltPacketWriter.CtxId);
                if (payload != null) m_Packet.AddPayload(noar, payload);
                return this;
            }

            /// <summary>
            /// Adds a payload for a control message
            /// </summary>
            /// <param name="time">The device time stamp.</param>
            /// <param name="msgType">The type of the message.</param>
            /// <param name="payload">The raw control payload data.</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder Control(TimeSpan time, DltType msgType, byte[] payload)
            {
                m_Packet.CreateStandardHeader(time, m_DltPacketWriter.EcuId, m_DltPacketWriter.SessionId, m_Count);
                m_Packet.CreateExtendedHeader(msgType, m_DltPacketWriter.AppId, m_DltPacketWriter.CtxId, false);
                if (payload != null) m_Packet.AddPayload(0, payload);
                return this;
            }

            /// <summary>
            /// Adds a payload for a non-verbose message
            /// </summary>
            /// <param name="time">The device time stamp.</param>
            /// <param name="payload">The non-verbose payload.</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder NonVerbose(TimeSpan time, byte[] payload)
            {
                m_Packet.CreateStandardHeader(time, m_DltPacketWriter.EcuId, m_DltPacketWriter.SessionId, m_Count);
                if (payload != null) m_Packet.AddPayload(payload);
                return this;
            }

            /// <summary>
            /// Marks this packet as big endian. Only the flag is set.
            /// </summary>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder BigEndian()
            {
                return BigEndian(true);
            }

            /// <summary>
            /// Marks this packet as big endian or little endian. Only the flag is set.
            /// </summary>
            /// <param name="bigEndian">
            /// Sets to Big Endian if <see langword="true"/>, else is set to Little Endian when <see langword="false"/>.
            /// </param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder BigEndian(bool bigEndian)
            {
                m_Packet.BigEndian(bigEndian);
                return this;
            }

            /// <summary>
            /// Writes the specified version into the standard header.
            /// </summary>
            /// <param name="version">The version in the range 0 to 7.</param>
            /// <returns>This object.</returns>
            /// <remarks>The line must have already been constructed.</remarks>
            public DltVerbosePacketBuilder Version(int version)
            {
                m_Packet.WriteStandardHeaderVersion(version);
                return this;
            }

            /// <summary>
            /// Overrides the calculated length with the current length.
            /// </summary>
            /// <param name="length">The length reported in the packet (not the actual packet length).</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder Length(int length)
            {
                m_Packet.SetLength(length);
                return this;
            }

            /// <summary>
            /// Creats a serial marker for this packet.
            /// </summary>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder SerialMarker()
            {
                m_Packet.CreateSerialMarker();
                return this;
            }

            /// <summary>
            /// Creates a storage header for this packet.
            /// </summary>
            /// <param name="time">The DLT time of the logger.</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder StorageHeader(DateTime time)
            {
                m_Packet.CreateStorageHeader(time, m_DltPacketWriter.EcuId);
                return this;
            }

            /// <summary>
            /// Creates a storage header for this packet.
            /// </summary>
            /// <param name="time">The DLT time of the logger.</param>
            /// <param name="ecuId">The ECU ID to use in the storage header.</param>
            /// <returns>This object.</returns>
            public DltVerbosePacketBuilder StorageHeader(DateTime time, string ecuId)
            {
                m_Packet.CreateStorageHeader(time, ecuId);
                return this;
            }

            /// <summary>
            /// Builds this verbose packet.
            /// </summary>
            /// <returns>The serialized packet.</returns>
            public byte[] Build()
            {
                return m_Packet.GetPacket();
            }

            /// <summary>
            /// Appends this instance to the <see cref="DltPacketWriter"/>.
            /// </summary>
            /// <returns>The length of the packet that was built and added.</returns>
            public int Append()
            {
                m_DltPacketWriter.m_Packets.Add(Build());
                return m_Packet.Length;
            }

            /// <summary>
            /// Appends this instance to the <see cref="DltPacketWriter"/> but truncates the output.
            /// </summary>
            /// <param name="maxLength">The maximum length to truncate at.</param>
            /// <returns>The length of the packet that was built and added.</returns>
            public int Append(int maxLength)
            {
                byte[] fullPacket = Build();
                if (maxLength < fullPacket.Length) {
                    m_DltPacketWriter.m_Packets.Add(fullPacket[..maxLength]);
                    return maxLength;
                }
                m_DltPacketWriter.m_Packets.Add(fullPacket);
                return fullPacket.Length;
            }
        }
    }
}
