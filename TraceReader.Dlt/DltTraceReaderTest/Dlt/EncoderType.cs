namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// Choose the style of test case for encoding.
    /// </summary>
    public enum EncoderType
    {
        /// <summary>
        /// Test the specific argument encoder in the namespace <c>ArgEncoder</c>.
        /// </summary>
        Argument,

        /// <summary>
        /// Test the <see cref="ArgEncoder.VerboseDltEncoder"/>.
        /// </summary>
        Arguments,

        /// <summary>
        /// Test the trace encoder <see cref="Encoder.DltTraceEncoder"/>.
        /// </summary>
        TraceEncoder,

        /// <summary>
        /// Test the trace writer <see cref="DltTraceWriter"/>.
        /// </summary>
        TraceWriter
    }
}
