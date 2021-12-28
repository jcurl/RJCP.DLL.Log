namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decode a verbose payload that contains raw data.
    /// </summary>
    public class RawArgDecoder : IVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian. The length of the string is expected to be coding with this endianness.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public int Decode(ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            if (TypeInfo.IsVariSet(buffer)) {
                arg = null;
                return -1;
            }

            ushort payloadLength = unchecked((ushort)BitOperations.To16Shift(buffer[4..6], !msbf));

            byte[] data;
            if (payloadLength == 0) {
                data = Array.Empty<byte>();
            } else {
                data = buffer[6..(6 + payloadLength)].ToArray();
            }
            arg = new RawDltArg(data);
            return DltConstants.TypeInfo.TypeInfoSize + 2 + payloadLength;
        }
    }
}
