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
            if (Data == null || Data.Length == 0) return "(00)";

            int strLength = 10;
            if (Data != null) strLength += Data.Length * 3;
            StringBuilder strBuilder = new StringBuilder(strLength);
            return Append(strBuilder).ToString();
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        /// <returns>The reference to the original string builder.</returns>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            if (Data == null || Data.Length == 0) {
                strBuilder.Append("(00)");
                return strBuilder;
            }
            strBuilder.Append($"({Data.Length:x2}) ");
            HexConvert.ConvertToHex(strBuilder, Data);
            return strBuilder;
        }
    }
}
