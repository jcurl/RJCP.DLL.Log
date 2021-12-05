namespace RJCP.Diagnostics.Log
{
    using Dlt;

    /// <summary>
    /// Representation of a single trace line in the DLT protocol.
    /// </summary>
    public class DltTraceLine : DltTraceLineBase
    {
        /// <summary>
        /// Describes the features which are available on this trace line.
        /// </summary>
        /// <value>The features available on this trace line.</value>
        /// <remarks>
        /// A DLT line has many fields of which some may not be present. This property allows to determine which of
        /// those fields are valid.
        /// </remarks>
        public override IDltLineFeatures Features { get; } = new DltLineFeatures();
    }
}
