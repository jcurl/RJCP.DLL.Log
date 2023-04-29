namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;

    /// <summary>
    /// DLT unknown verbose argument type.
    /// </summary>
    public class UnknownVerboseDltArg : UnknownDltArg
    {
        private static byte[] GetArray(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 4) throw new ArgumentException("Buffer too small", nameof(buffer));
            return buffer[4..].ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownVerboseDltArg"/> class with the given byte array.
        /// </summary>
        /// <param name="buffer">
        /// The buffer containing the verbose payload. It should be limited to the atual size of the payload and the
        /// first 4 bytes should contain the type information.
        /// </param>
        /// <param name="msbf">Indicates the endianness when decoding. This is provided by the standard header.</param>
        public UnknownVerboseDltArg(ReadOnlySpan<byte> buffer, bool msbf) : base(GetArray(buffer))
        {
            TypeInfo = new DltTypeInfo(buffer);
            IsBigEndian = msbf;
        }

        /// <summary>
        /// Gets or sets the type information.
        /// </summary>
        /// <value>
        /// The type information.
        /// </value>
        /// <remarks>
        /// This is expected to be 4 bytes long, according to the Specification of
        /// Diagnostic Log and Trace AUTOSAR Release 4.2.2
        /// </remarks>
        public DltTypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is big endian.
        /// </summary>
        /// <value>Returns <see langword="true"/> if this instance is big endian; otherwise, <see langword="false"/>.</value>
        public bool IsBigEndian { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        /// <remarks>
        /// The output string is expected to contain the Type Info bytes and the actual
        /// argument bytes.
        /// <para>If the underlying argument data is <see langword="null"/> or an array of
        /// 0 bytes, an empty string shall be used.</para>
        /// </remarks>
        public override string ToString()
        {
            // Data can't be null, as a ReadOnlySpan can never be null (value type), provided by the .ctor.
            int strLength = 60 + Data.Length * 3 + TypeInfo.Bytes.Length * 3;

            return Append(new StringBuilder(strLength)).ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The reference to the original string builder.</returns>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            // Data can't be null, as a ReadOnlySpan can never be null (value type), provided by the .ctor.
            strBuilder.Append("Type Info: ");
            TypeInfo.Append(strBuilder).Append(" Data: ");

            if (Data.Length == 0) return strBuilder;
            HexConvert.ConvertToHex(strBuilder, Data);
            return strBuilder;
        }
    }
}
