namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    /// <summary>
    /// Checks that <see cref="ITraceLine.Text"/> contains the string specified.
    /// </summary>
    public sealed class TextIString : IMatchConstraint
    {
        private readonly string m_Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextString"/> class.
        /// </summary>
        /// <param name="text">The text which should be a substring.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        public TextIString(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            m_Text = text.ToUpperInvariant();
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return line.Text.ToUpperInvariant().Contains(m_Text);
        }
    }
}
