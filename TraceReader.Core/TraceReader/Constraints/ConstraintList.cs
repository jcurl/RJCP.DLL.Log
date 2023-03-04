namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of Constraints.
    /// </summary>
    /// <remarks>
    /// This implementation uses a simple but effective mechanism for building up constraint expressions using a list.
    /// </remarks>
    internal class ConstraintList : IConstraintBase
    {
        // This precedence table is fixed also in code, and is not customizable (not that you want to, else all laws of
        // boolean logic don't work)! That is because this implementation makes the "and" logic implicit with the
        // previous result.
        private readonly Dictionary<Operation, int> ConstraintPrecedence =
            new Dictionary<Operation, int>() {
                { Operation.Not, 1 },
                { Operation.Check, 2 },
                { Operation.Or, 3 }
            };

        private IEnumerator<Token> m_Tokens;

        /// <summary>
        /// Builds an internal expression from the list of tokens provided.
        /// </summary>
        /// <param name="tokens">The list of tokens in order.</param>
        public IConstraintBase Build(IEnumerator<Token> tokens)
        {
            m_Tokens ??= tokens;
            return this;
        }

        /// <summary>
        /// Takes the internal expression from <see cref="Build(IEnumerator{Token})"/> and optionally compiles the
        /// expression.
        /// </summary>
        public IConstraintBase Compile() { return this; }

        /// <summary>
        /// Evaluates the specified line.
        /// </summary>
        /// <param name="line">The line to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        /// <exception cref="ConstraintException">No constraints defined or Error in expression</exception>
        /// <remarks>
        /// Before this function is called, it is expected that a list of tokens is given with the
        /// <see cref="Build(IEnumerator{Token})"/> method.
        /// </remarks>
        public bool Evaluate(ITraceLine line)
        {
            m_Tokens.Reset();
            if (!m_Tokens.MoveNext()) throw new ConstraintException("No constraints defined");

            bool result = Evaluate(ConstraintPrecedence[Operation.Check], m_Tokens, line, out bool parsing);
            if (parsing) throw new ConstraintException("Error in expression");
            return result;
        }

        private bool Evaluate(int precedenceLevel, IEnumerator<Token> tokens, ITraceLine line, out bool parsing)
        {
            bool eval = false;
            bool result = true;

            parsing = true;
            while (parsing) {
                Operation operation = tokens.Current.Operation;
                int tokenPrecedence = ConstraintPrecedence[operation];

                switch (operation) {
                case Operation.Check:
                    // Only evaluate if the left is true. If the left is false, then the result is always false
                    // regardless of the right. This speeds up evaluation.
                    if (result) result = tokens.Current.Constraint.Check(line);
                    parsing = tokens.MoveNext();
                    eval = true;
                    break;
                case Operation.Not:
                    // Only evaluate the next expression. Also evaluate only if the left is true (AND operation).
                    if (!tokens.MoveNext()) throw new ConstraintException("Operator 'Not' without right operand");
                    if (tokens.Current.Operation == Operation.Or) throw new ConstraintException("Undefined 'Not.Or' operation");
                    if (result) {
                        result = !Evaluate(tokenPrecedence, tokens, line, out parsing);
                    } else {
                        parsing = tokens.MoveNext();
                    }
                    eval = true;
                    break;
                case Operation.Or:
                    if (!eval) throw new ConstraintException("Operator 'Or' without left operand");
                    if (!tokens.MoveNext()) throw new ConstraintException("Operator 'Or' without right operand");
                    // Only evaluate if the left is false. If the left is true, then the result is always true
                    // regardless of the right. This speeds up evaluation
                    if (!result) {
                        result = Evaluate(tokenPrecedence, tokens, line, out parsing);
                    } else {
                        // Skip over the tokens with lower precedence
                        parsing = Skip(ConstraintPrecedence[Operation.Or], tokens);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Undefined token");
                }

                if (tokenPrecedence > precedenceLevel) return result;
            }
            return result;
        }

        private bool Skip(int precedenceLevel, IEnumerator<Token> token)
        {
            do {
                Operation operation = token.Current.Operation;
                int tokenPrecedence = ConstraintPrecedence[operation];

                if (tokenPrecedence > precedenceLevel) return true;
            } while (token.MoveNext());
            return false;
        }
    }
}
