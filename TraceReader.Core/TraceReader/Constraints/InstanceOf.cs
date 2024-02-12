namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    /// <summary>
    /// Checks that the <see cref="ITraceLine"/> being checked is of a given type or derived thereof.
    /// </summary>
    public sealed class InstanceOf : IMatchConstraint
    {
        private readonly Type m_Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceOf"/> class.
        /// </summary>
        /// <param name="type">The type which the <see cref="ITraceLine"/> should be an instance of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type"/> must by derived from <see cref="ITraceLine"/>.
        /// </exception>
        public InstanceOf(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            if (!typeof(ITraceLine).IsAssignableFrom(type))
                throw new ArgumentException("Type must be derived from ITraceLine", nameof(type));
            m_Type = type;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return m_Type.IsInstanceOfType(line);
        }
    }
}
