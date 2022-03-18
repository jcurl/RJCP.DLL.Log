namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Defines the target for a branch operation.
    /// </summary>
    internal class BranchTargetOperation : IOperation
    {
        private readonly List<BranchBaseOperation> m_Branches = new List<BranchBaseOperation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchTargetOperation"/> class.
        /// </summary>
        public BranchTargetOperation() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchTargetOperation"/> class.
        /// </summary>
        /// <param name="branch">The branch.</param>
        public BranchTargetOperation(BranchBaseOperation branch)
        {
            Branches.Add(branch);
        }

        /// <summary>
        /// Gets the list of branches that branch to this target.
        /// </summary>
        /// <value>The branches that target to this location.</value>
        public IList<BranchBaseOperation> Branches { get { return m_Branches; } }

        /// <summary>
        /// Gets the label for the target to branch to.
        /// </summary>
        /// <value>The label for the target to branch to.</value>
        protected LabelGen Target { get; set; }

        /// <summary>
        /// Fixes up branches.
        /// </summary>
        /// <param name="ilGen">The IL Generator.</param>
        /// <exception cref="InvalidOperationException">
        /// SetUpBranches has already been called.
        /// <para>- or -</para>
        /// Target branches not set up correctly.
        /// </exception>
        /// <remarks>
        /// Ensure that all branches that target here have the same value. This needs to be called before emitting code.
        /// </remarks>
        public virtual void SetUpBranches(ILGenerator ilGen)
        {
            if (Target != null) throw new InvalidOperationException("SetUpBranches has already been called");

            bool finished;
            do {
                finished = true;
                foreach (BranchBaseOperation branch in Branches) {
                    if (Target == null) {
                        if (branch.IsTargetSet) {
                            Target = branch.Target;
                        } else {
                            finished = false;
                        }
                    } else {
                        if (branch.IsTargetSet) {
                            if (!branch.Target.Equals(Target)) {
                                throw new InvalidOperationException("Target branches not set up correctly");
                            }
                        } else {
                            branch.Target = Target;
                        }
                    }
                }
                if (Target == null) {
                    Target = LabelGenFactory.Create(ilGen);
                    finished = false;
                }
            } while (!finished);
        }

        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public virtual void Emit(ILGenerator ilGen)
        {
            if (Target == null) throw new InvalidOperationException("SetUpBranches must be called first");

            Log.Constraints.TraceEvent(TraceEventType.Information, "L{0:00}: ", Target.LabelNum);
            ilGen.MarkLabel(Target.Label);
        }

        public override string ToString()
        {
            return "BranchTarget";
        }
    }
}
