namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// DLT string argument type.
    /// </summary>
    public class StringDltArg : DltArgBase<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDltArg"/> class with the given string value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        /// <paramref name="data"/>
        /// length is greater than 65518.
        /// <remarks>The value of the data string is not expected to include a termination char at the end.</remarks>
        public StringDltArg(string data) : this(data, StringEncodingType.Utf8) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringDltArg"/> class with the given string value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        /// <param name="coding">The type of coding associated with the argument. Used for encoding.</param>
        /// <paramref name="data"/>
        /// length is greater than 65518.
        /// <remarks>The value of the data string is not expected to include a termination char at the end.</remarks>
        public StringDltArg(string data, StringEncodingType coding) : base(data)
        {
            Coding = coding;
        }

        /// <summary>
        /// Gets the DLT coding used when decoded, equivalently the coding to use for encoding.
        /// </summary>
        /// <value>The DLT coding for the string.</value>
        public StringEncodingType Coding { get; private set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>If the underlying data is <see langword="null"/> an empty string shall be returned.</remarks>
        public override string ToString()
        {
            return Data ?? string.Empty;
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(Data);
        }
    }
}
