namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;

    /// <summary>
    /// DLT unknown verbose argument type.
    /// </summary>
    public class UnknownVerboseDltArg : UnknownDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownVerboseDltArg"/> class with the given byte array.
        /// </summary>
        /// <param name="buffer">
        /// The buffer containing the verbose payload. It should be limited to the atual size of the payload and the
        /// first 4 bytes should contain the type information.
        /// </param>
        /// <param name="msbf">Indicates the endianness when decoding. This is provided by the standard header.</param>
        public UnknownVerboseDltArg(ReadOnlySpan<byte> buffer, bool msbf) : base(buffer[4..].ToArray())
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
            int strLength = 60;
            if (Data != null) strLength += Data.Length * 3;
            if (TypeInfo.Bytes != null) strLength += TypeInfo.Bytes.Length * 3;

            StringBuilder strBuilder = new StringBuilder(strLength);
            return Append(strBuilder).ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            strBuilder.Append("Type Info: ");
            TypeInfo.Append(strBuilder).Append(" Data: ");

            if (Data == null || Data.Length == 0) return strBuilder;
            HexConvert.ConvertToHex(strBuilder, Data);
            return strBuilder;
        }
    }
}
