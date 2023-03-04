namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Generates the IL based on the boolean operations given.
    /// </summary>
    internal class BooleanIlGenerator
    {
        private delegate bool EvaluateDelegate(IMatchConstraint[] constraints, ITraceLine line);

        private readonly IMatchConstraint[] m_ConstraintTable;
        private readonly DynamicMethod m_EvaluationMethod;
        private readonly ILGenerator m_IlGenerator;
        private int m_ConstraintIndex;
        private EvaluateDelegate m_EvaluationDelegate;

        private readonly LinkedList<IOperation> m_Operations = new LinkedList<IOperation>();

        public BooleanIlGenerator(int elements)
        {
            // A pointer to the Check function from the expression tree to a fast lookup table that can be used directly
            // within the DynamicMethod.
            m_ConstraintTable = new IMatchConstraint[elements];

            // This is the method we'll compile to
            m_EvaluationMethod =
                new DynamicMethod(
                    "ConstraintCheck",
                    typeof(bool),
                    new[] {
                        typeof(IMatchConstraint[]),
                        typeof(ITraceLine)
                    },
                    typeof(ConstraintCompiled).Module);
            m_EvaluationMethod.DefineParameter(1, ParameterAttributes.In, "constraints");
            m_EvaluationMethod.DefineParameter(2, ParameterAttributes.In, "line");
            m_IlGenerator = m_EvaluationMethod.GetILGenerator();
        }

        /// <summary>
        /// Evaluates the constraint given by the specified index, and puts the result on the stack.
        /// </summary>
        public void EmitCheck(IMatchConstraint constraint)
        {
            m_ConstraintTable[m_ConstraintIndex] = constraint;
            m_Operations.AddLast(new CheckOperation(m_ConstraintIndex));
            m_ConstraintIndex++;
        }

        /// <summary>
        /// Branches if the current value on the stack is false.
        /// </summary>
        /// <returns>The branch operation that can be given to a target.</returns>
        public BranchOperation EmitBranchFalse()
        {
            BranchOperation op = new BranchOperation(false);
            m_Operations.AddLast(op);
            return op;
        }

        /// <summary>
        /// Branches if the current value on the stack is true.
        /// </summary>
        /// <returns>The branch operation that can be given to a target.</returns>
        public BranchOperation EmitBranchTrue()
        {
            BranchOperation op = new BranchOperation(true);
            m_Operations.AddLast(op);
            return op;
        }

        /// <summary>
        /// Finalizes the code.
        /// </summary>
        public void EmitReturn()
        {
            m_Operations.AddLast(new ReturnOperation());
        }

        public void EmitBranchTarget(BranchAlwaysOperation branch)
        {
            m_Operations.AddLast(new BranchTargetOperation(branch));
        }

        public void EmitBranchTargetSetTrue(BranchAlwaysOperation branch)
        {
            m_Operations.AddLast(new BranchTargetSetBoolOperation(true, branch));
        }

        public void EmitBranchTargetSetFalse(BranchAlwaysOperation branch)
        {
            m_Operations.AddLast(new BranchTargetSetBoolOperation(false, branch));
        }

        public void EmitInvert()
        {
            m_Operations.AddLast(new InvertOperation());
        }

        private bool m_Compiled;

        public void Optimize()
        {
            // Ordering of optimizations are important
            OptimizeBranches();
            OptimizeNotBeforeBranch();
            OptimizeNotAfterBranchTargetSet();
            OptimizeBranchTargetSet();
            OptimizeBranchToRet();
        }

        private void OptimizeBranches()
        {
            Dictionary<BranchBaseOperation, BranchTargetOperation> branches = new Dictionary<BranchBaseOperation, BranchTargetOperation>();

            // Map a branch to the branch target. When we merge a BranchTargetSetBoolOperation(XXX, branches) with the
            // next BranchOperation(XXX), because the operations are the same, we move all the branches from the target
            // we're removing to the target for the next BranchOperation.
            LinkedListNode<IOperation> opNode = m_Operations.First;
            while (opNode != null) {
                if (opNode.Value is BranchTargetOperation opBranchTarget) {
                    foreach (BranchBaseOperation opBranch in opBranchTarget.Branches) {
                        branches.Add(opBranch, opBranchTarget);
                    }
                }
                opNode = opNode.Next;
            }

            // Optimizes two operations one after the other, if the result is known from the previous op, then a branch
            // if true/false, then we don't need to generate these instructions.
            opNode = m_Operations.First;
            while (opNode != null) {
                if (opNode.Value is BranchTargetSetBoolOperation opBranchSetTarget) {
                    // b1 = BranchOperation(XX) check(x) BranchTargetSetBoolOperation(XX, b1) <-- opBranchSetTarget b2 =
                    // BranchOperation(YY) <-- opBranch check(y) BranchTargetSetBoolOperation(YY, b1)
                    LinkedListNode<IOperation> opNext = opNode.Next;
                    if (opNext != null) {
                        if (opNext.Value is BranchOperation opBranch) {
                            if (opBranchSetTarget.Value == opBranch.Operation) {
                                // The set operation is the same as the next branch, so the IL generated just pushes the
                                // Operation on the stack, pops it and always branches. So redirect the branch pointing
                                // to opBranchSetTarget to point to the target by opBranch, and remove the
                                // opBranchSetTarget as it's not needed.
                                BranchTargetOperation branchTarget = branches[opBranch];
                                foreach (BranchBaseOperation branch in opBranchSetTarget.Branches) {
                                    branchTarget.Branches.Add(branch);
                                    branches[branch] = branchTarget;
                                }

                                // Now we've redirected the branches, remove the target.
                                LinkedListNode<IOperation> node = opNode;
                                opNode = opNext;
                                m_Operations.Remove(node);
                                continue;
                            } else {
                                // The set operation is different to the next branch, so the IL generated pushes the
                                // operation on the stack and pops it without branching always, thus being a
                                // no-operation. So redirect the branch pointing to opBranchSetTarget to after the next
                                // branch operation
                                BranchTargetOperation branchTarget = new BranchTargetOperation();
                                m_Operations.AddAfter(opNext, branchTarget);
                                foreach (BranchBaseOperation branch in opBranchSetTarget.Branches) {
                                    branchTarget.Branches.Add(branch);
                                    branches[branch] = branchTarget;
                                }

                                // Now we've redirected the branches, remove the target.
                                LinkedListNode<IOperation> node = opNode;
                                opNode = opNext;
                                m_Operations.Remove(node);
                                continue;
                            }
                        }
                    }
                }
                opNode = opNode.Next;
            }
        }

        private void OptimizeNotBeforeBranch()
        {
            InvertOperation opInv = null;

            LinkedListNode<IOperation> opNode = m_Operations.First;
            while (opNode != null) {
                if (opNode.Value is BranchOperation opBranch && opInv != null) {
                    // We can invert the reason for the branch InvertOperation() <-- opInv b2 = BranchOperation(YY) <--
                    // opBranch
                    opBranch.Operation = !opBranch.Operation;

                    // opNode.Previous cannot be null, as opInv is not null, so this is at least the second element in
                    // the list.
                    m_Operations.Remove(opNode.Previous);
                    opInv = null;
                } else {
                    opInv = opNode.Value as InvertOperation;
                }
                opNode = opNode.Next;
            }
        }

        private void OptimizeNotAfterBranchTargetSet()
        {
            // If we have a BranchTargetSetBoolOperation followed by an invert operation before and after, then we can
            // invert the results and remove the inversions

            LinkedListNode<IOperation> opNode = m_Operations.First;
            LinkedListNode<IOperation> opInvNode = null;
            InvertOperation opInvFirst = null;
            while (opNode != null) {
                if (opNode.Value is InvertOperation opInv) {
                    if (opInvFirst != null) {
                        // Everything between here and "opInvFirst" can be inverted and the two inversion operations can
                        // be removed once done
                        //
                        // InvertOperation(); | BranchTargetSetBoolOperation(xx); | BranchTargetSetBoolOperation(!xx);
                        // BranchTargetSetBoolOperation(yy); | BranchTargetSetBoolOperation(!yy); InvertOperation(); |
                        LinkedListNode<IOperation> start = opInvNode.Next;
                        BranchTargetSetBoolOperation opBranchTarget = start.Value as BranchTargetSetBoolOperation;
                        while (opBranchTarget != null) {
                            opBranchTarget.Value = !opBranchTarget.Value;
                            start = start.Next;
                            opBranchTarget = start.Value as BranchTargetSetBoolOperation;
                        }

                        // Now remove the two invert operations that lead to a no-operation
                        m_Operations.Remove(opInvNode); opInvNode = null;
                        opNode = opNode.Next;
                        m_Operations.Remove(start);
                        continue;
                    } else {
                        // Mark the inversion as a potential start operation.
                        opInvFirst = opInv;
                        opInvNode = opNode;
                    }
                } else {
                    if (!(opNode.Value is BranchTargetSetBoolOperation)) {
                        opInvFirst = null;
                    }
                }
                opNode = opNode.Next;
            }
        }

        private void OptimizeBranchTargetSet()
        {
            // If we have two BranchTargetSetBool operations together, they can be optimized as the first doesn't need
            // to branch to the second, but skip it.
            LinkedListNode<IOperation> opNode = m_Operations.First;
            LinkedListNode<IOperation> opBranchTargetNodeFirst = null;
            int elements = 0;
            while (opNode != null) {
                if (opNode.Value is BranchTargetSetBoolOperation) {
                    elements++;
                    if (opBranchTargetNodeFirst == null) {
                        opBranchTargetNodeFirst = opNode;
                        opNode = opNode.Next;
                        continue;
                    }
                } else if (opBranchTargetNodeFirst != null) {
                    if (elements > 1) {
                        // Rework the branches to jump to the end. We only optimize if there are more than one
                        // BranchTargetSetBoolOperation.
                        BranchTargetOperation target = new BranchTargetOperation();
                        LinkedListNode<IOperation> opBranchTargetNode = opBranchTargetNodeFirst;
                        BranchTargetSetBoolOperation opTarget;
                        do {
                            opTarget = opBranchTargetNode.Value as BranchTargetSetBoolOperation;
                            if (opTarget != null) {
                                opTarget.SkipBranch = target;
                                opBranchTargetNode = opBranchTargetNode.Next;
                            }
                        } while (opTarget != null);
                        m_Operations.AddBefore(opNode, target);
                    }
                    elements = 0;
                }
                opNode = opNode.Next;
            }
        }

        private void OptimizeBranchToRet()
        {
            // Find the ret instructions (which are either "ret" or BranchTargetSetBoolOperation) and change those
            // branches to be a "ret" instruction. Branches also occur within the BranchTargetSetBoolOperation.
            LinkedListNode<IOperation> opNode = m_Operations.First;
            while (opNode != null) {
                if (opNode.Value is ReturnOperation) {
                    // If the previous instruction is a branch target, then find out where the branches are and replace
                    // them with a "ret"
                    bool finished;
                    LinkedListNode<IOperation> opRetNode = opNode;
                    do {
                        opRetNode = opRetNode.Previous;
                        finished = true;
                        if (opRetNode.Value is BranchTargetSetBoolOperation opBranchTargetSetModify) {
                            opBranchTargetSetModify.BranchTargetSetType = BranchTargetSetType.Return;
                            finished = false;
                            continue;
                        }

                        if (opRetNode.Value is BranchTargetOperation opBranchTargetModify) {
                            // From this target, find all the branches and replace them with the "ret" instruction
                            if (opBranchTargetModify.Branches != null) {
                                // TODO: Branches might be null if a BranchTargetSetBoolOperation points to this target
                                // via the SkipBranch collection. We should figure out how to change that also to a
                                // "ret", because that's also a useless operation.
                                //
                                // Change the object Branches to be a IOperation? Because a target can't be a branch
                                // simultaneously, unless we start implementing a bunch of interfaces or use other
                                // attributes.
                                foreach (BranchBaseOperation branch in opBranchTargetModify.Branches) {
                                    if (branch is BranchAlwaysOperation alwaysBranch) {
                                        alwaysBranch.BranchTargetSetType = BranchTargetSetType.Return;
                                    }
                                }
                            }
                            finished = false;
                        }
                    } while (!finished);

                    opNode = opNode.Next;
                    continue;
                }

                if (opNode.Value is BranchTargetSetBoolOperation) {
                    opNode = opNode.Next;
                    continue;
                }

                opNode = opNode.Next;
            }
        }

        public void Compile()
        {
            if (m_Compiled) throw new InvalidOperationException("Expression is already compiled");

            // First iterate through and fix any branch references
            foreach (IOperation operation in m_Operations) {
                if (operation is BranchTargetOperation branchTargetOp) {
                    branchTargetOp.SetUpBranches(m_IlGenerator);
                }
            }

            // Generate the IL
            Log.Constraints.TraceEvent(TraceEventType.Information, "** Compiling");
            try {
                foreach (IOperation operation in m_Operations) {
                    Log.Constraints.TraceEvent(TraceEventType.Information, "// Operation: {0}", operation.ToString());
                    operation.Emit(m_IlGenerator);
                }
            } finally {
                m_Compiled = true;
            }
        }

        public bool Evaluate(ITraceLine line)
        {
            m_EvaluationDelegate ??= (EvaluateDelegate)m_EvaluationMethod.CreateDelegate(typeof(EvaluateDelegate));
            return m_EvaluationDelegate(m_ConstraintTable, line);
        }
    }
}
