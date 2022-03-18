namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Collections.Generic;

    internal abstract class ExpressionTreeBuilder
    {
        protected ExpressionTreeBuilder(IEnumerator<Token> tokens)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            if (!tokens.MoveNext()) throw new ConstraintException("No constraints defined");
            Expression = BuildExpressionTree(tokens, out bool parsing);
            if (parsing) throw new ConstraintException("Error in expression");
        }

        public BTree<BNode> Expression { get; }

        public int Nodes { get; private set; }

        public int CheckNodes { get; private set; }

        private BTree<BNode> BuildExpressionTree(IEnumerator<Token> tokens, out bool parsing)
        {
            BTree<BNode> tree = null;
            parsing = true;
            while (parsing) {
                if (tokens.Current.Operation == Operation.Check) {
                    BTree<BNode> expr = BuildExpressionTree(tokens.Current.Constraint);
                    if (tree == null) {
                        tree = expr;
                    } else {
                        tree = Node(BOperation.And, tree, expr);
                    }
                    parsing = tokens.MoveNext();
                } else if (tokens.Current.Operation == Operation.Not) {
                    if (!tokens.MoveNext()) throw new ConstraintException("Operator 'Not' without right operand");
                    if (tokens.Current.Operation == Operation.Not) {
                        // Double "not", resulting in a No-Op
                    } else if (tokens.Current.Operation == Operation.Check) {
                        BTree<BNode> expr = BuildExpressionTree(tokens.Current.Constraint);
                        expr = Node(BOperation.Not, null, expr);
                        if (tree == null) {
                            tree = expr;
                        } else {
                            tree = Node(BOperation.And, tree, expr);
                        }
                    } else {
                        throw new ConstraintException("Undefined 'Not.Or' operation");
                    }
                    parsing = tokens.MoveNext();
                } else if (tokens.Current.Operation == Operation.Or) {
                    if (tree == null) throw new ConstraintException("Operator 'Or' without left operand");
                    if (!tokens.MoveNext()) throw new ConstraintException("Operator 'Or' without right operand");
                    tree = Node(BOperation.Or, tree, BuildExpressionTree(tokens, out parsing));
                }
            }
            return tree;
        }

        protected abstract BTree<BNode> BuildExpressionTree(IMatchConstraint constraint);

        // The next functions create nodes with an identifier, that counts from zero upwards. This is used as a
        // "hashing" function into an array for maintaining results during evaluation and to remember which nodes have
        // been visited. The hash is "perfect" indicating that there are no collisions and requires the minimum amount
        // of space, so it's also compact.

        protected BTree<BNode> Node(BOperation operation, BTree<BNode> left, BTree<BNode> right)
        {
            BTree<BNode> node = new BTree<BNode>(new BNode(operation), left, right);
            node.Value.Id = Nodes;
            Nodes++;
            return node;
        }

        protected BTree<BNode> Node(IMatchConstraint constraint)
        {
            BTree<BNode> node = new BTree<BNode>(new BNode(constraint));
            node.Value.Id = Nodes;
            Nodes++;
            CheckNodes++;
            return node;
        }

        protected BNode NodeCopy(BNode node)
        {
            BNode nnode;

            if (node.Operation == BOperation.Check) {
                nnode = new BNode(node.Function);
                CheckNodes++;
            } else {
                nnode = new BNode(node.Operation);
            }
            nnode.Id = Nodes;
            Nodes++;
            return nnode;
        }
    }
}
