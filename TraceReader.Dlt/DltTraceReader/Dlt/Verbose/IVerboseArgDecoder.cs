namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;

    /// <summary>
    /// Decode a DLT Verbose argument buffer.
    /// </summary>
    public interface IVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        int Decode(ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg);
    }
}
