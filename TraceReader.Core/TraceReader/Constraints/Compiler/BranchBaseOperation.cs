namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System;
    using System.Reflection.Emit;

    /// <summary>
    /// The base class for branch operations to a target.
    /// </summary>
    internal abstract class BranchBaseOperation : IOperation
    {
        private LabelGen m_Target;

        /// <summary>
        /// Gets a value indicating if the target has been set / defined.
        /// </summary>
        /// <value><see langword="true"/> if this instance's target is set; otherwise, <see langword="false"/>.</value>
        public bool IsTargetSet { get { return m_Target is not null; } }

        /// <summary>
        /// Gets or sets the label to branch to.
        /// </summary>
        /// <value>The label to branch to.</value>
        public LabelGen Target
        {
            get
            {
                if (!IsTargetSet) throw new InvalidOperationException("Target is not set");
                return m_Target;
            }
            set { m_Target = value; }
        }

        /// <summary>
        /// The object should emit code based on the state of the object.
        /// </summary>
        /// <param name="ilGen">The IL Generator object to emit code into.</param>
        public abstract void Emit(ILGenerator ilGen);
    }
}
