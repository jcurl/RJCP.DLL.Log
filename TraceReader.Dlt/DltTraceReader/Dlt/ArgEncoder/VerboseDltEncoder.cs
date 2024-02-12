namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Encodes all arguments in the line given to a buffer using DLT Verbose format.
    /// </summary>
    public class VerboseDltEncoder : IDltEncoder<DltTraceLine>
    {
        private readonly IArgEncoder m_VerboseArgEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseDltEncoder"/> class.
        /// </summary>
        public VerboseDltEncoder() : this(new DltArgEncoder()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseDltEncoder"/> class.
        /// </summary>
        /// <param name="lineEncoder">The encoder for writing lines in an argument.</param>
        /// <exception cref="ArgumentNullException"><paramref name="lineEncoder"/> is <see langword="null"/>.</exception>
        public VerboseDltEncoder(IArgEncoder lineEncoder)
        {
            ArgumentNullException.ThrowIfNull(lineEncoder);
            m_VerboseArgEncoder = lineEncoder;
        }

        /// <summary>
        /// Encodes the line to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to encode to.</param>
        /// <param name="line">The line to serialize.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> is <see langword="null"/>.</exception>
        public Result<int> Encode(Span<byte> buffer, DltTraceLine line)
        {
            ArgumentNullException.ThrowIfNull(line);

            int written = 0;
            foreach (IDltArg arg in line.Arguments) {
                Result<int> result = m_VerboseArgEncoder.Encode(buffer, true, line.Features.BigEndian, arg);
                if (!result.TryGet(out int argWrite)) return result;
                written += argWrite;
                buffer = buffer[argWrite..];
            }
            return written;
        }
    }
}
