namespace RJCP.Diagnostics.Log.Constraints
{
    public class Position : IMatchConstraint
    {
        private readonly int m_Position;

        public Position(int position)
        {
            m_Position = position;
        }

        public bool Check(ITraceLine line)
        {
            return line.Position == m_Position;
        }
    }
}
