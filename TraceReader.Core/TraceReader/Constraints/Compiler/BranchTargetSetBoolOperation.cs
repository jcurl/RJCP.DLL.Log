namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Defines the target for a branch operation, and pushes on the stack <see langword="true"/> or
    /// <see langword="false"/> if we were branched here, else don't modify the stack.
    /// </summary>
    internal class BranchTargetSetBoolOperation : BranchTargetOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchTargetSetBoolOperation"/> class.
        /// </summary>
        /// <param name="value">The value to put on the stack if we were branched here.</param>
        public BranchTargetSetBoolOperation(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchTargetSetBoolOperation"/> class.
        /// </summary>
        /// <param name="value">The value to put on the stack if we were branched here.</param>
        /// <param name="branch">The branch jumping to this target.</param>
        public BranchTargetSetBoolOperation(bool value, BranchBaseOperation branch) : this(value)
        {
            Branches.Add(branch);
        }

        /// <summary>
        /// Gets or sets a value specifying the value to be pushed when we're branched here.
        /// </summary>
        /// <value>The value to push on the stack if branched here.</value>
        public bool Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the branch target set.
        /// </summary>
        /// <value>The type of the branch target set.</value>
        /// <remarks>
        /// Control if entry to this block should just skip over (with a branch), or if we should return from the
        /// routine. Often this block is observed immediately before a "ret" instruction, in which case code can be
        /// generated to return here instead of branching to the "ret" instruction.
        /// </remarks>
        public BranchTargetSetType BranchTargetSetType { get; set; }

        private BranchAlwaysOperation m_SkipBranch;

        /// <summary>
        /// Gets the target we should jump to if this block is entered normally (i.e. it wasn't branched here).
        /// </summary>
        /// <remarks>
        /// Often multiple <see cref="BranchTargetSetBoolOperation"/> occurs in blocks, which results in the first block
        /// branching to the second block. This can be avoided so that the blocks branch instead to after the chain of
        /// targets.
        /// </remarks>
        public BranchTargetOperation SkipBranch { get; set; }

        /// <summary>
        /// Fixes up branches.
        /// </summary>
        /// <param name="ilGen">The IL Generator.</param>
        /// <remarks>
        /// Ensure that all branches that target here have the same value. This needs to be called before emitting code.
        /// </remarks>
        public override void SetUpBranches(ILGenerator ilGen)
        {
            base.SetUpBranches(ilGen);

            if (SkipBranch is not null) {
                m_SkipBranch = new BranchAlwaysOperation();
                SkipBranch.Branches.Add(m_SkipBranch);

                if (Target is null) {
                    Target = LabelGenFactory.Create(ilGen);
                    m_SkipBranch.Target = Target;
                }
            }
        }

        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public override void Emit(ILGenerator ilGen)
        {
            if (Target is null) throw new InvalidOperationException("SetUpBranches must be called first");

            switch (BranchTargetSetType) {
            case BranchTargetSetType.Branch:
                if (SkipBranch is null) {
                    EmitLocalBranch(ilGen);
                } else {
                    EmitSkipBranch(ilGen);
                }
                break;
            case BranchTargetSetType.Return:
                EmitRet(ilGen);
                break;
            default:
                throw new InvalidOperationException("Unknown BranchTargetSetType");
            }
        }

        private void EmitLocalBranch(ILGenerator ilGen)
        {
            LabelGen skip = LabelGenFactory.Create(ilGen);

            Log.Constraints.TraceEvent(TraceEventType.Information, "     br.s L{0:00}", skip.LabelNum);
            ilGen.Emit(OpCodes.Br_S, skip.Label);
            base.Emit(ilGen);
            EmitLoad(ilGen);

            Log.Constraints.TraceEvent(TraceEventType.Information, "L{0:00}: ", skip.LabelNum);
            ilGen.MarkLabel(skip.Label);
        }

        private void EmitSkipBranch(ILGenerator ilGen)
        {
            m_SkipBranch.Emit(ilGen);
            base.Emit(ilGen);
            EmitLoad(ilGen);
        }

        private void EmitRet(ILGenerator ilGen)
        {
            Log.Constraints.TraceEvent(TraceEventType.Information, "     ret");
            ilGen.Emit(OpCodes.Ret);
            base.Emit(ilGen);
            EmitLoad(ilGen);
        }

        private void EmitLoad(ILGenerator ilGen)
        {
            if (!Value) {
                Log.Constraints.TraceEvent(TraceEventType.Information, "     ldc.i4.0");
                ilGen.Emit(OpCodes.Ldc_I4_0);
            } else {
                Log.Constraints.TraceEvent(TraceEventType.Information, "     ldc.i4.1");
                ilGen.Emit(OpCodes.Ldc_I4_1);
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("BranchTargetSet({0})", Value);
        }
    }
}
