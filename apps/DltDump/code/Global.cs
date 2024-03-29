﻿namespace RJCP.App.DltDump
{
    using System.Diagnostics;
    using Domain;
    using Domain.Dlt;
    using RJCP.Core.Terminal;
    using RJCP.Core.Terminal.Log;
    using View;

    /// <summary>
    /// The application provides global instance data useful for the application.
    /// </summary>
    public class Global
    {
        private static Global s_Instance;
        private static readonly object m_Lock = new();

        /// <summary>
        /// Gets the singleton instance for this application.
        /// </summary>
        /// <value>The instance for application configuration.</value>
        /// <remarks>
        /// The code should use the <see cref="Instance"/> property of the <see cref="Global"/> class to get
        /// functionality needed for operation, such as factories and other configuration. Test code should be the only
        /// reason the instance data is overwritten.
        /// </remarks>
        public static Global Instance
        {
            get
            {
                Global result = s_Instance;
                if (result is null) {
                    lock (m_Lock) {
                        if (s_Instance is null) {
                            ConsoleTerminal terminal = new();
                            terminal.ConsoleWriteEvent += Terminal_ConsoleWriteEvent;

                            s_Instance = new Global() {
                                CommandFactory = new CommandFactory(),
                                Terminal = terminal,
                                InputStreamFactory = new InputStreamFactory(),
                                OutputStreamFactory = new OutputStreamFactory(),
                                DltReaderFactory = new DltDumpTraceReaderFactory()
                            };
                            result = s_Instance;
                        }
                    }
                }
                return result;
            }
        }

        private static void Terminal_ConsoleWriteEvent(object sender, TerminalWriteEventArgs e)
        {
            Log.AppTerminal.TraceEvent(TraceEventType.Information, "{0}", e.Line);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <remarks>
        /// On the next access to <see cref="Instance"/> will be reset to application defaults. This is important for
        /// unit testing to reset the state of the object to a known state.
        /// </remarks>
        public static void Reset()
        {
            lock (m_Lock) {
                s_Instance = null;
            }
        }

        /// <summary>
        /// The command factory that interprets the options and gives a command.
        /// </summary>
        /// <value>The command factory.</value>
        public ICommandFactory CommandFactory { get; set; }

        /// <summary>
        /// The class to write to the console terminal.
        /// </summary>
        /// <value>The console terminal.</value>
        public ITerminal Terminal { get; set; }

        /// <summary>
        /// Gets or sets the input stream factory.
        /// </summary>
        /// <value>The input stream factory.</value>
        public IInputStreamFactory InputStreamFactory { get; set; }

        /// <summary>
        /// Gets or sets the output stream factory.
        /// </summary>
        /// <value>The output stream factory.</value>
        public IOutputStreamFactory OutputStreamFactory { get; set; }

        /// <summary>
        /// The DLT reader factory that is used to obtain the reader based on inputs.
        /// </summary>
        /// <value>The DLT reader factory.</value>
        public IDltDumpTraceReaderFactory DltReaderFactory { get; set; }
    }
}
