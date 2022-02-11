﻿namespace RJCP.App.DltDump
{
    /// <summary>
    /// The application provides global instance data useful for the application.
    /// </summary>
    public class Application
    {
        private static Application s_Instance;
        private static readonly object m_Lock = new object();

        /// <summary>
        /// Gets the singleton instance for this application.
        /// </summary>
        /// <value>The instance for application configuration.</value>
        /// <remarks>
        /// The code should use the <see cref="Instance"/> property of the <see cref="Application"/> class to get
        /// functionality needed for operation, such as factories and other configuration. Test code should be the only
        /// reason the instance data is overwritten.
        /// </remarks>
        public static Application Instance
        {
            get
            {
                if (s_Instance == null) {
                    lock (m_Lock) {
                        if (s_Instance == null) {
                            s_Instance = new Application() {
                                CommandFactory = new View.CommandFactory()
                            };
                        }
                    }
                }
                return s_Instance;
            }
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
        public View.ICommandFactory CommandFactory { get; set; }
    }
}
