namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// DLT raw data argument type.
    /// </summary>
    public class RawDltArg : DltArgBase<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawDltArg"/> class with the given byte array.
        /// </summary>
        /// <param name="data">The value of the raw data bytes.</param>
        public RawDltArg(byte[] data) : base(data) { }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>
        /// If the underlying data is <see langword="null"/> or an array of 0 bytes, an empty string shall be returned.
        /// </remarks>
        public override string ToString()
        {
            if (Data == null || Data.Length == 0) return string.Empty;

            StringBuilder strBuilder = new StringBuilder(Data.Length * 3);
            HexConvert.ConvertToHex(strBuilder, Data);
            return strBuilder.ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            if (Data == null || Data.Length == 0) return strBuilder;

            HexConvert.ConvertToHex(strBuilder, Data);
            return strBuilder;
        }
    }
}
