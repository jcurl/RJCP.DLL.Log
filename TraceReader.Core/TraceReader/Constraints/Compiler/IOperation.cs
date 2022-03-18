namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Reflection.Emit;

    internal interface IOperation
    {
        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        void Emit(ILGenerator ilGen);
    }
}
