namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;

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
        /// <returns>The amount of bytes serialised into the buffer, -1 in case of an error.</returns>
        public int Encode(Span<byte> buffer, bool verbose, bool msbf, IDltArg arg)
        {
            if (!verbose) return -1;                         // Can only encode verbose.

            UnknownVerboseDltArg unknownArg = (UnknownVerboseDltArg)arg;
            if (unknownArg.IsBigEndian != msbf) return -1;   // Cannot change endianness.

            if (buffer.Length < unknownArg.Data.Length + 4) return -1;
            if (unknownArg.Data.Length + 4 > ushort.MaxValue) return -1;

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
