namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    internal class BNode
    {
        public BNode(BOperation operation)
        {
            Operation = operation;
        }

        public BNode(IMatchConstraint constraint)
        {
            Operation = BOperation.Check;
            Function = constraint;
        }

        public BOperation Operation { get; }

        public IMatchConstraint Function { get; }

        public int Id { get; set; }

        public override string ToString()
        {
            if (Function is object) {
                return string.Format("{0}: {1}", Operation, Function);
            } else {
                return string.Format("{0}: --", Operation);
            }
        }
    }
}
