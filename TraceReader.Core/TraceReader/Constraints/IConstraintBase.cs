namespace RJCP.Diagnostics.Log.Constraints
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for internal constraint expression building and evaluation.
    /// </summary>
    internal interface IConstraintBase
    {
        /// <summary>
        /// Builds an internal expression from the list of tokens provided.
        /// </summary>
        /// <param name="tokens">The list of tokens in order.</param>
        IConstraintBase Build(IEnumerator<Token> tokens);

        /// <summary>
        /// Takes the internal expression from <see cref="Build(IEnumerator{Token})"/> and optionally compiles the
        /// expression.
        /// </summary>
        IConstraintBase Compile();

        /// <summary>
        /// Evaluates the specified line.
        /// </summary>
        /// <param name="line">The line to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        /// <remarks>
        /// Before this function is called, it is expected that a list of tokens is given with the
        /// <see cref="Build(IEnumerator{Token})"/> method.
        /// </remarks>
        bool Evaluate(ITraceLine line);
    }
}
