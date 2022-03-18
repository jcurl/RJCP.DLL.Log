namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Returns from the current function.
    /// </summary>
    internal sealed class ReturnOperation : IOperation
    {
        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public void Emit(ILGenerator ilGen)
        {
            Log.Constraints.TraceEvent(TraceEventType.Information, "     ret");
            ilGen.Emit(OpCodes.Ret);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
