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
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Data.ToString();
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
