namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;

    /// <summary>
    /// DLT unsigned integer argument type.
    /// </summary>
    /// <remarks>This argument type is only capable to hold a 64-bit long integer.</remarks>
    public class UnsignedIntDltArg : IntDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedIntDltArg"/> class with the given long integer value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        public UnsignedIntDltArg(long data) : base(data) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedIntDltArg"/> class with the given unsigned long value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <remarks>
        /// Store the 64-bit value in the range of 0..2^64-1. Reading the value via <b>Data</b> will return the 2's
        /// complement interpretation, interpreting the MSBit as the sign. Use the property <see cref="DataUnsigned"/>
        /// to get the original unsigned value stored with this constructor. Useful for languages that support unsigned
        /// types in addition to CLSCompliant(true).
        /// </remarks>
        [CLSCompliant(false)]
        public UnsignedIntDltArg(ulong data) : base(unchecked((long)data)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedIntDltArg"/> class.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        /// <param name="bytesLength">The size of the data. Use zero for best fit.</param>
        public UnsignedIntDltArg(long data, int bytesLength) : base(data, bytesLength) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsignedIntDltArg"/> class.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        /// <param name="bytesLength">The size of the data. Use zero for best fit.</param>
        [CLSCompliant(false)]
        public UnsignedIntDltArg(ulong data, int bytesLength) : base(unchecked((long)data), bytesLength) { }

        /// <summary>
        /// Gets the data as a non CLS-Compliant unsigned long.
        /// </summary>
        /// <value>The data as a non CLS-Compliant unsigned long.</value>
        /// <remarks>
        /// Return the unsigned 64-bit value that is represented by this class in the range of 0 to <b>ulong.Max</b>.
        /// </remarks>
        [CLSCompliant(false)]
        public ulong DataUnsigned
        {
            get { return unchecked((ulong)Data); }
        }

        /// <summary>
        /// Gets the best fit size based on the value.
        /// </summary>
        /// <returns>The best fit size (in bytes) of the value.</returns>
        protected override int GetLength() { return GetLengthUnsigned(); }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>
        /// The string value shall represent the unsigned long numeric equivalent, regardless if a signed long value was
        /// provided when creating the <see cref="UnsignedIntDltArg"/> instance.
        /// </remarks>
        public override string ToString()
        {
            return unchecked((ulong)Data).ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(unchecked((ulong)Data));
        }
    }
}
