namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Inverts the boolean value on the stack.
    /// </summary>
    internal class InvertOperation : IOperation
    {
        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public virtual void Emit(ILGenerator ilGen)
        {
            Log.Constraints.TraceEvent(TraceEventType.Information, "     ldc.i4.1");
            ilGen.Emit(OpCodes.Ldc_I4_1);

            Log.Constraints.TraceEvent(TraceEventType.Information, "     xor");
            ilGen.Emit(OpCodes.Xor);
        }

        public override string ToString()
        {
            return "NOT";
        }
    }
}
