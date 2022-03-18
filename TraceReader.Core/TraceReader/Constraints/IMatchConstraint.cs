namespace RJCP.Diagnostics.Log.Constraints
{
    /// <summary>
    /// Interface IMatchConstraint for defining a constraint when checking an <see cref="ITraceLine"/> .
    /// </summary>
    public interface IMatchConstraint
    {
        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ConstraintException">
        /// May be raised if there is an error in the constraint definition.
        /// </exception>
        bool Check(ITraceLine line);
    }
}
