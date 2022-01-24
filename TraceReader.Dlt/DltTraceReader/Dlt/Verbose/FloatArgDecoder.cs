﻿namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using System.Diagnostics;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Float argument decoder.
    /// </summary>
    /// <remarks>
    /// Shall decode float arguments stored either on 32-bit or 64-bit.
    /// <para>
    /// If a float of a different length is provided (16-bit or 128-bit) an <see cref="UnknownVerboseDltArg"/> instance
    /// shall be returned.
    /// </para>
    /// </remarks>
    public class FloatArgDecoder : IVerboseArgDecoder
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
        /// <returns>The length of the argument decoded, to allow advancing to the next argument.</returns>
        public int Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            arg = null;
            if ((typeInfo & DltConstants.TypeInfo.VariableInfo) != 0) {
                Log.Dlt.TraceEvent(TraceEventType.Information, "Float argument with unsupported type info of 0x{0:x}", typeInfo);
                return -1;
            }

            int argLength = typeInfo & DltConstants.TypeInfo.TypeLengthMask;
            switch (argLength) {
            case DltConstants.TypeInfo.TypeLength128bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 16) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Float argument with insufficient buffer length of {0}",
                        DltConstants.TypeInfo.TypeInfoSize + 16);
                    return -1;
                }
                arg = new UnknownVerboseDltArg(buffer[0..(DltConstants.TypeInfo.TypeInfoSize + 16)], msbf);
                return DltConstants.TypeInfo.TypeInfoSize + 16;
            case DltConstants.TypeInfo.TypeLength64bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 8) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Float argument with insufficient buffer length of {0}",
                        DltConstants.TypeInfo.TypeInfoSize + 8);
                    return -1;
                }
                arg = Decode64Bit(buffer[DltConstants.TypeInfo.TypeInfoSize..], msbf);
                return DltConstants.TypeInfo.TypeInfoSize + 8;
            case DltConstants.TypeInfo.TypeLength32bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 4) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Float argument with insufficient buffer length of {0}",
                        DltConstants.TypeInfo.TypeInfoSize + 4);
                    return -1;
                }
                arg = Decode32Bit(buffer[DltConstants.TypeInfo.TypeInfoSize..], msbf);
                return DltConstants.TypeInfo.TypeInfoSize + 4;
            case DltConstants.TypeInfo.TypeLength16bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 2) {
                    Log.Dlt.TraceEvent(TraceEventType.Warning, "Float argument with insufficient buffer length of {0}",
                        DltConstants.TypeInfo.TypeInfoSize + 2);
                    return -1;
                }
                arg = new UnknownVerboseDltArg(buffer[0..(DltConstants.TypeInfo.TypeInfoSize + 2)], msbf);
                return DltConstants.TypeInfo.TypeInfoSize + 2;
            default:
                Log.Dlt.TraceEvent(TraceEventType.Warning, "Float argument with unsupported type length of 0x{0:x}", argLength);
                return -1;
            }
        }

        private static IDltArg Decode64Bit(ReadOnlySpan<byte> buffer, bool msbf)
        {
            double data = BitOperations.To64FloatShift(buffer, !msbf);
            return new Float64DltArg(data);
        }

        private static IDltArg Decode32Bit(ReadOnlySpan<byte> buffer, bool msbf)
        {
            float data = BitOperations.To32FloatShift(buffer, !msbf);
            return new Float32DltArg(data);
        }
    }
}
