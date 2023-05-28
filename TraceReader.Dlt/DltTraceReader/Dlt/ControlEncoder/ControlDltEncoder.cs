namespace RJCP.Diagnostics.Log.Dlt.ControlEncoder
{
    using System;
    using ArgEncoder;
    using RJCP.Core;

    /// <summary>
    /// Encodes the control service in the line to a buffer using DLT v1 format.
    /// </summary>
    public class ControlDltEncoder : IDltEncoder<DltControlTraceLine>
    {
        private readonly IControlArgEncoder m_ControlArgEncoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDltEncoder"/> class.
        /// </summary>
        public ControlDltEncoder() : this(new ControlArgEncoder()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDltEncoder"/> class.
        /// </summary>
        /// <param name="controlArgEncoder">The encoder for writing control arguments.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="controlArgEncoder"/> is <see langword="null"/>.
        /// </exception>
        public ControlDltEncoder(IControlArgEncoder controlArgEncoder)
        {
            if (controlArgEncoder is null) throw new ArgumentNullException(nameof(controlArgEncoder));
            m_ControlArgEncoder = controlArgEncoder;
        }

        /// <summary>
        /// Encodes the line to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to encode to.</param>
        /// <param name="line">The line to serialize.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> is <see langword="null"/>.</exception>
        public Result<int> Encode(Span<byte> buffer, DltControlTraceLine line)
        {
            if (line is null) throw new ArgumentNullException(nameof(line));
            return m_ControlArgEncoder.Encode(buffer, line.Features.BigEndian, line.Service);
        }
    }
}
