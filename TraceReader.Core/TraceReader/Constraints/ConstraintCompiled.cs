namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Collections.Generic;
    using Compiler;

    /// <summary>
    /// An implementation of Constraints.
    /// </summary>
    /// <remarks>
    /// This implementation uses expression trees (binary trees) which then describes the precedence and ordering. Using
    /// expression trees allow us to efficiently compile each check in the order in which the checks should be done from
    /// top to bottom.
    /// </remarks>
    internal class ConstraintCompiled : IConstraintBase
    {
        private sealed class ExpressionTree : ExpressionTreeBuilder
        {
            public ExpressionTree(IEnumerator<Token> tokens) : base(tokens) { }

            protected override BTree<BNode> BuildExpressionTree(IMatchConstraint constraint)
            {
                BTree<BNode> expr = null;

                // If the constraint is a "Constraint", we copy the expression here.
                if (constraint is Constraint ourConstraint) {
                    IConstraintBase constraintBase = ourConstraint.Build();
                    if (constraintBase is ConstraintCompiled exprConstraint) {
                        expr = exprConstraint.m_ExpressionTree.Expression.Copy(NodeCopy);
                    }
                }

                if (expr == null) expr = Node(constraint);
                return expr;
            }
        }

        private ExpressionTree m_ExpressionTree;
        private BooleanCompiler m_CompiledEvaluation;

        /// <summary>
        /// Builds an internal expression from the list of tokens provided.
        /// </summary>
        /// <param name="tokens">The list of tokens in order.</param>
        /// <exception cref="ConstraintException">No constraints defined or Error in expression</exception>
        public IConstraintBase Build(IEnumerator<Token> tokens)
        {
            if (m_ExpressionTree == null) {
                m_ExpressionTree = new ExpressionTree(tokens);
            }
            return this;
        }

        /// <summary>
        /// Takes the internal expression from <see cref="Build(IEnumerator{Token})"/> and optionally compiles the
        /// expression
        /// </summary>
        public IConstraintBase Compile()
        {
            if (m_CompiledEvaluation == null) {
                m_CompiledEvaluation = new BooleanCompiler(m_ExpressionTree);
            }
            return this;
        }

        /// <summary>
        /// Evaluates the specified line.
        /// </summary>
        /// <param name="line">The line to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        /// <exception cref="InvalidOperationException">Unhandled token.</exception>
        /// <remarks>
        /// Before this function is called, it is expected that a list of tokens is given with the
        /// <see cref="Build(IEnumerator{Token})"/> method.
        /// </remarks>
        public bool Evaluate(ITraceLine line)
        {
            return m_CompiledEvaluation.Evaluate(line);
        }
    }
}
