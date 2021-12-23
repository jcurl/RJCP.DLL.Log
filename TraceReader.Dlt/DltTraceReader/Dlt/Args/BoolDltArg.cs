namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// DLT boolean argument type.
    /// </summary>
    public class BoolDltArg : DltArgBase<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolDltArg"/> class with the given boolean value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        public BoolDltArg(bool data) : base(data) { }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Data ? "true" : "false";
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(ToString());
        }
    }
}
