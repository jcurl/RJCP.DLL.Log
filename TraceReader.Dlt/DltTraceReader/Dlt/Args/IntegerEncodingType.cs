namespace RJCP.Diagnostics.Log.Dlt.Args
{
    /// <summary>
    /// Encoding used for DLT Integer argument types.
    /// </summary>
    public enum IntegerEncodingType
    {
        /// <summary>
        /// Represents an integer argument types in decimal format.
        /// </summary>
        Decimal = 0,

        /// <summary>
        /// Represents an integer argument type in hexadecimal format.
        /// </summary>
        Hex = 2,

        /// <summary>
        /// Represents an integer argument type in binary format.
        /// </summary>
        Binary = 3
    }
}
