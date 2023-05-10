namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Checks that <see cref="ITraceLine.Text"/> satisfies a regular expression.
    /// </summary>
    public sealed class TextIRegEx : IMatchConstraint
    {
        private readonly Regex m_RegEx;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextIRegEx"/> class.
        /// </summary>
        /// <param name="regEx">The regular expression which should match the <see cref="ITraceLine.Text"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="regEx"/> is <see langword="null"/>.</exception>
        public TextIRegEx(string regEx)
        {
            if (regEx is null) throw new ArgumentNullException(nameof(regEx));
            m_RegEx = new Regex(regEx, RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextIRegEx"/> class.
        /// </summary>
        /// <param name="regEx">The regular expression which should match the <see cref="ITraceLine.Text"/>.</param>
        /// <param name="options">The options to apply to the regular expression.</param>
        /// <exception cref="ArgumentNullException"><paramref name="regEx"/> is <see langword="null"/>.</exception>
        public TextIRegEx(string regEx, RegexOptions options)
        {
            if (regEx is null) throw new ArgumentNullException(nameof(regEx));
            m_RegEx = new Regex(regEx, options | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        public bool Check(ITraceLine line)
        {
            return m_RegEx.IsMatch(line.Text);
        }
    }
}
