namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes an unknown DLT verbose argument.
    /// </summary>
    public sealed class UnknownVerboseArgEncoder : IArgEncoder
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
            if (!verbose)                                    // Can only encode verbose.
                return Result.FromException<int>(new DltEncodeException("'UnknownVerboseArgEncoder' only verbose encoding supported"));

            UnknownVerboseDltArg unknownArg = (UnknownVerboseDltArg)arg;
            if (unknownArg.IsBigEndian != msbf)              // Cannot change endianness.
                return Result.FromException<int>(new DltEncodeException("'UnknownVerboseArgEncoder' endianness mismatch"));

            if (buffer.Length < unknownArg.Data.Length + 4)
                return Result.FromException<int>(new DltEncodeException("'UnknownVerboseArgEncoder' insufficient buffer"));
            if (unknownArg.Data.Length + 4 > ushort.MaxValue)
                return Result.FromException<int>(new DltEncodeException("'UnknownVerboseArgEncoder' argument payload buffer too large"));
            unsafe {
                fixed (byte* srcPtr = unknownArg.TypeInfo.Bytes)
                fixed (byte* dstPtr = buffer) {
                    Buffer.MemoryCopy(srcPtr, dstPtr, 4, 4);
                }

                fixed (byte* srcPtr = unknownArg.Data)
                fixed (byte* dstPtr = buffer[4..]) {
                    Buffer.MemoryCopy(srcPtr, dstPtr, unknownArg.Data.Length, unknownArg.Data.Length);
                }
            }

            return 4 + unknownArg.Data.Length;
        }
    }
}
