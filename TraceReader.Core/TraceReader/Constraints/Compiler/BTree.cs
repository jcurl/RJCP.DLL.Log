namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An internal implementation of a Binary Tree.
    /// </summary>
    /// <typeparam name="T">The type parameter for the value stored in this tree node.</typeparam>
    /// <remarks>
    /// This is a simple binary tree. The data structure has no checks for the best performance possible. Because there
    /// are no checks to ensure there are no cycles that can be built in, it is not considered safe for general use.
    /// Algorithms must be reviewed to ensure that they cannot provide cycles, else use a different design.
    /// </remarks>
    internal class BTree<T>
    {
        public BTree(T value)
        {
            Value = value;
        }

        public BTree(T value, BTree<T> left, BTree<T> right)
        {
            Value = value;
            Left = left;
            Right = right;
        }

        public T Value { get; }

        public BTree<T> Left { get; private set; }

        public BTree<T> Right { get; private set; }

        public BTree<T> Copy(Func<T, T> copyAction)
        {
            Queue<BTree<T>> queue = new Queue<BTree<T>>();
            Queue<BTree<T>> qcopy = new Queue<BTree<T>>();
            BTree<T> tree = new BTree<T>(copyAction(Value));

            queue.Enqueue(this);
            qcopy.Enqueue(tree);
            while (queue.Count > 0) {
                BTree<T> node = queue.Dequeue();
                BTree<T> copy = qcopy.Dequeue();
                if (node.Left is object) {
                    copy.Left = new BTree<T>(copyAction(node.Left.Value));
                    queue.Enqueue(node.Left);
                    qcopy.Enqueue(copy.Left);
                }
                if (node.Right is object) {
                    copy.Right = new BTree<T>(copyAction(node.Right.Value));
                    queue.Enqueue(node.Right);
                    qcopy.Enqueue(copy.Right);
                }
            }
            return tree;
        }

        public override string ToString()
        {
            return string.Format("{0}", Value.ToString());
        }
    }
}
