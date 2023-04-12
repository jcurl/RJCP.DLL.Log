namespace RJCP.Diagnostics.Log.Constraints
{
    /// <summary>
    /// A constraint checking for a specific message identifier.
    /// </summary>
    public sealed class DltMessageId : IMatchConstraint
    {
        private readonly int m_MessageId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltMessageId"/> class.
        /// </summary>
        /// <param name="message">The non-verbose message id to check for.</param>
        public DltMessageId(int message)
        {
            m_MessageId = message;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            if (line is DltNonVerboseTraceLine nonverboseLine) {
                return nonverboseLine.MessageId == m_MessageId;
            }
            return false;
        }
    }
}
