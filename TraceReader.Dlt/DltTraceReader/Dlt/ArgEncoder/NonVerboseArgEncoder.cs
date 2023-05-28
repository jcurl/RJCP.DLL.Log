namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes an unknown DLT non-verbose argument payload.
    /// </summary>
    public sealed class NonVerboseArgEncoder : IArgEncoder
    {
        private static readonly RawArgEncoder RawEncoder = new RawArgEncoder();

        /// <summary>
        /// Encodes the DLT argument to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialise the argument to.</param>
        /// <param name="verbose">If the argument encoding should include the type information.</param>
        /// <param name="msbf">The endianness that was used when capturing the payload.</param>
        /// <param name="arg">The argument to serialise.</param>
        /// <returns>The amount of bytes serialised into the buffer.</returns>
        public Result<int> Encode(Span<byte> buffer, bool verbose, bool msbf, IDltArg arg)
        {
            if (verbose) return RawEncoder.Encode(buffer, true, msbf, arg);

            DltArgBase<byte[]> unknownArg = (DltArgBase<byte[]>)arg;
            // Write the payload directly as it is. We can't use the MSBF and assume the data matches the line.
            if (buffer.Length < unknownArg.Data.Length)
                return Result.FromException<int>(new DltEncodeException("'NonVerboseArgEncoder' insufficient buffer"));
            if (unknownArg.Data.Length > ushort.MaxValue)
                return Result.FromException<int>(new DltEncodeException("'NonVerboseArgEncoder' argument payload buffer too large"));

            unsafe {
                fixed (byte* srcPtr = unknownArg.Data)
                fixed (byte* dstPtr = buffer) {
                    Buffer.MemoryCopy(srcPtr, dstPtr, unknownArg.Data.Length, unknownArg.Data.Length);
                }
            }

            return unknownArg.Data.Length;
        }
    }
}
