namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Diagnostics;
    using Args;

    /// <summary>
    /// Decode a verbose payload assuming this is a boolean.
    /// </summary>
    public class BoolArgDecoder : IVerboseArgDecoder
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
        /// The length of the argument decoded, to allow advancing to the next argument. In case the argument cannot be
        /// decoded, the argument is <see langword="null"/> and the result is -1.
        /// </returns>
        public int Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            arg = null;
            if ((typeInfo & DltConstants.TypeInfo.VariableInfo) != 0) {
                Log.Dlt.TraceEvent(TraceEventType.Information, "Bool argument with unsupported type info of 0x{0:x}", typeInfo);
                return -1;
            }

            int argLength;
            int typeLength = typeInfo & DltConstants.TypeInfo.TypeLengthMask;
            switch (typeLength) {
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
                Log.Dlt.TraceEvent(TraceEventType.Warning, "Bool argument with unsupported type length of 0x{0:x}", typeLength);
                return -1;
            }

            if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + argLength) {
                Log.Dlt.TraceEvent(TraceEventType.Warning, "Bool argument with insufficient buffer length of {0}",
                    DltConstants.TypeInfo.TypeInfoSize + argLength);
                return -1;
            }

            arg = new BoolDltArg(buffer[4] != 0);
            return DltConstants.TypeInfo.TypeInfoSize + argLength;
        }
    }
}
