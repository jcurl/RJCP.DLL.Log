namespace RJCP.Diagnostics.Log.Constraints
{
    using Dlt;

    /// <summary>
    /// A check constraint against the DLT type and subtype <see cref="DltTraceLineBase.Type"/>.
    /// </summary>
    /// <remarks>For more details about the DLT type and subtype possible values, see <see cref="DltType"/>.</remarks>
    public class DltMessageType : IMatchConstraint
    {
        private readonly DltType m_Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltMessageType"/> class.
        /// </summary>
        /// <param name="dltType">The DLT Type that should be identical.</param>
        public DltMessageType(DltType dltType)
        {
            m_Type = dltType;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.MessageType &&
                   dltLine.Type == m_Type;
        }
    }
}
