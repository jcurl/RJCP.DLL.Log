namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using System;
    using Args;
    using RJCP.Core;

    /// <summary>
    /// Base class for integer decoder classes.
    /// </summary>
    public abstract class IntegerArgDecoder : VerboseArgDecoderBase
    {
        /// <summary>
        /// Creates the 64-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected abstract IDltArg Create64BitArgument(long data, IntegerEncodingType coding);

        /// <summary>
        /// Creates the 32-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected abstract IDltArg Create32BitArgument(int data, IntegerEncodingType coding);

        /// <summary>
        /// Creates the 16-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected abstract IDltArg Create16BitArgument(short data, IntegerEncodingType coding);

        /// <summary>
        /// Creates the 8-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected abstract IDltArg Create8BitArgument(byte data, IntegerEncodingType coding);

        /// <summary>
        /// Decodes a verbose DLT message payload signed integer argument.
        /// </summary>
        /// <param name="typeInfo">The decoded Type Info for the specific argument decoded correctly.</param>
        /// <param name="buffer">The buffer where the argument to be decoded can be found.</param>
        /// <param name="msbf">
        /// Sets the endianness, if <see langword="false"/> then little endian, else if <see langword="true"/> sets big
        /// endian.
        /// </param>
        /// <param name="arg">On output, the decoded argument.</param>
        /// <returns>
        /// The number of decoded bytes, which represent the encoded argument, or -1 if decoding fails.
        /// </returns>
        /// <remarks>This decoder cannot decode fields with the VARI bit or FIXP bit set.</remarks>
        public override int Decode(int typeInfo, ReadOnlySpan<byte> buffer, bool msbf, out IDltArg arg)
        {
            if ((typeInfo & (DltConstants.TypeInfo.VariableInfo | DltConstants.TypeInfo.FixedPoint)) != 0)
                return DecodeError("'Integer' unsupported type info", out arg);

            IntegerEncodingType coding =
                (IntegerEncodingType)((typeInfo & DltConstants.TypeInfo.CodingMask) >> DltConstants.TypeInfo.CodingBitShift);
            int argLength = typeInfo & DltConstants.TypeInfo.TypeLengthMask;
            switch (argLength) {
            case DltConstants.TypeInfo.TypeLength128bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 16)
                    return DecodeError("'Integer' insufficient buffer length {0}", buffer.Length, out arg);
                arg = new UnknownVerboseDltArg(buffer[0..(DltConstants.TypeInfo.TypeInfoSize + 16)], msbf);
                return DltConstants.TypeInfo.TypeInfoSize + 16;
            case DltConstants.TypeInfo.TypeLength64bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 8)
                    return DecodeError("'Integer' insufficient buffer length {0}", buffer.Length, out arg);
                arg = Decode64Bit(buffer[DltConstants.TypeInfo.TypeInfoSize..], msbf, coding);
                return DltConstants.TypeInfo.TypeInfoSize + 8;
            case DltConstants.TypeInfo.TypeLength32bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 4)
                    return DecodeError("'Integer' insufficient buffer length {0}", buffer.Length, out arg);
                arg = Decode32Bit(buffer[DltConstants.TypeInfo.TypeInfoSize..], msbf, coding);
                return DltConstants.TypeInfo.TypeInfoSize + 4;
            case DltConstants.TypeInfo.TypeLength16bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 2)
                    return DecodeError("'Integer' insufficient buffer length {0}", buffer.Length, out arg);
                arg = Decode16Bit(buffer[DltConstants.TypeInfo.TypeInfoSize..], msbf, coding);
                return DltConstants.TypeInfo.TypeInfoSize + 2;
            case DltConstants.TypeInfo.TypeLength8bit:
                if (buffer.Length < DltConstants.TypeInfo.TypeInfoSize + 1)
                    return DecodeError("'Integer' insufficient buffer length {0}", buffer.Length, out arg);
                arg = Create8BitArgument(buffer[DltConstants.TypeInfo.TypeInfoSize], coding);
                return DltConstants.TypeInfo.TypeInfoSize + 1;
            default:
                return DecodeError("'Integer' unsupported type length 0x{0:x}", argLength, out arg);
            }
        }

        private IDltArg Decode64Bit(ReadOnlySpan<byte> buffer, bool msbf, IntegerEncodingType coding)
        {
            long data = BitOperations.To64Shift(buffer, !msbf);
            return Create64BitArgument(data, coding);
        }

        private IDltArg Decode32Bit(ReadOnlySpan<byte> buffer, bool msbf, IntegerEncodingType coding)
        {
            int data = BitOperations.To32Shift(buffer, !msbf);
            return Create32BitArgument(data, coding);
        }

        private IDltArg Decode16Bit(ReadOnlySpan<byte> buffer, bool msbf, IntegerEncodingType coding)
        {
            short data = BitOperations.To16Shift(buffer, !msbf);
            return Create16BitArgument(data, coding);
        }
    }
}
