namespace RJCP.App.DltDump.Infrastructure.Terminal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class to be used for testing purposes only.
    /// </summary>
    /// <remarks>
    /// All write related methods populate the publicly available <see cref="Lines"/> collection, which can be used to
    /// either check the actual contents of the lines being written, or one can use the <see
    /// cref="List{T}.Count"/> to verify the number of lines that were written.
    /// </remarks>
    public class VirtualOutBase : ITerminalOut
    {
        private enum LineState
        {
            None,
            LineFeed
        }

        private bool m_NewLine = true;
        private LineState m_LineState;
        private readonly List<string> m_Lines = new List<string>();

        /// <summary>
        /// Gets the lines that were written.
        /// </summary>
        /// <value>The lines that were written.</value>
        /// <remarks>
        /// Since this is expected to be used only for the purpose of testing, populating the <see cref="Lines"/>
        /// collection is not expected to generate any high memory usage during unit testing.
        /// </remarks>
        public virtual List<string> Lines { get { return m_Lines; } }

        /// <summary>
        /// Writes the line to the <see cref="Lines" /> collection.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> may not be <see langword="null"/>.</exception>
        public virtual void Write(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            SplitLines(line);
        }

        /// <summary>
        /// Writes the line of the specified format to the <see cref="Lines"/> collection.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid;
        /// <para>- or -</para>
        /// The index of a format item is less than zero, or greater than or equal to the length of the
        /// <paramref name="args"/> array.
        /// </exception>
        public virtual void Write(string format, params object[] args)
        {
            string line = string.Format(format, args);
            Write(line);
        }

        /// <summary>
        /// Writes the line to the <see cref="Lines"/> collection.
        /// </summary>
        /// <param name="line">The line to be written.</param>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> may not be <see langword="null"/>.</exception>
        public virtual void WriteLine(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            SplitLines(line);
            NewLine();
        }

        /// <summary>
        /// Writes the line of the specified format to the <see cref="Lines"/> collection.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid;
        /// <para>- or -</para>
        /// The index of a format item is less than zero, or greater than or equal to the length of the
        /// <paramref name="args"/> array.
        /// </exception>
        public virtual void WriteLine(string format, params object[] args)
        {
            string line = string.Format(format, args);
            WriteLine(line);
        }

        private void SplitLines(string line)
        {
            int offset = 0;
            int length = line.Length;
            while (offset < length) {
                int charPos = line.IndexOfAny(new[] { '\r', '\n' }, offset);
                if (charPos == -1) {
                    AddLine(line, offset, length - offset);
                    m_LineState = LineState.None;
                    m_NewLine = false;
                    return;
                } else {
                    int c = line[charPos];
                    if (m_LineState == LineState.LineFeed && charPos == offset && c == '\n') {
                        // If a \n is followed immediately by a \r, then ignore it, as this is the Windows new line
                        // sequence.
                        m_LineState = LineState.None;
                    } else {
                        AddLine(line, offset, charPos - offset);
                        m_NewLine = true;
                        m_LineState = (c == '\r') ? LineState.LineFeed : LineState.None;
                    }
                    offset = charPos + 1;
                }
            }
        }

        private void AddLine(string line, int offset, int length)
        {
            if (m_NewLine) {
                m_Lines.Add(line.Substring(offset, length));
            } else {
                m_Lines[^1] += line.Substring(offset, length);
            }
        }

        private void NewLine()
        {
            if (m_NewLine) {
                m_Lines.Add(string.Empty);
            }
            m_LineState = LineState.None;
            m_NewLine = true;
        }
    }
}
