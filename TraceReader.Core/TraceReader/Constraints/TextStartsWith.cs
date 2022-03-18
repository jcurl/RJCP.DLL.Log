namespace RJCP.Diagnostics.Log.Constraints
{
    using System;

    /// <summary>
    /// Checks that <see cref="ITraceLine.Text"/> starts with the string specified.
    /// </summary>
    public sealed class TextStartsWith : IMatchConstraint
    {
        private readonly string m_Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextStartsWith"/> class.
        /// </summary>
        /// <param name="text">The text which the line should start with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        public TextStartsWith(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            m_Text = text;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return line.Text.StartsWith(m_Text, StringComparison.Ordinal);
        }
    }
}
