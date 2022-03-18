namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Always issue a branch.
    /// </summary>
    internal class BranchAlwaysOperation : BranchBaseOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchAlwaysOperation"/> class.
        /// </summary>
        public BranchAlwaysOperation()
        {
            BranchTargetSetType = BranchTargetSetType.Branch;
        }

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

        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public override void Emit(ILGenerator ilGen)
        {
            switch (BranchTargetSetType) {
            case BranchTargetSetType.Branch:
                if (!IsTargetSet) {
                    Target = LabelGenFactory.Create(ilGen);
                }

                Log.Constraints.TraceEvent(TraceEventType.Information, "     br L{0:00}", Target.LabelNum);
                ilGen.Emit(OpCodes.Br, Target.Label);
                break;
            case BranchTargetSetType.Return:
                ilGen.Emit(OpCodes.Ret);
                break;
            default:
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "Branch()";
        }
    }
}
