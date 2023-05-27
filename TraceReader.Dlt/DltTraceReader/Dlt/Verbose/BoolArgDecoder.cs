namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Decode a verbose payload assuming this is a boolean.
    /// </summary>
    public sealed class BoolArgDecoder : IVerboseArgDecoder
    {
        /// <summary>
        /// Decodes the DLT verbose argument given in the current buffer.
        /// </summary>
        /// <param name="typeInfo">The decoded Type Info for the specific argument decoded correctly.</param>
        /// <param name="buffer">The buffer that starts with the Type Info.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="arg">On return, contains the DLT argument.</param>
        /// <returns>
        /// The length of the argument decoded, to allow advancing to the next argument.</returns>
        public Result<int> Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            if ((typeInfo & DltConstants.TypeInfo.VariableInfo) != 0) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException("'Bool' unsupported type info"));
            }

            int argLength;
            int typeLength = typeInfo & DltConstants.TypeInfo.TypeLengthMask;
            switch (typeLength) {
            case DltConstants.TypeInfo.TypeLengthUnknown:
            case DltConstants.TypeInfo.TypeLength8bit:
                argLength = 1;
                break;
            case DltConstants.TypeInfo.TypeLength16bit:
                argLength = 2;
                break;
            case DltConstants.TypeInfo.TypeLength32bit:
                argLength = 4;
                break;
            case DltConstants.TypeInfo.TypeLength64bit:
                argLength = 8;
                break;
            case DltConstants.TypeInfo.TypeLength128bit:
                argLength = 16;
                break;
            default:
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"'Bool' unsupported type length 0x{typeLength:x}"));
            }

            if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + argLength) {
                arg = null;
                return Result.FromException<int>(new DltDecodeException($"'Bool' insufficient buffer length {buffer.Length}"));
            }

            bool boolArg = false;
            for (int i = 0; i < argLength && !boolArg; i++) {
                boolArg = buffer[DltConstants.TypeInfo.TypeInfoSize + i] != 0;
            }

            arg = new BoolDltArg(boolArg);
            return DltConstants.TypeInfo.TypeInfoSize + argLength;
        }
    }
}
