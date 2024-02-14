namespace RJCP.Diagnostics.Log.Constraints
{
    using System.Collections;
    using System.Collections.Generic;

    internal enum Operation
    {
        Check,
        Or,
        Not,
    }

    internal class Token
    {
        public Token(Operation operation)
        {
            Operation = operation;
            Constraint = null;
        }

        public Token(IMatchConstraint constraint)
        {
            Operation = Operation.Check;
            Constraint = constraint;
        }

        public Operation Operation { get; }

        public IMatchConstraint Constraint { get; }
    }

    internal class ConstraintTokens : IEnumerable<Token>
    {
        private readonly List<Token> m_Constraints = new();

        public void Append(IMatchConstraint constraint)
        {
            // Optimization for None, as we know it will always evaluate true
            if (constraint is None) {
                if (m_Constraints.Count == 0) {
                    // First element. Must add it, so that a Check actually results in true.
                    m_Constraints.Add(new Token(constraint));
                } else {
                    // Ignore, as this won't change the result of the previous element,
                    // unless it is an operator.
                    if (m_Constraints[^1].Operation != Operation.Check) {
                        m_Constraints.Add(new Token(constraint));
                    }
                }
                return;
            }

            if (m_Constraints.Count == 1 && m_Constraints[0].Constraint is None) {
                // The first element is None, so we can replace it with a real element
                m_Constraints.Clear();
            }

            m_Constraints.Add(new Token(constraint));
        }

        public void Or()
        {
            m_Constraints.Add(new Token(Operation.Or));
        }

        public void Not()
        {
            m_Constraints.Add(new Token(Operation.Not));
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return new Enumerator(m_Constraints);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Enumerator : IEnumerator<Token>
        {
            private List<Token> m_List;
            private int m_Index = -1;
            private readonly int m_Count;

            public Enumerator(List<Token> list)
            {
                m_List = list;
                m_Count = m_List.Count;
            }

            public void Reset() { m_Index = -1; }

            public bool MoveNext()
            {
                m_Index++;
                return m_Index < m_Count;
            }

            public Token Current { get { return m_List[m_Index]; } }

            object IEnumerator.Current { get { return Current; } }

            public void Dispose() { m_List = null; }
        }
    }
}
