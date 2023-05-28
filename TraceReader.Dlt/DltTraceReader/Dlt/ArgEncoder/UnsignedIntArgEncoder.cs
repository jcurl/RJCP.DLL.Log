﻿namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT unsigned integer types. The encoded length depends on the input.
    /// </summary>
    public sealed class UnsignedIntArgEncoder : IArgEncoder
    {
        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the argument to.</param>
        /// <param name="verbose">If the argument encoding should include the type information.</param>
        /// <param name="msbf">If <see langword="true" /> encode using big endian, else little endian.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        public Result<int> Encode(Span<byte> buffer, bool verbose, bool msbf, IDltArg arg)
        {
            int len = verbose ? 4 : 0;
            int typeInfo = DltConstants.TypeInfo.TypeUint;

            if (buffer.Length < len)
                return Result.FromException<int>(new DltEncodeException("'UnsignedIntArgEncoder' insufficient buffer"));

            Span<byte> payload = buffer[len..];
            UnsignedIntDltArg unsignedArg = (UnsignedIntDltArg)arg;
            if (payload.Length < unsignedArg.DataBytesLength)
                return Result.FromException<int>(new DltEncodeException("'UnsignedIntArgEncoder' insufficient buffer"));

            if (unsignedArg.DataBytesLength == 1) {
                typeInfo += DltConstants.TypeInfo.TypeLength8bit;
                payload[0] = unchecked((byte)unsignedArg.Data);
            } else if (unsignedArg.DataBytesLength == 2) {
                typeInfo += DltConstants.TypeInfo.TypeLength16bit;
                BitOperations.Copy16Shift(unsignedArg.Data, payload[0..2], !msbf);
            } else if (unsignedArg.DataBytesLength == 4) {
                typeInfo += DltConstants.TypeInfo.TypeLength32bit;
                BitOperations.Copy32Shift(unsignedArg.Data, payload[0..4], !msbf);
            } else if (unsignedArg.DataBytesLength == 8) {
                typeInfo += DltConstants.TypeInfo.TypeLength64bit;
                BitOperations.Copy64Shift(unsignedArg.Data, payload[0..8], !msbf);
            } else {
                return Result.FromException<int>(new DltEncodeException($"'UnsignedIntArgEncoder' unknown byte length {unsignedArg.DataBytesLength}"));
            }

            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + unsignedArg.DataBytesLength;
        }
    }
}
