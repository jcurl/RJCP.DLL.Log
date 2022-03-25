﻿namespace RJCP.App.DltDump.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;

    /// <summary>
    /// Maintains context for lines being traced.
    /// </summary>
    /// <remarks>
    /// When checking context:
    /// <list type="bullet">
    /// <item>Initialize with the number of lines of before context and after context.</item>
    /// <item>
    /// <see cref="Check"/> the line. If it is <see langword="true"/> there was a match, and before context should first
    /// be printed.
    /// </item>
    /// <item>
    /// If <see cref="Check"/> was <see langword="true"/>, then get the context with <see cref="GetBeforeContext()"/>.
    /// This should be called before any further call to <see cref="Check"/> as the enumerator operates on the buffer
    /// and may result in incorrect data if a call to <see cref="Check"/> is given before enumerating the list.
    /// </item>
    /// <item>
    /// If the result of <see cref="Check"/> is <see langword="false"/>, a check should be made to
    /// <see cref="IsAfterContext()"/> if the line should be printed.
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class Context
    {
        private readonly int m_BeforeContext;
        private readonly int m_AfterContext;
        private readonly DltTraceLineBase[] m_Buffer;
        private readonly Constraint m_Filter;
        private int m_BufferStart;
        private int m_BufferLength;
        private int m_AfterContextLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="filter">The filter that is used to define the context point.</param>
        /// <param name="before">The context that should be maintained before a match.</param>
        /// <param name="after">The context that should be maintained after a match.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filter"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="before"/> or <paramref name="after"/> are negative.
        /// </exception>
        public Context(Constraint filter, int before, int after)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (before < 0)
                throw new ArgumentOutOfRangeException(nameof(before));
            if (after < 0)
                throw new ArgumentOutOfRangeException(nameof(after));

            m_Filter = filter;
            m_BeforeContext = before;
            m_AfterContext = after;
            m_Buffer = new DltTraceLineBase[before];
        }

        /// <summary>
        /// Checks the specified line if there is a match.
        /// </summary>
        /// <param name="line">The line that should be checked against the filter.</param>
        /// <returns>Is <see langword="true"/> if the filter matches, <see langword="false"/> otherwise.</returns>
        public bool Check(DltTraceLineBase line)
        {
            if (m_Filter.Check(line)) {
                m_AfterContextLength = m_AfterContext;
                return true;
            }

            if (m_BeforeContext > 0 && m_AfterContextLength == 0) {
                int p = (m_BufferStart + m_BufferLength) % m_BeforeContext;
                m_Buffer[p] = line;
                if (m_BufferLength == m_BeforeContext) {
                    m_BufferStart = (m_BufferStart + 1) % m_BeforeContext;
                } else {
                    m_BufferLength++;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets an enumerable object to iterate over the context before the current match.
        /// </summary>
        /// <returns>An enumerable object to iterate over the context before the current match..</returns>
        public IEnumerable<DltTraceLineBase> GetBeforeContext()
        {
            if (m_BufferLength == 0) return EmptyEnumerable;
            IEnumerable<DltTraceLineBase> result = new BeforeContext(m_Buffer, m_BufferStart, m_BufferLength);
            m_BufferStart = 0;
            m_BufferLength = 0;
            return result;
        }

        /// <summary>
        /// Determines if lines should be printed after a context match.
        /// </summary>
        /// <returns>
        /// Is <see langword="true"/> if lines should be printed after the last match; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool IsAfterContext()
        {
            if (m_AfterContextLength > 0) {
                --m_AfterContextLength;
                return true;
            }

            return false;
        }

        private static readonly EmptyContext EmptyEnumerable = new EmptyContext();

        private sealed class EmptyContext : IEnumerable<DltTraceLineBase>
        {
            private static readonly EmptyContextEnumerator EmptyEnumerator = new EmptyContextEnumerator();

            public IEnumerator<DltTraceLineBase> GetEnumerator()
            {
                return EmptyEnumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private sealed class EmptyContextEnumerator : IEnumerator<DltTraceLineBase>
            {
                public DltTraceLineBase Current { get { return null; } }

                object IEnumerator.Current { get { return Current; } }

                public bool MoveNext() { return false; }

                public void Reset() { /* nothing to do */ }

                public void Dispose() { /* nothing to do */ }
            }
        }

        private sealed class BeforeContext : IEnumerable<DltTraceLineBase>
        {
            private readonly DltTraceLineBase[] m_Buffer;
            private readonly int m_BufferStart;
            private readonly int m_BufferLength;

            public BeforeContext(DltTraceLineBase[] buffer, int start, int length)
            {
                m_Buffer = buffer;
                m_BufferStart = start;
                m_BufferLength = length;
            }

            public IEnumerator<DltTraceLineBase> GetEnumerator()
            {
                return new BeforeContextEnumerator(m_Buffer, m_BufferStart, m_BufferLength);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private sealed class BeforeContextEnumerator : IEnumerator<DltTraceLineBase>
            {
                private readonly DltTraceLineBase[] m_Buffer;
                private readonly int m_BufferStart;
                private readonly int m_BufferLength;
                private int m_Index;

                public BeforeContextEnumerator(DltTraceLineBase[] buffer, int start, int length)
                {
                    m_Buffer = buffer;
                    m_BufferStart = start;
                    m_BufferLength = length;
                }

                public DltTraceLineBase Current { get; private set; }

                object IEnumerator.Current { get { return Current; } }

                public bool MoveNext()
                {
                    if (m_Index == m_BufferLength) return false;
                    Current = m_Buffer[(m_BufferStart + m_Index) % m_BufferLength];
                    m_Index++;
                    return true;
                }

                public void Reset() { /* nothing to do */ }

                public void Dispose() { /* nothing to do */ }
            }
        }
    }
}