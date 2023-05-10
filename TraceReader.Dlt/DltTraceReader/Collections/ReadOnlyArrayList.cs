namespace RJCP.Diagnostics.Log.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A read-only list wrapper around an array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    internal sealed class ReadOnlyArrayList<T> : IList<T>
    {
        private readonly T[] m_Array;

        public ReadOnlyArrayList(T[] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            m_Array = array;
        }

        public T this[int index]
        {
            get { return m_Array[index]; }
            set
            {
                throw new NotSupportedException("List is read only");
            }
        }

        public int Count { get { return m_Array.Length; } }

        public bool IsReadOnly { get { return true; } }

        public void Add(T item)
        {
            throw new NotSupportedException("List is read only");
        }

        public void Clear()
        {
            throw new NotSupportedException("List is read only");
        }

        public bool Contains(T item)
        {
            return m_Array.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Array.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < m_Array.Length; i++) {
                if (m_Array[i].Equals(item)) return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("List is read only");
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("List is read only");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("List is read only");
        }

        private struct ArrayEnumerator : IEnumerator<T>
        {
            private readonly T[] m_Array;
            private int m_Element;

            public ArrayEnumerator(T[] array)
            {
                m_Array = array;
                m_Element = -1;
            }

            public T Current { get { return m_Array[m_Element]; } }

            object IEnumerator.Current { get { return m_Array[m_Element]; } }

            public bool MoveNext()
            {
                m_Element++;
                return m_Element < m_Array.Length;
            }

            public void Reset()
            {
                m_Element = -1;
            }

            public void Dispose()
            {
                /* Nothing to dispose */
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator(m_Array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Array.GetEnumerator();
        }
    }
}
