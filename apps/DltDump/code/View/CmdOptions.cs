namespace RJCP.App.DltDump.View
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Infrastructure.Dlt;
    using Resources;
    using RJCP.Core.CommandLine;
    using RJCP.Diagnostics.Log.Dlt;

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

        [Option("type")]
        private readonly List<string> m_DltTypeFilters = new List<string>();

        private readonly List<DltType> m_DltType = new List<DltType>();

        /// <summary>
        /// Gets the list of DLT filter types to filter for.
        /// </summary>
        /// <value>The DLT type filters to filter for.</value>
        public IReadOnlyList<DltType> DltTypeFilters { get { return m_DltType; } }

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
        /// Gets a value indicating if all messages should be discarded.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if all messages should be discarded; ptoerwise <see langword="false"/>.
        /// </value>
        [Option("none")]
        public bool None { get; private set; }

        /// <summary>
        /// Gets the number of lines to print before a match.
        /// </summary>
        /// <value>The number of lines to print before a match.</value>
        [Option('B', "before-context")]
        public int BeforeContext { get; private set; }

        /// <summary>
        /// Gets the number of lines to print after a match.
        /// </summary>
        /// <value>The number of lines to print after a match.</value>
        [Option('A', "after-context")]
        public int AfterContext { get; private set; }

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

            if (BeforeContext < 0)
                throw new OptionFormatException("before-context");
            if (AfterContext < 0)
                throw new OptionFormatException("after-context");

            try {
                // Interpret the strings and convert to the correct DltType.
                foreach (string dltTypeFilter in m_DltTypeFilters) {
                    m_DltType.Add(GetDltType(dltTypeFilter));
                }
            } catch (ArgumentException ex) {
                throw new OptionFormatException("type", ex.Message, ex);
            }
        }

        private static DltType GetDltType(string dltTypeFilter)
        {
            if (int.TryParse(dltTypeFilter,
                    NumberStyles.Number, CultureInfo.InvariantCulture,
                    out int value)) {
                return (DltType)value;
            }

            if (!Enum.TryParse<DltTypeFilter>(dltTypeFilter, true, out DltTypeFilter typeFilter)) {
                string msg = string.Format(AppResources.OptionInvalidFilterType, dltTypeFilter);
                throw new ArgumentException(msg, nameof(dltTypeFilter));
            }

            switch (typeFilter) {
            case DltTypeFilter.Fatal: return DltType.LOG_FATAL;
            case DltTypeFilter.Error: return DltType.LOG_ERROR;
            case DltTypeFilter.Warn: return DltType.LOG_WARN;
            case DltTypeFilter.Info: return DltType.LOG_INFO;
            case DltTypeFilter.Debug: return DltType.LOG_DEBUG;
            case DltTypeFilter.Verbose: return DltType.LOG_VERBOSE;
            case DltTypeFilter.IPC: return DltType.NW_TRACE_IPC;
            case DltTypeFilter.CAN: return DltType.NW_TRACE_CAN;
            case DltTypeFilter.FlexRay: return DltType.NW_TRACE_FLEXRAY;
            case DltTypeFilter.MOST: return DltType.NW_TRACE_MOST;
            case DltTypeFilter.Ethernet: return DltType.NW_TRACE_ETHERNET;
            case DltTypeFilter.SomeIP: return DltType.NW_TRACE_SOMEIP;
            case DltTypeFilter.User1: return DltType.NW_TRACE_USER_DEFINED_0;
            case DltTypeFilter.User2: return DltType.NW_TRACE_USER_DEFINED_1;
            case DltTypeFilter.User3: return DltType.NW_TRACE_USER_DEFINED_2;
            case DltTypeFilter.User4: return DltType.NW_TRACE_USER_DEFINED_3;
            case DltTypeFilter.User5: return DltType.NW_TRACE_USER_DEFINED_4;
            case DltTypeFilter.User6: return DltType.NW_TRACE_USER_DEFINED_5;
            case DltTypeFilter.User7: return DltType.NW_TRACE_USER_DEFINED_6;
            case DltTypeFilter.User8: return DltType.NW_TRACE_USER_DEFINED_7;
            case DltTypeFilter.User9: return DltType.NW_TRACE_USER_DEFINED_8;
            case DltTypeFilter.Request: return DltType.CONTROL_REQUEST;
            case DltTypeFilter.Response: return DltType.CONTROL_RESPONSE;
            case DltTypeFilter.Time: return DltType.CONTROL_TIME;
            case DltTypeFilter.Variable: return DltType.APP_TRACE_VARIABLE;
            case DltTypeFilter.FunctionIn: return DltType.APP_TRACE_FUNCTION_IN;
            case DltTypeFilter.FunctionOut: return DltType.APP_TRACE_FUNCTION_OUT;
            case DltTypeFilter.State: return DltType.APP_TRACE_STATE;
            case DltTypeFilter.VFB: return DltType.APP_TRACE_VFB;
            default:
                string msg = string.Format(AppResources.OptionInvalidFilterType, dltTypeFilter);
                throw new ArgumentException(msg, nameof(dltTypeFilter));
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
