namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// DLT signed integer argument type.
    /// </summary>
    /// <remarks>This argument type is only capable to hold a 64-bit long integer.</remarks>
    public class SignedIntDltArg : IntDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignedIntDltArg"/> class with the given signed integer value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        public SignedIntDltArg(long data) : base(data) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignedIntDltArg"/> class with the given signed integer value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        /// <param name="bytesLength">The size of the data. Use zero for best fit.</param>
        public SignedIntDltArg(long data, int bytesLength) : base(data, bytesLength) { }

        /// <summary>
        /// Gets the best fit size of the integer in bytes based on the value.
        /// </summary>
        /// <returns>The best fit size (in bytes) of the value.</returns>
        protected override int GetLength() { return GetLengthSigned(); }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString() { return Data.ToString(); }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The original <paramref name="strBuilder"/> for chaining.</returns>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(Data);
        }
    }
}
