namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Serializes a DLT signed integer types. The encoded length depends on the input.
    /// </summary>
    public sealed class SignedIntArgEncoder : IArgEncoder
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
            int len = verbose ? 4 : 0;
            int typeInfo = DltConstants.TypeInfo.TypeSint;

            if (buffer.Length < len) return -1;
            Span<byte> payload = buffer[len..];

            SignedIntDltArg signedArg = (SignedIntDltArg)arg;
            if (payload.Length < signedArg.DataBytesLength) return -1;

            if (signedArg.DataBytesLength == 1) {
                typeInfo += DltConstants.TypeInfo.TypeLength8bit;
                payload[0] = unchecked((byte)signedArg.Data);
            } else if (signedArg.DataBytesLength == 2) {
                typeInfo += DltConstants.TypeInfo.TypeLength16bit;
                BitOperations.Copy16Shift(signedArg.Data, payload[0..2], !msbf);
            } else if (signedArg.DataBytesLength == 4) {
                typeInfo += DltConstants.TypeInfo.TypeLength32bit;
                BitOperations.Copy32Shift(signedArg.Data, payload[0..4], !msbf);
            } else if (signedArg.DataBytesLength == 8) {
                typeInfo += DltConstants.TypeInfo.TypeLength64bit;
                BitOperations.Copy64Shift(signedArg.Data, payload[0..8], !msbf);
            } else {
                return -1;
            }

            if (verbose) BitOperations.Copy32Shift(typeInfo, buffer, !msbf);
            return len + signedArg.DataBytesLength;
        }
    }
}
