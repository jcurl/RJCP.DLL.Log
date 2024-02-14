namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Collections.Generic;

    internal class BooleanCompiler
    {
        private readonly BooleanIlGenerator m_BooleanIlGen;

        public BooleanCompiler(ExpressionTreeBuilder expressionTree)
        {
            m_BooleanIlGen = Compile(expressionTree);
        }

        public bool Evaluate(ITraceLine line)
        {
            return m_BooleanIlGen.Evaluate(line);
        }

        private struct EvalData
        {
            public bool Visited { get; private set; }

            public void Visit()
            {
                Visited = true;
            }

            public bool Labelled { get; private set; }

            public BranchAlwaysOperation Branch { get; private set; }

            public void SetLabel(BranchAlwaysOperation branch)
            {
                Labelled = true;
                Branch = branch;
            }
        }

        private static BooleanIlGenerator Compile(ExpressionTreeBuilder expressionTree)
        {
            BTree<BNode> node = expressionTree.Expression;
            EvalData[] state = new EvalData[expressionTree.Nodes];
            Stack<BTree<BNode>> evalStack = new();

            BooleanIlGenerator compiledEvaluation = new(expressionTree.CheckNodes);

            while (true) {
                if (state[node.Value.Id].Visited) {
                    if (evalStack.Count == 0) {
                        compiledEvaluation.EmitReturn();
                        compiledEvaluation.Optimize();
                        compiledEvaluation.Compile();
                        return compiledEvaluation;
                    }
                    node = evalStack.Pop();
                    continue;
                }

                switch (node.Value.Operation) {
                case BOperation.Check:
                    compiledEvaluation.EmitCheck(node.Value.Function);
                    state[node.Value.Id].Visit();
                    break;
                case BOperation.And:
                    if (!state[node.Left.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Left;
                        continue;
                    }
                    if (!state[node.Value.Id].Labelled) {
                        state[node.Value.Id].SetLabel(compiledEvaluation.EmitBranchFalse());
                    }
                    if (!state[node.Right.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Right;
                        continue;
                    }
                    compiledEvaluation.EmitBranchTargetSetFalse(state[node.Value.Id].Branch);
                    state[node.Value.Id].Visit();
                    break;
                case BOperation.Or:
                    if (!state[node.Left.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Left;
                        continue;
                    }
                    if (!state[node.Value.Id].Labelled) {
                        state[node.Value.Id].SetLabel(compiledEvaluation.EmitBranchTrue());
                    }
                    if (!state[node.Right.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Right;
                        continue;
                    }
                    compiledEvaluation.EmitBranchTargetSetTrue(state[node.Value.Id].Branch);
                    state[node.Value.Id].Visit();
                    break;
                case BOperation.Not:
                    if (!state[node.Right.Value.Id].Visited) {
                        evalStack.Push(node);
                        node = node.Right;
                        continue;
                    }
                    compiledEvaluation.EmitInvert();
                    state[node.Value.Id].Visit();
                    break;
                default:
                    throw new InvalidOperationException("Unhandled token");
                }
            }
        }
    }
}
