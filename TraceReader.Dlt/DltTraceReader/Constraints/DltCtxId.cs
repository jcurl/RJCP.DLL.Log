namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;

    /// <summary>
    /// A check constraint against a DLT Context Id <see cref="DltTraceLineBase.ContextId"/>.
    /// </summary>
    public sealed class DltCtxId : IMatchConstraint
    {
        private readonly string m_CtxId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltCtxId"/> class.
        /// </summary>
        /// <param name="id">The Context identifier that should match.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
        public DltCtxId(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            m_CtxId = id;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.ContextId &&
                   dltLine.ContextId.Equals(m_CtxId, StringComparison.Ordinal);
        }
    }
}
