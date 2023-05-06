namespace RJCP.Diagnostics.Log.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using RJCP.CodeQuality;

    public class ReadOnlyArrayListAccessor<T> : AccessorBase, IList<T>
    {
        private const string AssemblyName = "RJCP.Diagnostics.Log.Dlt";
        private const string TypeName = "RJCP.Diagnostics.Log.Collections.ReadOnlyArrayList`1";
        private static readonly PrivateType AccType =
            new PrivateType(AssemblyName, TypeName, new Type[] { typeof(T) });

        public ReadOnlyArrayListAccessor(T[] array)
            : base(AccType, new Type[] { typeof(T[]) }, new object[] { array }) { }

        public T this[int index]
        {
            get { return GetProperty<T>("Item", index); }
            set { SetProperty("Item", value, index); }
        }

        public int Count { get { return GetProperty<int>(nameof(Count)); } }

        public bool IsReadOnly { get { return GetProperty<bool>(nameof(IsReadOnly)); } }

        public void Add(T item)
        {
            Invoke(nameof(Add), new Type[] { typeof(T) }, new object[] { item });
        }

        public void Clear()
        {
            Invoke(nameof(Clear));
        }

        public bool Contains(T item)
        {
            return (bool)Invoke(nameof(Contains), new Type[] { typeof(T) }, new object[] { item });
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Invoke(nameof(CopyTo), new Type[] { typeof(T[]), typeof(int) }, new object[] { array, arrayIndex });
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)Invoke(nameof(GetEnumerator));
        }

        public int IndexOf(T item)
        {
            return (int)Invoke(nameof(IndexOf), new Type[] { typeof(T) }, new object[] { item });
        }

        public void Insert(int index, T item)
        {
            Invoke(nameof(Insert), new Type[] { typeof(int), typeof(T) }, new object[] { index, item });
        }

        public bool Remove(T item)
        {
            return (bool)Invoke(nameof(Remove), new Type[] { typeof(T) }, new object[] { item });
        }

        public void RemoveAt(int index)
        {
            Invoke(nameof(RemoveAt), index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)Invoke(nameof(GetEnumerator));
        }
    }
}
