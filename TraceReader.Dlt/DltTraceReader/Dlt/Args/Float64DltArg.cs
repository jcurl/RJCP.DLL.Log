namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;
    using RJCP.Core.Text;

    /// <summary>
    /// DLT floating point argument type stored on 64 bits.
    /// </summary>
    public class Float64DltArg : DltArgBase<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Float64DltArg"/> class with the given double value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        public Float64DltArg(double data) : base(data) { }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (double.IsPositiveInfinity(Data))
                return "inf";

            if (double.IsNegativeInfinity(Data))
                return "-inf";

            if (double.IsNaN(Data))
                return "nan";

            // Use an output that is compatible with C
            return StringUtilities.SPrintF("%g", Data);
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The original <paramref name="strBuilder"/> for chaining.</returns>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            if (double.IsPositiveInfinity(Data))
                return strBuilder.Append("inf");

            if (double.IsNegativeInfinity(Data))
                return strBuilder.Append("-inf");

            if (double.IsNaN(Data))
                return strBuilder.Append("nan");

            // Use an output that is compatible with C
            return strBuilder.Append(StringUtilities.SPrintF("%g", Data));
        }
    }
}
