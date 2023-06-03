namespace RJCP.App.DltDump.Domain.OutputStream
{
    using System;
    using Domain.Dlt;
    using RJCP.Core;
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Encoder;

    /// <summary>
    /// An output module that can write binary data as it is received.
    /// </summary>
    public sealed class DltOutput : OutputBase, IOutputStream
    {
        private readonly object m_WriteLock = new object();
        private ITraceEncoder<DltTraceLineBase> m_TraceEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName) : base(fileName)
        {
            InitializeTraceEncoder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName, bool force) : base(fileName, 0, force)
        {
            InitializeTraceEncoder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltOutput"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="split">The number of bytes to write before splitting.</param>
        /// <param name="force">Force overwrite the file if <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
        public DltOutput(string fileName, long split, bool force) : base(fileName, split, force)
        {
            InitializeTraceEncoder();
        }

        private void InitializeTraceEncoder()
        {
            ITraceEncoderFactory<DltTraceLineBase> factory = new DltFileTraceEncoderFactory();
            m_TraceEncoder = factory.Create();
        }

        /// <summary>
        /// Indicates if this output stream can write binary data.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if this object can write binary data; otherwise, <see langword="false"/>.
        /// </value>
        public bool SupportsBinary { get { return true; } }

        private InputFormat m_InputFormat = InputFormat.File;

        /// <summary>
        /// Defines the input file name and the format of the input file.
        /// </summary>
        /// <param name="fileName">Name of the input file.</param>
        /// <param name="inputFormat">The input format.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputFormat"/> is not defined, or is <see cref="InputFormat.Automatic"/>.
        /// </exception>
        /// <remarks>
        /// Setting the input file name and the format can assist with knowing how to write binary data, and optionally
        /// set the name of the file that should be written.
        /// </remarks>
        public void SetInput(string fileName, InputFormat inputFormat)
        {
            if (!Enum.IsDefined(typeof(InputFormat), inputFormat))
                throw new ArgumentOutOfRangeException(nameof(inputFormat));
            if (inputFormat == InputFormat.Automatic)
                throw new ArgumentOutOfRangeException(nameof(inputFormat));

            m_InputFormat = inputFormat;
            SetInput(fileName);
        }

        private readonly byte[] m_StorageHeader = new byte[16] {
            0x44, 0x4c, 0x54, 0x01,                            // DLT\1 header
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,    // Time stamp
            0x00, 0x00, 0x00, 0x00                             // ECU ID
        };

        // Amount of data that can be written in one packet (storage header + packet)
        private readonly byte[] m_Packet = new byte[65536 + 16];

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
            lock (m_WriteLock) {
                Result<int> result = m_TraceEncoder.Encode(m_Packet, line);
                if (!result.TryGet(out int length)) return false;

                Write(line.TimeStamp, m_Packet.AsSpan(0, length));
                return true;
            }
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
            ReadOnlySpan<byte> payload;

            switch (m_InputFormat) {
            case InputFormat.File:
            case InputFormat.Network:
            case InputFormat.Pcap:
                payload = packet;
                break;
            case InputFormat.Serial:
                payload = packet[4..];
                break;
            default:
                return false;
            }

            lock (m_WriteLock) {
                switch (m_InputFormat) {
                case InputFormat.File:
                    Write(line.TimeStamp, payload);
                    return true;
                default:
                    Result<int> result = DltFileTraceEncoder.WriteStorageHeader(m_StorageHeader, line);
                    if (!result.HasValue) return false;
                    Write(line.TimeStamp, m_StorageHeader, payload);
                    return true;
                }
            }
        }
    }
}
