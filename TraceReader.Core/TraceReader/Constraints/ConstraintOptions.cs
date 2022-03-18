namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    /// <summary>
    /// A set of options when defining a <see cref="Constraint"/>.
    /// </summary>
    [Flags]
    public enum ConstraintOptions
    {
        /// <summary>
        /// The constraint should be interpreted, not compiled.
        /// </summary>
        Interpreted = 0,

        /// <summary>
        /// The constraint should be compiled. Causes an impact on the first time its used.
        /// </summary>
        Compiled = 1
    }
}
