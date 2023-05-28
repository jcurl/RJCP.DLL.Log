namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT raw buffer.
    /// </summary>
    public sealed class RawArgEncoder : IArgEncoder
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
            const int typeInfo = DltConstants.TypeInfo.TypeRaw;

            DltArgBase<byte[]> rawArg = (DltArgBase<byte[]>)arg;

            int len = verbose ? 4 : 0;
            if (buffer.Length < len + 2 + rawArg.Data.Length)
                return Result.FromException<int>(new DltEncodeException("'RawArgEncoder' insufficient buffer"));
            if (len + 2 + rawArg.Data.Length > ushort.MaxValue)
                return Result.FromException<int>(new DltEncodeException("'RawArgEncoder' argument payload buffer too large"));

            Span<byte> payload = buffer[len..];
            BitOperations.Copy16Shift(rawArg.Data.Length, payload[0..], !msbf);
            if (rawArg.Data.Length > 0) {
                unsafe {
                    fixed (byte* srcPtr = rawArg.Data)
                    fixed (byte* dstPtr = payload[2..]) {
                        Buffer.MemoryCopy(srcPtr, dstPtr, rawArg.Data.Length, rawArg.Data.Length);
                    }
                }
            }

            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + 2 + rawArg.Data.Length;
        }
    }
}
