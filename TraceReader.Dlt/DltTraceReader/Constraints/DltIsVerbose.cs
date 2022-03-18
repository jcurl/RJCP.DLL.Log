namespace RJCP.Diagnostics.Log.Constraints
{
    using Dlt;

    /// <summary>
    /// A check constraint against the DLT <see cref="DltLineFeatures.IsVerbose"/> flag.
    /// </summary>
    public class DltIsVerbose : IMatchConstraint
    {
        private readonly bool m_IsVerbose;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltIsVerbose"/> class.
        /// </summary>
        /// <param name="isVerbose">
        /// If <see langword="true"/>, the <see cref="DltTraceLineBase"/> is expected to be
        /// <see cref="DltLineFeatures.IsVerbose"/>.
        /// </param>
        public DltIsVerbose(bool isVerbose)
        {
            m_IsVerbose = isVerbose;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.IsVerbose == m_IsVerbose;
        }
    }
}
