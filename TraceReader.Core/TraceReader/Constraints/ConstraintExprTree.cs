namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Collections.Generic;
    using Compiler;

    /// <summary>
    /// An implementation of Constraints.
    /// </summary>
    /// <remarks>
    /// This implementation uses expression trees (binary trees) which then describes the precedence and ordering.
    /// Expression trees have the advantage that we can better handle subexpressions (which <see cref="ConstraintList"/>
    /// can't do very well, it would just call <see cref="IMatchConstraint.Check(ITraceLine)"/>).
    /// </remarks>
    internal class ConstraintExprTree : IConstraintBase
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
                    if (constraintBase is ConstraintExprTree exprConstraint) {
                        expr = exprConstraint.m_ExpressionTree.Expression.Copy(NodeCopy);
                    }
                }

                expr ??= Node(constraint);
                return expr;
            }
        }

        private ExpressionTree m_ExpressionTree;

        /// <summary>
        /// Builds an internal expression from the list of tokens provided.
        /// </summary>
        /// <param name="tokens">The list of tokens in order.</param>
        /// <exception cref="ConstraintException">No constraints defined or Error in expression</exception>
        public IConstraintBase Build(IEnumerator<Token> tokens)
        {
            m_ExpressionTree ??= new ExpressionTree(tokens);
            return this;
        }

        /// <summary>
        /// Takes the internal expression from <see cref="Build(IEnumerator{Token})"/> and optionally compiles the
        /// expression.
        /// </summary>
        public IConstraintBase Compile() { return this; }

        private struct EvalData
        {
            public bool Visited { get; set; }

            public bool Result { get; set; }
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
            Stack<BTree<BNode>> evalStack = new Stack<BTree<BNode>>();
            BTree<BNode> node = m_ExpressionTree.Expression;

            // EvalData maintains the visit flag using the node.Value.Id as the index to the array. This allows two
            // evaluations now to occur in parallel. Using a struct for EvalData is faster (no indirection) and more
            // efficient memory usage.
            EvalData[] state = new EvalData[m_ExpressionTree.Nodes];

            while (true) {
                if (state[node.Value.Id].Visited) {
                    if (evalStack.Count == 0) return state[node.Value.Id].Result;
                    node = evalStack.Pop();
                    continue;
                }

                bool result;
                switch (node.Value.Operation) {
                case BOperation.Check:
                    state[node.Value.Id].Result = node.Value.Function.Check(line);
                    state[node.Value.Id].Visited = true;
                    break;
                case BOperation.And:
                    if (!state[node.Left.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Left;
                        continue;
                    }
                    result = state[node.Left.Value.Id].Result;
                    if (result) {
                        if (!state[node.Right.Value.Id].Visited) {
                            evalStack.Push(node);
                            node = node.Right;
                            continue;
                        }
                        result = state[node.Right.Value.Id].Result;
                    }
                    state[node.Value.Id].Result = result;
                    state[node.Value.Id].Visited = true;
                    break;
                case BOperation.Or:
                    if (!state[node.Left.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Left;
                        continue;
                    }
                    result = state[node.Left.Value.Id].Result;
                    if (!result) {
                        if (!state[node.Right.Value.Id].Visited) {
                            evalStack.Push(node);
                            node = node.Right;
                            continue;
                        }
                        result = state[node.Right.Value.Id].Result;
                    }
                    state[node.Value.Id].Result = result;
                    state[node.Value.Id].Visited = true;
                    break;
                case BOperation.Not:
                    if (!state[node.Right.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Right;
                        continue;
                    }
                    state[node.Value.Id].Result = !state[node.Right.Value.Id].Result;
                    state[node.Value.Id].Visited = true;
                    break;
                default:
                    throw new InvalidOperationException("Unhandled token");
                }
            }
        }
    }
}
