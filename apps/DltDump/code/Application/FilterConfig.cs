namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Configuration for the <see cref="FilterApp"/>.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterConfig"/> class.
        /// </summary>
        /// <param name="input">The list of inputs.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="input"/> is <see langword="null"/>.</exception>
        public FilterConfig(IReadOnlyList<string> input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Input = input;
        }

        /// <summary>
        /// Gets the input streams for parsing.
        /// </summary>
        /// <value>The input streams for parsing.</value>
        public IReadOnlyList<string> Input { get; }

        /// <summary>
        /// Print the position when outputting in text mode (console or file).
        /// </summary>
        /// <value>Shows the position when <see langword="true"/>; otherwise, <see langword="false"/>.</value>
        public bool ShowPosition { get; set; }
    }
}
