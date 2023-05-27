namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decode a verbose payload that contains raw data.
    /// </summary>
    public sealed class RawArgDecoder : IVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="typeInfo">The decoded Type Info for the specific argument decoded correctly.</param>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian. The length of the string is expected to be coding with this endianness.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public Result<int> Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            const int DataOffset = DltConstants.TypeInfo.TypeInfoSize + 2;

            if ((typeInfo & DltConstants.TypeInfo.VariableInfo) != 0) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException("'Raw' unsupported type info"));
            }

            if (buffer.Length < DataOffset) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"'Raw' insufficient buffer length {buffer.Length}"));
            }

            ushort payloadLength = unchecked((ushort)BitOperations.To16Shift(
                buffer[DltConstants.TypeInfo.TypeInfoSize..DataOffset], !msbf));

            byte[] data;
            if (payloadLength == 0) {
                data = Array.Empty<byte>();
            } else {
                if (buffer.Length < DataOffset + payloadLength) {
                    arg = null;
                    return Result.FromException<int>(new DltDecodeException($"'Raw' insufficient buffer length {buffer.Length}"));
                }
                data = buffer[DataOffset..(DataOffset + payloadLength)].ToArray();
            }
            arg = new RawDltArg(data);
            return DataOffset + payloadLength;
        }
    }
}
