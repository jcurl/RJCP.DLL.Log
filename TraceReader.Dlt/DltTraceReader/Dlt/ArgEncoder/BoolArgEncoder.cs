namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT boolean type.
    /// </summary>
    public sealed class BoolArgEncoder : IArgEncoder
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
            const int typeInfo = DltConstants.TypeInfo.TypeLength8bit + DltConstants.TypeInfo.TypeBool;

            int len = verbose ? 4 : 0;
            if (buffer.Length < len + 1)
                return Result.FromException<int>(new DltEncodeException("'BoolArgEncoder' insufficient buffer"));

            buffer[len] = ((BoolDltArg)arg).Data ? (byte)1 : (byte)0;
            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + 1;
        }
    }
}
