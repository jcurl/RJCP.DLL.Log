namespace RJCP.Diagnostics.Log.Dlt.Args
{
    /// <summary>
    /// Base class for representing an unknown DLT argument.
    /// </summary>
    public abstract class UnknownDltArg : DltArgBase<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownDltArg"/> class.
        /// </summary>
        /// <param name="data">The value of the byte array representing the argument bytes.</param>
        /// <returns>The reference to the original string builder.</returns>
        protected UnknownDltArg(byte[] data) : base(data) { }
    }
}
