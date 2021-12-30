namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// DLT integer argument type in hexadecimal format.
    /// </summary>
    /// <remarks>This argument type is only capable to hold a 64-bit long integer.</remarks>
    public class HexIntDltArg : IntDltArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HexIntDltArg"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bytesLength">The length of the bytes.</param>
        public HexIntDltArg(long data, int bytesLength) : base(data)
        {
            DataBytesLength = bytesLength;
        }

        /// <summary>
        /// Gets the length of the data bytes.
        /// </summary>
        /// <value>The length of the data bytes.</value>
        public int DataBytesLength { get; private set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            switch (DataBytesLength) {
            case 1:
                return string.Format("0x{0:x2}", Data);
            case 2:
                return string.Format("0x{0:x4}", Data);
            case 4:
                return string.Format("0x{0:x8}", Data);
            case 8:
                return string.Format("0x{0:x16}", Data);
            default:
                return string.Format("0x{0:x2}", Data);
            }
        }

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            switch (DataBytesLength) {
            case 1:
                return strBuilder.AppendFormat("0x{0:x2}", Data);
            case 2:
                return strBuilder.AppendFormat("0x{0:x4}", Data);
            case 4:
                return strBuilder.AppendFormat("0x{0:x8}", Data);
            case 8:
                return strBuilder.AppendFormat("0x{0:x16}", Data);
            default:
                return strBuilder.AppendFormat("0x{0:x2}", Data);
            }
        }
    }
}
