namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    /// <summary>
    /// Checks that <see cref="ITraceLine.Text"/> contains the string specified.
    /// </summary>
    public sealed class TextString : IMatchConstraint
    {
        private readonly string m_Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextString"/> class.
        /// </summary>
        /// <param name="text">The text which should be a substring.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        public TextString(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            m_Text = text;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return line.Text.Contains(m_Text);
        }
    }
}
