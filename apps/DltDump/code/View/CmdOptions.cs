namespace RJCP.App.DltDump.View
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using RJCP.Core.CommandLine;

    /// <summary>
    /// Defines the options available for DltDump.
    /// </summary>
    /// <remarks>
    /// This class doesn't derive from <see cref="IOptions"/>, preferring instead that the main application just handles
    /// the exceptions.
    /// </remarks>
    public class CmdOptions
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
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Set by reflection")]
        private List<string> m_Arguments = new List<string>();

        /// <summary>
        /// Gets the list of arguments which are inputs for DLT streams.
        /// </summary>
        /// <value>The input DLT streams to read.</value>
        public IReadOnlyList<string> Arguments { get { return m_Arguments; } }
    }
}
