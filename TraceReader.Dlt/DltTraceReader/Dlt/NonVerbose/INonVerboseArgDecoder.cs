namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System;
    using Args;

    /// <summary>
    /// The main decoder interface for decoding DLT non verbose argument payloads.
    /// </summary>
    public interface INonVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT non verbose argument.
        /// </summary>
        /// <param name="buffer">The buffer where the encoded DLT non verbose argument can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="pdu">The Packet Data Unit instance representing the argument structure.</param>
        /// <param name="arg">On output, the decoded argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        int Decode(ReadOnlySpan<byte> buffer, bool msbf, IPdu pdu, out IDltArg arg);
    }
}
