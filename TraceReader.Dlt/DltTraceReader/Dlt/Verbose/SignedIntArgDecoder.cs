namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using Args;

    /// <summary>
    /// Signed integer argument decoder.
    /// </summary>
    /// <remarks>
    /// Can decode 8, 16, 32 or 64 bit signed integer values.
    /// </remarks>
    public sealed class SignedIntArgDecoder : IntegerArgDecoder
    {
        /// <summary>
        /// Creates the 8-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding for this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create8BitArgument(byte data, IntegerEncodingType coding)
        {
            return new SignedIntDltArg(unchecked((sbyte)data), 1);
        }

        /// <summary>
        /// Creates the 16-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create16BitArgument(short data, IntegerEncodingType coding)
        {
            return new SignedIntDltArg(data, 2);
        }

        /// <summary>
        /// Creates the 32-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create32BitArgument(int data, IntegerEncodingType coding)
        {
            return new SignedIntDltArg(data, 4);
        }

        /// <summary>
        /// Creates the 64-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create64BitArgument(long data, IntegerEncodingType coding)
        {
            return new SignedIntDltArg(data, 8);
        }
    }
}
