namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT float types.
    /// </summary>
    public sealed class Float32ArgEncoder : IArgEncoder
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
            const int typeInfo = DltConstants.TypeInfo.TypeFloat + DltConstants.TypeInfo.TypeLength32bit;

            int len = verbose ? 4 : 0;
            if (buffer.Length < len + 4) return -1;
            Span<byte> payload = buffer[len..];

            Float32DltArg floatArg = (Float32DltArg)arg;
            BitOperations.Copy32FloatShift(floatArg.Data, payload[0..4], !msbf);

            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + 4;
        }
    }
}
