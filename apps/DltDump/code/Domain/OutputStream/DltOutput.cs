namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Domain.Dlt;
    using RJCP.Core;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// An output module that can write binary data as it is received.
    /// </summary>
    public sealed class DltOutput : OutputBase, IOutputStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName) : base(fileName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName, bool force) : base(fileName, 0, force) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName, long split, bool force) : base(fileName, split, force) { }

        /// <summary>
        /// Indicates if this output stream can write binary data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this object can write binary data; otherwise, <see langword="false"/>.
        /// </value>
        public bool SupportsBinary { get { return true; } }

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <exception cref="ArgumentNullException">fileName</exception>
        /// <exception cref="ArgumentOutOfRangeException">inputFormat</exception>
        /// <remarks>
        /// Setting the input file name and the format can assist with knowing how to write binary data, and optionally
        /// set the name of the file that should be written.
        /// </remarks>
        public void SetInput(string fileName, InputFormat inputFormat)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!Enum.IsDefined(typeof(InputFormat), inputFormat))
                throw new ArgumentOutOfRangeException(nameof(inputFormat));
            if (inputFormat == InputFormat.Automatic)
                throw new ArgumentOutOfRangeException(nameof(inputFormat));

            if (inputFormat != InputFormat.File)
                throw new NotImplementedException();

            SetInput(fileName);
        }

        private readonly byte[] m_StorageHeader = new byte[16] {
            0x44, 0x4c, 0x54, 0x01,                            // DLT\1 header
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,    // Time stamp
            0x00, 0x00, 0x00, 0x00                             // ECU ID
        };

        private readonly byte[] m_Packet = new byte[65536];

        /// <summary>
        /// Writes the specified line to the output.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <returns>
        /// Returns <see langword="true"/> if after writing this line was written. A pure writer would not filter any
        /// lines. Is <see langword="false"/> if the line was filtered out.
        /// </returns>
        /// <remarks>
        /// This method will only convert lines of type <see cref="DltTraceLine"/>. Other line types are not written and
        /// will result in <see langword="false"/> being returned. When the line is written, the output is consolidated
        /// into a single argument and written as a UTF-8 string. The arguments are not individually encoded.
        /// </remarks>
        public bool Write(DltTraceLineBase line)
        {
            // We only write trace lines (not control lines).
            if (!(line is DltTraceLine traceLine)) return false;

            BuildStorageHeader(traceLine);
            int length = BuildPacket(traceLine);
            Write(line.TimeStamp, m_StorageHeader, m_Packet.AsSpan(0, length));
            return true;
        }

        /// <summary>
        /// Writes the specified line to the output.
        /// </summary>
        /// <param name="line">The line to write.</param>
        /// <param name="packet">The original packet data that generated the line to help write the output.</param>
        /// <returns>
        /// Returns <see langword="true"/> if after writing this line was written. A pure writer would not filter any
        /// lines. Is <see langword="false"/> if the line was filtered out.
        /// </returns>
        /// <remarks>The output knows of the input format through the method <see cref="SetInput"/>.</remarks>
        public bool Write(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            Write(line.TimeStamp, packet);
            return true;
        }

        private void BuildStorageHeader(DltTraceLineBase line)
        {
            if (line.Features.TimeStamp) {
                DateTimeOffset storageTime = line.TimeStamp.ToUniversalTime();
                long unixSeconds = storageTime.ToUnixTimeSeconds();
                long uSeconds = (storageTime.Ticks % TimeSpan.TicksPerSecond) / (TimeSpan.TicksPerMillisecond / 1000);
                BitOperations.Copy32ShiftLittleEndian(unixSeconds, m_StorageHeader.AsSpan(4));
                BitOperations.Copy32ShiftLittleEndian(uSeconds, m_StorageHeader.AsSpan(8));
            } else {
                BitOperations.Copy64ShiftLittleEndian(0, m_StorageHeader.AsSpan(4));
            }

            if (line.Features.EcuId) {
                BitOperations.Copy32ShiftLittleEndian(GetId(line.EcuId), m_StorageHeader.AsSpan(12));
            } else {
                BitOperations.Copy32ShiftLittleEndian(0, m_StorageHeader.AsSpan(12));
            }
        }

        private static int GetId(string id)
        {
            return (id[0] & 0xFF) |
                ((id[1] & 0xFF) << 8) |
                ((id[2] & 0xFF) << 16) |
                ((id[3] & 0xFF) << 24);
        }

        private int BuildPacket(DltTraceLine line)
        {
            // Standard Header
            byte htyp = 0x21;     // USH, LSB and Version 1
            short length = 4;

            if (line.Features.EcuId) {
                BitOperations.Copy32ShiftLittleEndian(GetId(line.EcuId), m_Packet.AsSpan(length));
                length += 4;
                htyp |= 0x04;
            }

            if (line.Features.SessionId) {
                BitOperations.Copy32ShiftLittleEndian(line.SessionId, m_Packet.AsSpan(length));
                length += 4;
                htyp |= 0x08;
            }

            if (line.Features.DeviceTimeStamp) {
                int timestampValue = unchecked((int)(line.DeviceTimeStamp.Ticks / (TimeSpan.TicksPerMillisecond / 10)));
                BitOperations.Copy32ShiftBigEndian(timestampValue, m_Packet.AsSpan(length));
                length += 4;
                htyp |= 0x10;
            }

            m_Packet[0] = htyp;
            m_Packet[1] = (byte)(line.Count & 0xFF);

            // Extended Header
            m_Packet[length] = (byte)(0x01 | ((int)line.Type & 0xFF));  // MSIN
            length++;

            m_Packet[length] = 1; // NOAR
            length++;

            if (line.Features.ApplicationId) {
                BitOperations.Copy32ShiftLittleEndian(GetId(line.ApplicationId), m_Packet.AsSpan(length));
            } else {
                BitOperations.Copy32ShiftLittleEndian(0, m_Packet.AsSpan(length));
            }
            length += 4;

            if (line.Features.ContextId) {
                BitOperations.Copy32ShiftLittleEndian(GetId(line.ContextId), m_Packet.AsSpan(length));
            } else {
                BitOperations.Copy32ShiftLittleEndian(0, m_Packet.AsSpan(length));
            }
            length += 4;

            // Verbose Argument
            BitOperations.Copy32ShiftLittleEndian(0x8200, m_Packet.AsSpan(length));  // UTF8 String
            length += 4;

            int strLen = System.Text.Encoding.UTF8.GetBytes(line.Text.AsSpan(), m_Packet.AsSpan(length + 2));
            BitOperations.Copy16ShiftLittleEndian(strLen + 1, m_Packet.AsSpan(length));
            m_Packet[length + 2 + strLen] = 0;
            length += 3;
            length += (short)strLen;

            BitOperations.Copy16ShiftBigEndian(length, m_Packet.AsSpan(2));
            return length;
        }
    }
}
