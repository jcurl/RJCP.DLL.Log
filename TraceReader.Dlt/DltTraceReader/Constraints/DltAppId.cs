namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;

    /// <summary>
    /// A check constraint against a DLT Application Identifier <see cref="DltTraceLineBase.ApplicationId"/>.
    /// </summary>
    public class DltAppId : IMatchConstraint
    {
        private readonly string m_AppId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltAppId"/> class.
        /// </summary>
        /// <param name="appId">The Application identifier that should match.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appId"/> is <see langword="null"/>.
        /// </exception>
        public DltAppId(string appId)
        {
            if (appId == null)
                throw new ArgumentNullException(nameof(appId));
            m_AppId = appId;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.ApplicationId &&
                   dltLine.ApplicationId.Equals(m_AppId, StringComparison.Ordinal);
        }
    }
}
