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
        /// <summary>
        /// Initializes a new instance of the <see cref="IntDltArg"/> class with the given signed integer value.
        /// </summary>
        /// <param name="data">The value of the data.</param>
        protected IntDltArg(long data) : base(data) { }
    }
}
