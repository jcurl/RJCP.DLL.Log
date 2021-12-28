namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;

    internal static class TypeInfo
    {
        /// <summary>
        /// Gets the length of the type, given the encoded TYLE field.
        /// </summary>
        /// <param name="buffer">buffer where the verbose argument starts.</param>
        /// <returns>The number of bytes according to the type length. Returns -1 for unknown.</returns>
        public static int GetTypeLength(ReadOnlySpan<byte> buffer)
        {
            byte typeLength = (byte)(buffer[0] & DltConstants.TypeInfo.TypeLengthMask);
            switch (typeLength) {
            case DltConstants.TypeInfo.TypeLengthUnknown: return 0;
            case DltConstants.TypeInfo.TypeLength8bit: return 1;
            case DltConstants.TypeInfo.TypeLength16bit: return 2;
            case DltConstants.TypeInfo.TypeLength32bit: return 4;
            case DltConstants.TypeInfo.TypeLength64bit: return 8;
            case DltConstants.TypeInfo.TypeLength128bit: return 16;
            default: return -1;
            }
        }

        /// <summary>
        /// Determines whether the VARI bit is set.
        /// </summary>
        /// <param name="buffer">The buffer where the verbose argument starts.</param>
        /// <returns>Returns <see langword="true"/> if the VARI bit is set, otherwise <see langword="false"/>.</returns>
        public static bool IsVariSet(ReadOnlySpan<byte> buffer)
        {
            return (buffer[1] & (DltConstants.TypeInfo.VariableInfo >> 8)) != 0;
        }

        /// <summary>
        /// Gets the coding in the Type Information.
        /// </summary>
        /// <param name="buffer">The buffer where the verbose argument starts.</param>
        /// <returns>The coding bits 15-17.</returns>
        public static int GetCoding(ReadOnlySpan<byte> buffer)
        {
            // Looks complicated but grabs bits 15-17 of the first 4 bytes of buffer encoded as little endian. The '8'
            // is because the constants are for 32-bit, and to save CPU cycles, we do this for bytes 1 and 2.
            return
                (((buffer[2] << 8) | (buffer[1])) & (DltConstants.TypeInfo.CodingMask >> 8)) >>
                (DltConstants.TypeInfo.CodingBitShift - 8);
        }
    }
}
