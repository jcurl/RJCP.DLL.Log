namespace RJCP.Diagnostics.Log.Constraints
{
    public class Counter : IMatchConstraint
    {
        public bool Check(ITraceLine line)
        {
            Count++;
            return true;
        }

        public int Count { get; private set; }
    }
}
