namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;

    /// <summary>
    /// A check constraint against a DLT Logger Time stamp in the Storage Header <see cref="DltTraceLineBase.TimeStamp"/>.
    /// </summary>
    public sealed class DltNotBeforeDate : IMatchConstraint
    {
        private readonly DateTime m_NotBefore;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltNotBeforeDate"/> class.
        /// </summary>
        /// <param name="notBefore">The time stamp which should match for all messages after.</param>
        /// <remarks>
        /// The comparison is based on the <see cref="DateTime"/> comparison, which ignores the
        /// <see cref="DateTime.Kind"/>. Therefore, you should make sure that the time stamps are compatible before
        /// comparing.
        /// </remarks>
        public DltNotBeforeDate(DateTime notBefore)
        {
            m_NotBefore = notBefore;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.TimeStamp &&
                   dltLine.TimeStamp >= m_NotBefore;
        }
    }
}
