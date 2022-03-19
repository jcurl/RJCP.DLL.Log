namespace RJCP.App.DltDump.Infrastructure.Constraints
{
    using RJCP.Diagnostics.Log;
    using RJCP.Diagnostics.Log.Constraints;

    /// <summary>
    /// A constraint object that specifies no constraints.
    /// </summary>
    public sealed class BlockAll : IMatchConstraint
    {
        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return false;
        }
    }
}
