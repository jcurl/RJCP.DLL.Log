namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a packet based log decoder.
    /// </summary>
    /// <typeparam name="T">The type of trace line the decoder produces.</typeparam>
    public interface ITraceDecoder<T> : IDisposable where T : ITraceLine
    {
        /// <summary>
        /// Decodes data from the buffer and returns a read only collection of trace lines.
        /// </summary>
        /// <param name="buffer">The buffer data that should be decoded.</param>
        /// <param name="position">The position in the stream where the data begins.</param>
        /// <returns>An enumerable collection of the decoded lines.</returns>
        /// <remarks>
        /// The <see cref="Decode(ReadOnlySpan{byte}, long)"/> method shall accept any number of bytes for decoding. It should
        /// also consume all data that is received, so that data which is not processed is buffered locally by the
        /// decoder.
        /// <para>
        /// On return, this method should return a read only collection of trace lines that were fully decoded. If no
        /// lines were decoded, it should return an empty collection (and avoid <see langword="null"/>).
        /// </para>
        /// </remarks>
        IEnumerable<T> Decode(ReadOnlySpan<byte> buffer, long position);

        /// <summary>
        /// Flushes any data that is locally cached, and returns any pending trace lines.
        /// </summary>
        /// <returns>A read only collection of the decoded lines.</returns>
        /// <remarks>
        /// Flushing a decoder typically happens by the trace reader when the stream is finished, so that any remaining
        /// data the decoder may have can be returned to the user application (including error trace lines).
        /// </remarks>
        IEnumerable<T> Flush();
    }
}
