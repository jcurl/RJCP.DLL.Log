namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT unsigned integer with hexadecimal representation.
    /// </summary>
    public sealed class HexIntArgEncoder : IArgEncoder
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
            int typeInfo = DltConstants.TypeInfo.TypeUint +
                ((int)IntegerEncodingType.Binary << DltConstants.TypeInfo.CodingBitShift);

            if (buffer.Length < len)
                return Result.FromException<int>(new DltEncodeException("'HexIntArgEncoder' insufficient buffer"));

            Span<byte> payload = buffer[len..];
            HexIntDltArg intArg = (HexIntDltArg)arg;
            if (payload.Length < intArg.DataBytesLength)
                return Result.FromException<int>(new DltEncodeException("'HexIntArgEncoder' insufficient buffer"));

            if (intArg.DataBytesLength == 1) {
                typeInfo += DltConstants.TypeInfo.TypeLength8bit;
                payload[0] = unchecked((byte)intArg.Data);
            } else if (intArg.DataBytesLength == 2) {
                typeInfo += DltConstants.TypeInfo.TypeLength16bit;
                BitOperations.Copy16Shift(intArg.Data, payload[0..2], !msbf);
            } else if (intArg.DataBytesLength == 4) {
                typeInfo += DltConstants.TypeInfo.TypeLength32bit;
                BitOperations.Copy32Shift(intArg.Data, payload[0..4], !msbf);
            } else if (intArg.DataBytesLength == 8) {
                typeInfo += DltConstants.TypeInfo.TypeLength64bit;
                BitOperations.Copy64Shift(intArg.Data, payload[0..8], !msbf);
            } else {
                return Result.FromException<int>(new DltEncodeException($"'HexIntArgEncoder' unknown byte length {intArg.DataBytesLength}"));
            }

            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + intArg.DataBytesLength;
        }
    }
}
