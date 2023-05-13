namespace RJCP.Diagnostics.Log.Dlt.Verbose
{
    using Args;

    /// <summary>
    /// Unsigned integer argument decoder.
    /// </summary>
    /// <remarks>Can decode 8, 16, 32 or 64 bit unsigned integer values.</remarks>
    public sealed class UnsignedIntArgDecoder : IntegerArgDecoder
    {
        /// <summary>
        /// Creates the 8-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create8BitArgument(byte data, IntegerEncodingType coding)
        {
            switch (coding) {
            case IntegerEncodingType.Hex:
                return new HexIntDltArg(data, 1);
            case IntegerEncodingType.Binary:
                return new BinaryIntDltArg(data, 1);
            default:
                // IntegerCoding.Decimal is the default use case
                return new UnsignedIntDltArg(data, 1);
            }
        }

        /// <summary>
        /// Creates the 16-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create16BitArgument(short data, IntegerEncodingType coding)
        {
            switch (coding) {
            case IntegerEncodingType.Hex:
                return new HexIntDltArg(unchecked((ushort)data), 2);
            case IntegerEncodingType.Binary:
                return new BinaryIntDltArg(unchecked((ushort)data), 2);
            default:
                // .NET uses sign-extension to convert a short to long, thus an explicit conversion to an unsigned type
                // is required to prevent this. IntegerCoding.Decimal is the default use case
                return new UnsignedIntDltArg(unchecked((ushort)data), 2);
            }
        }

        /// <summary>
        /// Creates the 32-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create32BitArgument(int data, IntegerEncodingType coding)
        {
            switch (coding) {
            case IntegerEncodingType.Hex:
                return new HexIntDltArg(unchecked((uint)data), 4);
            case IntegerEncodingType.Binary:
                return new BinaryIntDltArg(unchecked((uint)data), 4);
            default:
                // .NET uses sign-extension to convert a int to long, thus an explicit conversion to an unsigned type is
                // required to prevent this. IntegerCoding.Decimal is the default use case
                return new UnsignedIntDltArg(unchecked((uint)data), 4);
            }
        }

        /// <summary>
        /// Creates the 64-bit numeric argument instance.
        /// </summary>
        /// <param name="data">The data to be used when creating the argument instance.</param>
        /// <param name="coding">The coding of this argument.</param>
        /// <returns>The argument instance.</returns>
        protected override IDltArg Create64BitArgument(long data, IntegerEncodingType coding)
        {
            switch (coding) {
            case IntegerEncodingType.Hex:
                return new HexIntDltArg(data, 8);
            case IntegerEncodingType.Binary:
                return new BinaryIntDltArg(data, 8);
            default:
                // IntegerCoding.Decimal is the default use case
                return new UnsignedIntDltArg(data, 8);
            }
        }
    }
}
