namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;

    /// <summary>
    /// Decode a verbose payload assuming this is a boolean.
    /// </summary>
    public class BoolArgDecoder : IVerboseArgDecoder
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
        /// <returns>
        /// The length of the argument decoded, to allow advancing to the next argument. In case the argument cannot be
        /// decoded, the argument is <see langword="null"/> and the result is -1.
        /// </returns>
        public int Decode(ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            int argLength = TypeInfo.GetTypeLength(buffer);
            if (argLength <= 0) {
                arg = null;
                return -1;
            }

            if (TypeInfo.IsVariSet(buffer)) {
                arg = null;
                return -1;
            }

            arg = new BoolDltArg(buffer[4] != 0);
            return DltConstants.TypeInfo.TypeInfoSize + argLength;
        }
    }
}
