namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Issues a branch if <see langword="true"/> or branch if <see langword="false"/> opcode.
    /// </summary>
    internal class BranchOperation : BranchAlwaysOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchOperation"/> class.
        /// </summary>
        /// <param name="operation">Specifies the branch condition.</param>
        public BranchOperation(bool operation)
        {
            Operation = operation;
        }

        /// <summary>
        /// Gets or sets a value specifying the branch condition.
        /// </summary>
        /// <value>Branch if <see langword="true"/> or branch if <see langword="false"/>.</value>
        public bool Operation { get; set; }

        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public override void Emit(ILGenerator ilGen)
        {
            if (!IsTargetSet) {
                Target = LabelGenFactory.Create(ilGen);
            }

            if (!Operation) {
                Log.Constraints.TraceEvent(TraceEventType.Information, "     brfalse L{0:00}", Target.LabelNum);
                ilGen.Emit(OpCodes.Brfalse, Target.Label);
            } else {
                Log.Constraints.TraceEvent(TraceEventType.Information, "     brtrue L{0:00}", Target.LabelNum);
                ilGen.Emit(OpCodes.Brtrue, Target.Label);
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Branch({0})", Operation);
        }
    }
}
