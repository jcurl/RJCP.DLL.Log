namespace RJCP.Diagnostics.Log.Constraints
{
    using Dlt;

    /// <summary>
    /// A check constraint against a DLT Application Identifier <see cref="DltTraceLineBase.SessionId"/>.
    /// </summary>
    public class DltSessionId : IMatchConstraint
    {
        private readonly int m_SessionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSessionId"/> class.
        /// </summary>
        /// <param name="sessionId">The session identifier that should match.</param>
        public DltSessionId(int sessionId)
        {
            m_SessionId = sessionId;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// The original line must have the flag indicating that a session identifier is present (see
        /// <see cref="DltTraceLineBase.Features"/>) and be the same as the condition when constructed.
        /// </remarks>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.SessionId &&
                   dltLine.SessionId == m_SessionId;
        }
    }
}
