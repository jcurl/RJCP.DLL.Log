namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Diagnostics;
    using System.Reflection.Emit;

    /// <summary>
    /// Executes the <see cref="IMatchConstraint.Check(ITraceLine)"/> operation as given in an index of an array.
    /// </summary>
    /// <remarks>
    /// The first argument of the generated function (ldarg.0) is the array reference (the jump table) of
    /// <see cref="IMatchConstraint.Check(ITraceLine)"/> objects, the second argument (ldarg.1) is the
    /// <see cref="ITraceLine"/> to pass to the object. This class hard codes the index into the jump table to execute.
    /// </remarks>
    internal class CheckOperation : IOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckOperation"/> class.
        /// </summary>
        /// <param name="jumpTableIndex">Index of the object to call of the given jump table.</param>
        public CheckOperation(int jumpTableIndex)
        {
            JumpTableIndex = jumpTableIndex;
        }

        /// <summary>
        /// Gets the index into the jump table.
        /// </summary>
        /// <value>The index of the object to the jump table to call.</value>
        public int JumpTableIndex { get; }

        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public virtual void Emit(ILGenerator ilGen)
        {
            Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldarg.0");
            ilGen.Emit(OpCodes.Ldarg_0);
            switch (JumpTableIndex) {
            case 0:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.0");
                ilGen.Emit(OpCodes.Ldc_I4_0);
                break;
            case 1:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.1");
                ilGen.Emit(OpCodes.Ldc_I4_1);
                break;
            case 2:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.2");
                ilGen.Emit(OpCodes.Ldc_I4_2);
                break;
            case 3:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.3");
                ilGen.Emit(OpCodes.Ldc_I4_3);
                break;
            case 4:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.4");
                ilGen.Emit(OpCodes.Ldc_I4_4);
                break;
            case 5:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.5");
                ilGen.Emit(OpCodes.Ldc_I4_5);
                break;
            case 6:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.6");
                ilGen.Emit(OpCodes.Ldc_I4_6);
                break;
            case 7:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.7");
                ilGen.Emit(OpCodes.Ldc_I4_7);
                break;
            case 8:
                Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.8");
                ilGen.Emit(OpCodes.Ldc_I4_8);
                break;
            default:
                if (JumpTableIndex < 256) {
                    Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4.s {0}", JumpTableIndex);
                    ilGen.Emit(OpCodes.Ldc_I4_S, (byte)JumpTableIndex);
                } else {
                    Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldc.i4 {0}", JumpTableIndex);
                    ilGen.Emit(OpCodes.Ldc_I4, JumpTableIndex);
                }
                break;
            }
            Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldelem.ref");
            Log.Constraints.TraceEvent(TraceEventType.Verbose, "     ldarg.1");
            Log.Constraints.TraceEvent(TraceEventType.Verbose, "     callvirt IMatchConstraint.Check(ITraceLine)");
            ilGen.Emit(OpCodes.Ldelem_Ref);

            // Evaluate it against "lines"
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Callvirt, typeof(IMatchConstraint).GetMethod("Check", new[] { typeof(ITraceLine) }));
        }

        public override string ToString()
        {
            return string.Format("check({0})", JumpTableIndex);
        }
    }
}
