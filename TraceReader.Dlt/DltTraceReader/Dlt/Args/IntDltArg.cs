namespace RJCP.Diagnostics.Log.Dlt.Args
{
    /// <summary>
    /// A base type class for all integer types.
    /// </summary>
    /// <remarks>
    /// When decoding integer types, they can be cast to this type to extract the information in a common way.
    /// </remarks>
    public abstract class IntDltArg : DltArgBase<long>
    {
        private int m_DataBytesLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntDltArg"/> class with the given signed integer value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        protected IntDltArg(long data) : base(data) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntDltArg"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="bytesLength">The size (in bytes) of the integer.</param>
        protected IntDltArg(long data, int bytesLength) : base(data)
        {
            m_DataBytesLength = bytesLength;
        }

        /// <summary>
        /// Gets the size (in bytes) of the stored integer.
        /// </summary>
        /// <value>The size (in bytes) of the stored integer.</value>
        public int DataBytesLength
        {
            get
            {
                if (m_DataBytesLength == 0) {
                    m_DataBytesLength = GetLength();
                }
                return m_DataBytesLength;
            }
        }

        /// <summary>
        /// Gets the best fit size based on the value.
        /// </summary>
        /// <returns>The best fit size (in bytes) of the value.</returns>
        protected abstract int GetLength();

        /// <summary>
        /// Gets the best fit size based on the value as a signed integer.
        /// </summary>
        /// <returns>The best fit size (in bytes) of the signed value.</returns>
        protected int GetLengthSigned()
        {
            if (Data >= sbyte.MinValue && Data <= sbyte.MaxValue) return 1;
            if (Data >= short.MinValue && Data <= short.MaxValue) return 2;
            if (Data >= int.MinValue && Data <= int.MaxValue) return 4;
            return 8;
        }

        const long len8 = unchecked((long)0xFFFFFFFF_FFFFFF00);
        const long len16 = unchecked((long)0xFFFFFFFF_FFFF0000);
        const long len32 = unchecked((long)0xFFFFFFFF_00000000);

        /// <summary>
        /// Gets the best fit size based on the value as a unsigned integer.
        /// </summary>
        /// <returns>The best fit size (in bytes) of the unsigned value.</returns>
        protected int GetLengthUnsigned()
        {
            if ((Data & len8) == 0) return 1;
            if ((Data & len16) == 0) return 2;
            if ((Data & len32) == 0) return 4;
            return 8;
        }
    }
}
