namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    /// <summary>
    /// DLT integer argument type in binary format.
    /// </summary>
    /// <remarks>This argument type is only capable to hold a 64-bit long integer.</remarks>
    public class BinaryIntDltArg : DltArgBase<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryIntDltArg"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bytesLength">The length of the bytes.</param>
        /// <remarks>
        /// If <paramref name="bytesLength"/> is different than 1, 2, 4, 8, a 64-bit value shall be interpreted.
        /// </remarks>
        public BinaryIntDltArg(long data, int bytesLength) : base(data)
        {
            DataBytesLength = bytesLength;
        }

        /// <summary>
        /// Gets the length of the data bytes.
        /// </summary>
        /// <value>The length of the data bytes.</value>
        public int DataBytesLength { get; private set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return Append(sb).ToString();
        }

        private readonly string[][] Nibble = new[] {
            new string[] {
                "0b0000", "0b0001", "0b0010", "0b0011", "0b0100", "0b0101", "0b0110", "0b0111",
                "0b1000", "0b1001", "0b1010", "0b1011", "0b1100", "0b1101", "0b1110", "0b1111"
            },
            new string[] {
                " 0000", " 0001", " 0010", " 0011", " 0100", " 0101", " 0110", " 0111",
                " 1000", " 1001", " 1010", " 1011", " 1100", " 1101", " 1110", " 1111"
            }
        };

        /// <summary>
        /// Appends the argument to the end of the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="strBuilder">The string builder to append this argument to.</param>
        public override StringBuilder Append(StringBuilder strBuilder)
        {
            const int shift = 56;
            const ulong mask = 0xFF00000000000000;
            ulong data = unchecked((ulong)Data);
            bool print = false;
            int index = 0;
            for (int i = 8; i > 0; i--) {
                byte value = (byte)((data & mask) >> shift);
                print |= (value != 0);
                if (i <= DataBytesLength || print) {
                    strBuilder.Append(Nibble[index][value >> 4]);
                    strBuilder.Append(Nibble[1][value & 0xF]);
                    index = 1;
                }
                data <<= 8;
            }

            return strBuilder;
        }
    }
}
