namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;
    using RJCP.Core;

    /// <summary>
    /// DLT type information that can be used to describe a verbose argument.
    /// </summary>
    /// <remarks>
    /// Describes an unknown argument type, with some fields interpreted as given by
    /// the Diagnostic Log and Trace AutoSAR Release 4.2.2 specification.
    /// </remarks>
    public sealed class DltTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltTypeInfo"/> class.
        /// </summary>
        /// <param name="buffer">The buffer where the first four bytes is the type information.</param>
        public DltTypeInfo(ReadOnlySpan<byte> buffer)
        {
            Bytes = new byte[4];
            buffer[0..4].CopyTo(Bytes);

            int typeInfo = BitOperations.To32ShiftLittleEndian(buffer);
            Length = typeInfo & DltConstants.TypeInfo.TypeLengthMask;
            ArgumentType = ((Bytes[0] & 0xF0) << 1) |
                    ((Bytes[1] & 0x07) << 2) |
                    ((Bytes[1] & 0x60) >> 5);
            IsVariableInfo = (typeInfo & DltConstants.TypeInfo.VariableInfo) != 0;
            IsFixedPoint = (typeInfo & DltConstants.TypeInfo.FixedPoint) != 0;
            Encoding = (typeInfo & DltConstants.TypeInfo.CodingMask) >> DltConstants.TypeInfo.CodingBitShift;
        }

        /// <summary>
        /// Gets the type information bytes.
        /// </summary>
        /// <value>
        /// The type information bytes.
        /// </value>
        /// <remarks>
        /// This forms the basis of the unknown argument, the raw data as interpreted from the stream of
        /// bytes. Classes deriving from this should ensure to set the <see cref="Bytes"/> property.
        /// </remarks>
        public byte[] Bytes { get; }

        /// <summary>
        /// Gets the encoded length of the type length.
        /// </summary>
        /// <value>
        /// The  encoded length of the type length.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        /// <value>
        /// The type of the argument.
        /// </value>
        /// <remarks>
        /// The value is a result of combining bits 4 to 10, 13 and 14 into a single value.
        /// </remarks>
        public int ArgumentType { get; }

        /// <summary>
        /// Gets a value indicating whether variable information bit is set.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if variable information bit is set, <see langword="false"/> otherwise.
        /// </value>
        public bool IsVariableInfo { get; }

        /// <summary>
        /// Gets a value indicating whether the fixed point bit is set.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if fixed point bit is set, <see langword="false"/> otherwise.
        /// </value>
        public bool IsFixedPoint { get; }

        /// <summary>
        /// Gets the encoding of the type.
        /// </summary>
        /// <value>
        /// The coding of the type.
        /// </value>
        public int Encoding { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder(Bytes.Length * 3);
            HexConvert.ConvertToHex(strBuilder, Bytes);
            return strBuilder.ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public StringBuilder Append(StringBuilder strBuilder)
        {
            HexConvert.ConvertToHex(strBuilder, Bytes);
            return strBuilder;
        }
    }
}
