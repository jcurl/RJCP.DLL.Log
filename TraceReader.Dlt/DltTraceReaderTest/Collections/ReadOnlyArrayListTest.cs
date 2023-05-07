namespace RJCP.Diagnostics.Log.Collections
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ReadOnlyArrayListTest
    {
        [Test]
        public void NullArray()
        {
            Assert.That(() => {
                _ = new ReadOnlyArrayListAccessor<int>(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Length()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(list.Count, Is.EqualTo(array.Length));
            Assert.That(list.IsReadOnly, Is.True);
        }

        [Test]
        public void GetItems()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[8], Is.EqualTo(9));
        }

        [Test]
        public void SetItems()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(() => {
                list[0] = 0;
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Add()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(() => {
                list.Add(42);
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Clear()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(() => {
                list.Clear();
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Contains()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(list.Contains(1), Is.True);
            Assert.That(list.Contains(42), Is.False);
            Assert.That(list.Contains(0), Is.False);
        }

        [Test]
        public void IndexOf()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(list.IndexOf(1), Is.EqualTo(0));
            Assert.That(list.IndexOf(9), Is.EqualTo(8));
            Assert.That(list.IndexOf(42), Is.EqualTo(-1));
            Assert.That(list.IndexOf(0), Is.EqualTo(-1));
        }

        [Test]
        public void Insert()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(() => {
                list.Insert(0, 42);
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Remove()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(() => {
                list.Remove(5);
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void RemoveAt()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(() => {
                list.RemoveAt(5);
            }, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void CopyTo()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            int[] cpArray = new int[array.Length];
            list.CopyTo(cpArray, 0);

            Assert.That(cpArray, Is.EqualTo(array));
        }

        [Test]
        public void Enumerator()
        {
            int[] array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ReadOnlyArrayListAccessor<int> list = new ReadOnlyArrayListAccessor<int>(array);

            Assert.That(list, Is.EqualTo(array));
        }
    }
}
