namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    public class InvalidConstraint : IMatchConstraint
    {
        public bool Check(ITraceLine line)
        {
            throw new InvalidOperationException();
        }
    }
}
