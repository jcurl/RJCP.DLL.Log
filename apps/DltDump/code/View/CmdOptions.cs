namespace RJCP.App.DltDump.View
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Infrastructure.Dlt;
    using RJCP.Core.CommandLine;

    /// <summary>
    /// Defines the options available for DltDump.
    /// </summary>
    public class CmdOptions : IOptions
    {
        /// <summary>
        /// Gets a value indicating if the help should be shown.
        /// </summary>
        /// <value>Is <see langword="true"/> if help should be printed; otherwise,  <see langword="false"/>.</value>
        [Option('?', "help")]
        public bool Help { get; private set; }

        /// <summary>
        /// Gets a value indicating if the version should be shown.
        /// </summary>
        /// <value>Is <see langword="true"/> if version should be printed; otherwise, <see langword="false"/>.</value>
        [Option("version")]
        public bool Version { get; private set; }

        /// <summary>
        /// Gets a value indicating if a crash log should be generated when complete.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if a log file should be generated; otherwise, <see langword="false"/>.
        /// </value>
        [Option("log")]
        public bool Log { get; private set; }

#if DEBUG
        /// <summary>
        /// Gets a value indicating if a crash test should be performed.
        /// </summary>
        /// <value>A crash test should be performed if <see langword="true"/>; otherwise, <see langword="false"/>.</value>
        [Option("crashtest")]
        public bool CrashTest { get; private set; }
#endif

        /// <summary>
        /// Gets a value if the position for each line should be shown on the console.
        /// </summary>
        /// <value>Show the position if <see langword="true"/>; otherwise, <see langword="false"/>.</value>
        [Option("position")]
        public bool Position { get; private set; }

        [OptionArguments]
        private readonly List<string> m_Arguments = new List<string>();

        [Option("format")]
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set by reflection")]
#pragma warning disable CS0649
        // This is set by reflection
        private string m_InputFormat;
#pragma warning restore CS0649

        /// <summary>
        /// Gets the input format.
        /// </summary>
        /// <value>The input format.</value>
        public InputFormat InputFormat { get; private set; }

        /// <summary>
        /// Gets the connect retries for input connection types that need a connection.
        /// </summary>
        /// <value>The connect retries.</value>
        [Option("retries")]
        public int ConnectRetries { get; private set; }

        /// <summary>
        /// Gets the search string.
        /// </summary>
        /// <value>The search string.</value>
        [Option('s', "string")]
        private readonly List<string> m_SearchString = new List<string>();

        public IReadOnlyList<string> SearchString { get { return m_SearchString; } }

        /// <summary>
        /// Gets the search regular expression.
        /// </summary>
        /// <value>The search regular expression.</value>
        [Option('r', "regex")]
        private readonly List<string> m_SearchRegex = new List<string>();

        public IReadOnlyList<string> SearchRegex { get { return m_SearchRegex; } }

        /// <summary>
        /// Gets a value indicating whether case should be ignored when searching
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if case should be ignored when searching; otherwise, <see langword="false"/>.
        /// </value>
        [Option('i', "ignorecase")]
        public bool IgnoreCase { get; private set; }

        [Option("ecuid")]
        private readonly List<string> m_EcuId = new List<string>();

        /// <summary>
        /// A list of ECU IDs that should match.
        /// </summary>
        public IReadOnlyList<string> EcuId { get { return m_EcuId; } }

        [Option("appid")]
        private readonly List<string> m_AppId = new List<string>();

        /// <summary>
        /// A list of Application IDs that should match.
        /// </summary>
        public IReadOnlyList<string> AppId { get { return m_AppId; } }

        [Option("ctxid")]
        private readonly List<string> m_CtxId = new List<string>();

        /// <summary>
        /// A list of Context IDs that should match.
        /// </summary>
        public IReadOnlyList<string> CtxId { get { return m_CtxId; } }

        [Option("sessionid")]
        private readonly List<int> m_SessionId = new List<int>();

        /// <summary>
        /// A list of Session IDs that should match for lines that have a session identifier.
        /// </summary>
        public IReadOnlyList<int> SessionId { get { return m_SessionId; } }

        /// <summary>
        /// Gets a value indicating whether verbose messages should be filtered.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if verbose messages should be filtered; otherwise, <see langword="false"/>.
        /// </value>
        [Option("verbose")]
        public bool VerboseMessage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether verbose messages should be filtered.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if non-verbose messages should be filtered; otherwise, <see langword="false"/>.
        /// This does not include control messages.
        /// </value>
        [Option("nonverbose")]
        public bool NonVerboseMessage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether verbose messages should be filtered.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if control messages should be filtered; otherwise, <see langword="false"/>. This
        /// does not include control messages.
        /// </value>
        [Option("control")]
        public bool ControlMessage { get; private set; }

        /// <summary>
        /// Gets the list of arguments which are inputs for DLT streams.
        /// </summary>
        /// <value>The input DLT streams to read.</value>
        public IReadOnlyList<string> Arguments { get { return m_Arguments; } }

        public void Check()
        {
            if (string.IsNullOrEmpty(m_InputFormat)) {
                InputFormat = InputFormat.Automatic;
            } else if (Enum.TryParse(m_InputFormat, true, out InputFormat inputFormat)) {
                InputFormat = inputFormat;
            } else {
                switch (m_InputFormat.ToLowerInvariant()) {
                case "auto":
                    InputFormat = InputFormat.Automatic;
                    break;
                case "net":
                    InputFormat = InputFormat.Network;
                    break;
                case "ser":
                    InputFormat = InputFormat.Serial;
                    break;
                default:
                    throw new OptionFormatException("format");
                }
            }
        }

        public void Usage()
        {
            // The application will handle this.
        }

        public void Missing(IList<string> missingOptions)
        {
            // The application will handle this.
        }

        public void InvalidOption(string option)
        {
            // The application will handle this.
        }
    }
}
