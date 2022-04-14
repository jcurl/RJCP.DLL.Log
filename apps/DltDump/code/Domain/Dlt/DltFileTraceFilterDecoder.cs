namespace RJCP.App.DltDump.Domain.Dlt
{
    using System;
    using Resources;
    using RJCP.Diagnostics.Log.Decoder;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// A decoder that writes binary data to an <see cref="IOutputStream"/> supporting Binary.
    /// </summary>
    public sealed class DltFileTraceFilterDecoder : DltFileTraceDecoder
    {
        private readonly IOutputStream m_OutputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltFileTraceFilterDecoder"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputStream"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="outputStream"/> doesn't support binary mode.</exception>
        public DltFileTraceFilterDecoder(IOutputStream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            if (!outputStream.SupportsBinary)
                throw new ArgumentException(AppResources.DomainDecoderInvalidOutputStream, nameof(outputStream));

            m_OutputStream = outputStream;
        }

        /// <summary>
        /// Checks the line before adding to the list of data that can be parsed.
        /// </summary>
        /// <param name="line">The line that should be checked.</param>
        /// <param name="packet">The raw packet data.</param>
        /// <returns>Returns <see langword="true" /> if the line should be added, <see langword="false" /> otherwise.</returns>
        protected override bool CheckLine(DltTraceLineBase line, ReadOnlySpan<byte> packet)
        {
            m_OutputStream.Write(line, packet);
            return false;
        }
    }
}
