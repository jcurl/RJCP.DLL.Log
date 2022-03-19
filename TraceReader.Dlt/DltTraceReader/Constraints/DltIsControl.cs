namespace RJCP.Diagnostics.Log.Constraints
{
    using Dlt;

    /// <summary>
    /// A check constraint against the DLT Type <see cref="DltType"/> of control messages.
    /// </summary>
    public sealed class DltIsControl : IMatchConstraint
    {
        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            DltTraceLineBase dltLine = (DltTraceLineBase)line;
            return dltLine.Features.MessageType &&
                   ((int)dltLine.Type & 0x0E) == 0x06;
        }
    }
}
