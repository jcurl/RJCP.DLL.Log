namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using Dlt;

    /// <summary>
    /// A check constraint against a DLT ECU Id <see cref="DltTraceLineBase.EcuId"/>.
    /// </summary>
    public sealed class DltEcuId : IMatchConstraint
    {
        private readonly string m_EcuId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltEcuId"/> class.
        /// </summary>
        /// <param name="id">The ECU identifier that should match.</param>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
        public DltEcuId(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            m_EcuId = id;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.EcuId &&
                   dltLine.EcuId.Equals(m_EcuId, StringComparison.Ordinal);
        }
    }
}
