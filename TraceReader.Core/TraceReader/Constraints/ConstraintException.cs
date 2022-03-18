namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class ConstraintException for optionally when a constraint detects a configuration problem.
    /// </summary>
    /// <remarks>
    /// The exception indicates when errors occur during <see cref="IMatchConstraint.Check(ITraceLine)"/> when there is
    /// a logic error in the constraint definition.
    /// </remarks>
    [Serializable]
    public class ConstraintException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        public ConstraintException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConstraintException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
        public ConstraintException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected ConstraintException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
