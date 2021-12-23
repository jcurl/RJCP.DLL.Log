namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
    using RJCP.Core.Text;

    /// <summary>
    /// DLT floating point argument type stored on 32 bits.
    /// </summary>
    public class Float32DltArg : DltArgBase<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Float32DltArg"/> class with the given float value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        public Float32DltArg(float data) : base(data) { }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (float.IsPositiveInfinity(Data))
                return "inf";

            if (float.IsNegativeInfinity(Data))
                return "-inf";

            if (float.IsNaN(Data))
                return "nan";

            // Use an output that is compatible with C
            return StringUtilities.SPrintF("%g", Data);
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            if (float.IsPositiveInfinity(Data))
                return strBuilder.Append("inf");

            if (float.IsNegativeInfinity(Data))
                return strBuilder.Append("-inf");

            if (float.IsNaN(Data))
                return strBuilder.Append("nan");

            // Use an output that is compatible with C
            return strBuilder.Append(StringUtilities.SPrintF("%g", Data));
        }
    }
}
