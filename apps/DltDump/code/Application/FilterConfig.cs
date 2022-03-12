namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Dlt;

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

        /// <summary>
        /// Gets or sets the input format for reading the input stream.
        /// </summary>
        /// <value>The input format for reading the input stream.</value>
        public InputFormat InputFormat { get; set; } = InputFormat.Automatic;

        /// <summary>
        /// Gets or sets the connect retries for input stream types that require a connection.
        /// </summary>
        /// <value>The number of connect retries.</value>
        public int ConnectRetries { get; set; } = 0;
    }
}
