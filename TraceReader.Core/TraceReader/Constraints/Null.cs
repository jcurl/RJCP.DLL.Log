namespace RJCP.Diagnostics.Log.Constraints
{
    /// <summary>
    /// Checks if the <see cref="ITraceLine"/> is <see langword="null"/>.
    /// </summary>
    public sealed class Null : IMatchConstraint
    {
        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return line is null;
        }
    }
}
