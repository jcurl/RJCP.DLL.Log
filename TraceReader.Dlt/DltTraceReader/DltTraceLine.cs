namespace RJCP.Diagnostics.Log
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Dlt;
    using Dlt.Args;

    /// <summary>
    /// Representation of a single trace line in the DLT protocol.
    /// </summary>
    public class DltTraceLine : DltTraceLineBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceLine"/> class.
        /// </summary>
        public DltTraceLine()
        {
            Arguments = new List<IDltArg>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DltTraceLine"/> class.
        /// </summary>
        /// <param name="arguments">The payload argument collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/> may not be <see langword="null"/>.</exception>
        public DltTraceLine(IEnumerable<IDltArg> arguments)
        {
            Arguments = new List<IDltArg>(arguments);
        }

        /// <summary>
        /// This is the decoded line of text with time stamp and other metadata stripped.
        /// </summary>
        /// <value>
        /// This is a string, that should normally present the text log that was emitted by the device being logged.
        /// That is, all metadata, such as timestamps, severity and other information is not present in this string. The
        /// DLT trace line only contains the concatenation of all the arguments. Feature fields and other properties are
        /// not represented here, but by <see cref="ToString"/>.
        /// </value>
        /// <remarks>
        /// The text is generated lazily, at the time it is queried, and the result is then stored. Thus changes to this
        /// object will not automatically update the <see cref="Text"/> property. Writing to this property will override
        /// the value calculated. If the text should be automatically recalculated, set this property the value of
        /// <see langword="null"/>.
        /// </remarks>
        public override string Text
        {
            get
            {
                if (base.Text == null) {
                    base.Text = BuildArgumentText();
                }
                return base.Text;
            }
            set { base.Text = value; }
        }

        /// <summary>
        /// Gets the arguments as a non-modifiable list.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<IDltArg> Arguments { get; protected set; }

        private string BuildArgumentText()
        {
            StringBuilder strBuilder = new StringBuilder();
            if (Arguments.Count == 0) return string.Empty;

            Arguments[0].Append(strBuilder);
            for (int i = 1; i < Arguments.Count; i++) {
                strBuilder.Append(' ');
                Arguments[i].Append(strBuilder);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        /// <remarks>
        /// Shall return a string containing the values of the DLT line fields, similar to a DLT trace line, exported to
        /// a text file using DLT Viewer 2.18.0. This can be used to assign to the <see cref="DltTraceLineBase.Text"/>
        /// property.
        /// </remarks>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", base.ToString(),
                Features.IsVerbose ? "verbose" : "non-verbose", BuildArgumentText());
        }
    }
}
