namespace RJCP.Diagnostics.Log.Encoder
{
    using System;
    using Dlt;
    using Dlt.ArgEncoder;
    using Dlt.ControlEncoder;
    using RJCP.Core;

    /// <summary>
    /// Encodes DLT Trace Lines.
    /// </summary>
    /// <remarks>
    /// This class creates trace lines encoded to a buffer that contains a standard header, and optionally, an extended
    /// header. The output format is DLT v1.
    /// </remarks>
    public class DltTraceEncoder : ITraceEncoder<DltTraceLineBase>
    {
        private readonly IDltEncoder<DltTraceLine> m_DltArgsEncoder;
        private readonly IDltEncoder<DltControlTraceLine> m_ControlArgsEncoder;
        private int m_Count = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceEncoder"/> class.
        /// </summary>
        /// <remarks>
        /// Uses the default argument encoder.
        /// </remarks>
        public DltTraceEncoder() : this(new VerboseDltEncoder(), new ControlDltEncoder()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceEncoder"/> class.
        /// </summary>
        /// <param name="argsEncoder">The encoder that can iterate over the arguments to generate payloads.</param>
        /// <param name="controlEncoder">The encoder that can encode control service payloads.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="argsEncoder"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="controlEncoder"/> is <see langword="null"/>.
        /// </exception>
        public DltTraceEncoder(IDltEncoder<DltTraceLine> argsEncoder, IDltEncoder<DltControlTraceLine> controlEncoder)
        {
            ArgumentNullException.ThrowIfNull(argsEncoder);
            ArgumentNullException.ThrowIfNull(controlEncoder);
            m_DltArgsEncoder = argsEncoder;
            m_ControlArgsEncoder = controlEncoder;
        }

        /// <summary>
        /// Encodes the line to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to encode to.</param>
        /// <param name="line">The line to serialize.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <remarks>This encoder takes a trace line or a control line and always writes out a verbose line.</remarks>
        public virtual Result<int> Encode(Span<byte> buffer, DltTraceLineBase line)
        {
            ArgumentNullException.ThrowIfNull(line);

            int written;
            switch (line) {
            case DltTraceLine traceLine: {
                Result<int> result = WriteStandardHeader(buffer, traceLine);
                if (!result.TryGet(out written)) {
                    Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Information, result.Error.Message);
                    return result;
                }

                result = WriteExtendedHeader(buffer[written..], traceLine);
                if (!result.TryGet(out int length)) {
                    Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Information, result.Error.Message);
                    return result;
                }
                written += length;

                result = m_DltArgsEncoder.Encode(buffer[written..], traceLine);
                if (!result.TryGet(out length)) {
                    Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Information, result.Error.Message);
                    return result;
                }
                written += length;
                break;
            }
            case DltControlTraceLine controlLine: {
                Result<int> result = WriteStandardHeader(buffer, controlLine);
                if (!result.TryGet(out written)) {
                    Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Information, result.Error.Message);
                    return result;
                }

                result = WriteExtendedHeader(buffer[written..], controlLine);
                if (!result.TryGet(out int length)) {
                    Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Information, result.Error.Message);
                    return result;
                }
                written += length;

                result = m_ControlArgsEncoder.Encode(buffer[written..], controlLine);
                if (!result.TryGet(out length)) {
                    Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Information, result.Error.Message);
                    return result;
                }
                written += length;
                break;
            }
            default:
                Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Warning, "Unknown Line Type");
                return Result.FromException<int>(new DltEncodeException("Unknown Line Type"));
            }

            // Write the length.
            if (written > ushort.MaxValue) {
                Log.Encoder.TraceEvent(System.Diagnostics.TraceEventType.Warning, "Encoding exceeds maximum length");
                return Result.FromException<int>(new DltEncodeException("Encoding exceeds maximum length"));
            }
            BitOperations.Copy16ShiftBigEndian(written, buffer[2..4]);
            return written;
        }

        private Result<int> WriteStandardHeader(Span<byte> buffer, DltTraceLineBase line)
        {
            int len = 4;
            byte htyp = DltConstants.HeaderType.Version1;
            if (line.Features.EcuId) {
                len += 4;
                htyp += DltConstants.HeaderType.WithEcuId;
            }
            if (line.Features.SessionId) {
                len += 4;
                htyp += DltConstants.HeaderType.WithSessionId;
            }
            if (line.Features.DeviceTimeStamp) {
                len += 4;
                htyp += DltConstants.HeaderType.WithTimeStamp;
            }
            if (buffer.Length < len)
                return Result.FromException<int>(new DltEncodeException("Insufficient buffer encoding line"));

            if (line.Features.BigEndian) {
                htyp += DltConstants.HeaderType.MostSignificantByte;
            }

            // We're always writing a verbose payload, so we always need the extended header.
            htyp += DltConstants.HeaderType.UseExtendedHeader;

            m_Count = ((line.Count != -1) ?
                line.Count :
                m_Count + 1) & 0xFF;

            buffer[0] = htyp;
            buffer[1] = (byte)m_Count;
            buffer[2] = 0;
            buffer[3] = 0;
            int pos = 4;
            if (line.Features.EcuId) {
                WriteId(buffer[pos..], line.EcuId);
                pos += 4;
            }
            if (line.Features.SessionId) {
                BitOperations.Copy32ShiftBigEndian(line.SessionId, buffer[pos..]);
                pos += 4;
            }
            if (line.Features.DeviceTimeStamp) {
                BitOperations.Copy32ShiftBigEndian(GetDltTimeStamp(line.DeviceTimeStamp), buffer[pos..]);
                pos += 4;
            }
            return pos;
        }

        private static Result<int> WriteExtendedHeader(Span<byte> buffer, DltTraceLine line)
        {
            if (buffer.Length < 10)
                return Result.FromException<int>(new DltEncodeException("Insufficient buffer encoding line"));

            if (line.Arguments.Count > 255)                     // NOAR is 8-bit, so we can only write 255 arguments.
                return Result.FromException<int>(new DltEncodeException("Too many arguments (limit at 255)"));

            buffer[0] = (byte)((byte)line.Type | 1);      // Always writing verbose mode.
            buffer[1] = (byte)(line.Arguments.Count);
            WriteId(buffer[2..6], line.Features.ApplicationId ? line.ApplicationId : null);
            WriteId(buffer[6..10], line.Features.ContextId ? line.ContextId : null);
            return 10;
        }

        private static Result<int> WriteExtendedHeader(Span<byte> buffer, DltControlTraceLine line)
        {
            if (buffer.Length < 10)
                return Result.FromException<int>(new DltEncodeException("Insufficient buffer encoding line"));

            buffer[0] = (byte)line.Type;
            buffer[1] = 0;

            // Even though a DltControlTraceLine always defines these two features, we check again here for correctness.
            WriteId(buffer[2..6], line.Features.ApplicationId ? line.ApplicationId : null);
            WriteId(buffer[6..10], line.Features.ContextId ? line.ContextId : null);
            return 10;
        }

        /// <summary>
        /// Writes the identifier to the specified location as ASCII.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="id">The identifier to write.</param>
        public static void WriteId(Span<byte> buffer, string id)
        {
            int idLen = id is null ? 0 : id.Length;
            buffer[0] = idLen > 0 ? (byte)(id[0] & 0x7F) : (byte)0;   // Note, can never dereference null here as idLen will be zero.
            buffer[1] = idLen > 1 ? (byte)(id[1] & 0x7F) : (byte)0;
            buffer[2] = idLen > 2 ? (byte)(id[2] & 0x7F) : (byte)0;
            buffer[3] = idLen > 3 ? (byte)(id[3] & 0x7F) : (byte)0;
        }

        private static int GetDltTimeStamp(TimeSpan time)
        {
            return unchecked((int)(time.Ticks / TimeSpan.TicksPerMillisecond * 10));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// Is <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to
        /// release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            /* Nothing to dispose of */
        }
    }
}
