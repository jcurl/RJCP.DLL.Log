namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System;
    using System.Text;

    /// <summary>
    /// Parse an unknown PDU.
    /// </summary>
    public class UnknownNonVerboseDltArg : UnknownDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownNonVerboseDltArg"/> class.
        /// </summary>
        /// <param name="buffer">The buffer of the unknown PDU.</param>
        public UnknownNonVerboseDltArg(ReadOnlySpan<byte> buffer)
            : base(buffer.ToArray()) { }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            // Data can't be null, as a ReadOnlySpan can never be null (value type), provided by the .ctor.
            if (Data.Length == 0) return "(00)";

            int strLength = 10 + Data.Length * 3;
            StringBuilder strBuilder = new(strLength);
            return Append(strBuilder).ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The original <paramref name="strBuilder"/> for chaining.</returns>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            // Data can't be null, as a ReadOnlySpan can never be null (value type), provided by the .ctor.
            if (Data.Length == 0) return strBuilder.Append("(00)");

            strBuilder.Append($"({Data.Length:x2}) ");
            HexConvert.ConvertToHex(strBuilder, Data);
            return strBuilder;
        }
    }
}
