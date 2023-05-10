namespace RJCP.App.DltDump.Application
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Domain.Dlt;
    using Infrastructure.Constraints;
    using RJCP.Diagnostics.Log.Constraints;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose;

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
            if (input is null) throw new ArgumentNullException(nameof(input));

            Input = input;
        }

        /// <summary>
        /// Gets the input streams for parsing.
        /// </summary>
        /// <value>The input streams for parsing.</value>
        public IReadOnlyList<string> Input { get; }

        /// <summary>
        /// Gets or sets the frame map that is used for decoding non-verbose messages.
        /// </summary>
        /// <value>The frame map used for decoding non-verbose messages.</value>
        public IFrameMap FrameMap { get; set; }

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
        /// Allows force overwrite of the output file.
        /// </summary>
        /// <value>
        /// Force overwrite if <see langword="true"/>; otherwise, raise errors with <see langword="false"/>.
        /// </value>
        public bool Force { get; set; }

        /// <summary>
        /// Gets or sets the split.
        /// </summary>
        /// <value>The number of bytes each file should grow to before splitting.</value>
        public long Split { get; set; }

        /// <summary>
        /// Gets or sets the name of the output file.
        /// </summary>
        /// <value>The name of the output file.</value>
        public string OutputFileName { get; set; }

        /// <summary>
        /// Gets or sets the output format.
        /// </summary>
        /// <value>The output format.</value>
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Automatic;

        /// <summary>
        /// Gets or sets the connect retries for input stream types that require a connection.
        /// </summary>
        /// <value>The number of connect retries.</value>
        public int ConnectRetries { get; set; }

        #region Filters using Constraints
        private Constraint m_EcuId;
        private Constraint m_AppId;
        private Constraint m_CtxId;
        private Constraint m_Search;
        private Constraint m_Session;
        private Constraint m_MessageType;
        private Constraint m_Time;
        private Constraint m_MessageId;
        private bool m_Verbose;
        private bool m_NonVerbose;
        private bool m_Control;
        private bool m_None;

        private static Constraint AddConstraint(Constraint constraint, IMatchConstraint match)
        {
            if (constraint is null) {
                constraint = new Constraint();
                constraint.Expr(match);
            } else {
                constraint.Or.Expr(match);
            }

            return constraint;
        }

        /// <summary>
        /// Adds the ECU identifier to the filter.
        /// </summary>
        /// <param name="ecuid">The ecu identifier.</param>
        public void AddEcuId(string ecuid)
        {
            m_EcuId = AddConstraint(m_EcuId, new DltEcuId(ecuid));
        }

        /// <summary>
        /// Adds the application identifier to the filter.
        /// </summary>
        /// <param name="appid">The application identifier.</param>
        public void AddAppId(string appid)
        {
            m_AppId = AddConstraint(m_AppId, new DltAppId(appid));
        }

        /// <summary>
        /// Adds the context identifier to the filter.
        /// </summary>
        /// <param name="ctxid">The context identifier.</param>
        public void AddCtxId(string ctxid)
        {
            m_CtxId = AddConstraint(m_CtxId, new DltCtxId(ctxid));
        }

        /// <summary>
        /// Adds the search string to the filter.
        /// </summary>
        /// <param name="text">The text string to add to the filter.</param>
        /// <param name="ignoreCase">
        /// Does a case insensitive search if <see langword="true"/>, else case sensitive search if
        /// <see langword="false"/>.
        /// </param>
        public void AddSearchString(string text, bool ignoreCase)
        {
            IMatchConstraint filter;
            if (!ignoreCase) {
                filter = new TextString(text);
            } else {
                filter = new TextIString(text);
            }

            m_Search = AddConstraint(m_Search, filter);
        }

        /// <summary>
        /// Adds the regular expression string to the filter.
        /// </summary>
        /// <param name="text">The regular expression to add to the filter.</param>
        /// <param name="ignoreCase">
        /// Does a case insensitive search if <see langword="true"/>, else case sensitive search if
        /// <see langword="false"/>.
        /// </param>
        public void AddRegexString(string text, bool ignoreCase)
        {
            IMatchConstraint filter;
            if (!ignoreCase) {
                filter = new TextRegEx(text);
            } else {
                filter = new TextIRegEx(text);
            }

            m_Search = AddConstraint(m_Search, filter);
        }

        /// <summary>
        /// Adds the session identifier to the filter.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void AddSessionId(int sessionId)
        {
            m_Session = AddConstraint(m_Session, new DltSessionId(sessionId));
        }

        /// <summary>
        /// Adds the message type to the filter.
        /// </summary>
        /// <param name="dltType">The message type to add to the filter.</param>
        public void AddMessageType(DltType dltType)
        {
            m_MessageType = AddConstraint(m_MessageType, new DltMessageType(dltType));
        }

        /// <summary>
        /// Adds the message identifier to the filter.
        /// </summary>
        /// <param name="sessionId">The message identifier.</param>
        public void AddMessageId(int messageId)
        {
            m_MessageId = AddConstraint(m_MessageId, new DltMessageId(messageId));
        }

        /// <summary>
        /// Adds the time range to filter. This can only be set once.
        /// </summary>
        /// <param name="notBefore">The time stamp that messages not before should be shown.</param>
        /// <param name="notAfter">The time stamp that messages not after should be shown.</param>
        public void AddTimeRange(DateTime? notBefore, DateTime? notAfter)
        {
            if (!notBefore.HasValue && !notAfter.HasValue) return;

            m_Time = new Constraint();
            if (notBefore.HasValue) m_Time.Expr(new DltNotBeforeDate(notBefore.Value));
            if (notAfter.HasValue) m_Time.Expr(new DltNotAfterDate(notAfter.Value));
        }

        /// <summary>
        /// Filter for non-control messages that have the verbose flag set.
        /// </summary>
        public void SetVerbose()
        {
            m_Verbose = true;
        }

        /// <summary>
        /// Filter for non-control messages that have the non-verbose flag not set.
        /// </summary>
        public void SetNonVerbose()
        {
            m_NonVerbose = true;
        }

        /// <summary>
        /// Filter for control messages.
        /// </summary>
        public void SetControlMessage()
        {
            m_Control = true;
        }

        /// <summary>
        /// Filter so that no messages match.
        /// </summary>
        public void None()
        {
            m_None = true;
        }

        /// <summary>
        /// Gets the filter set from the other filter methods.
        /// </summary>
        /// <returns>
        /// A constraint object that can be used as a filter. If there should be no filtering, the result is
        /// <see langword="null"/>.
        /// </returns>
        public Constraint GetFilter()
        {
            if (m_None) return new Constraint().Expr(new BlockAll());

            Constraint constraint = new Constraint();
            bool filtered = false;
            if (m_EcuId is object) {
                constraint.Expr(m_EcuId);
                filtered = true;
            }

            if (m_AppId is object) {
                constraint.Expr(m_AppId);
                filtered = true;
            }

            if (m_CtxId is object) {
                constraint.Expr(m_CtxId);
                filtered = true;
            }

            if (m_Session is object) {
                constraint.Expr(m_Session);
                filtered = true;
            }

            if (m_Time is object) {
                constraint.Expr(m_Time);
                filtered = true;
            }

            if (m_Search is object) {
                constraint.Expr(m_Search);
                filtered = true;
            }

            if (m_MessageType is object) {
                constraint.Expr(m_MessageType);
                filtered = true;
            }

            if (m_MessageId is object) {
                constraint.Expr(m_MessageId);
                filtered = true;
            }

            if ((m_Verbose || m_NonVerbose || m_Control) &&
                !(m_Verbose && m_NonVerbose && m_Control)) {
                // If all options are given, or no options are given, we search for all messages.

                Constraint typeConstraint = null;
                if (m_Verbose) {
                    typeConstraint = AddConstraint(typeConstraint,
                        new Constraint().Not.DltIsControl().DltIsVerbose(true));
                }

                if (m_NonVerbose) {
                    typeConstraint = AddConstraint(typeConstraint,
                        new Constraint().Not.DltIsControl().DltIsVerbose(false));
                }

                if (m_Control) {
                    typeConstraint = AddConstraint(typeConstraint,
                        new DltIsControl());
                }

                filtered = true;
                constraint.Expr(typeConstraint);
            }

            if (!filtered) return null;
            return constraint.End();
        }
        #endregion

        /// <summary>
        /// Gets the number of lines to print before a match.
        /// </summary>
        /// <value>The number of lines to print before a match.</value>
        public int BeforeContext { get; set; }

        /// <summary>
        /// Gets the number of lines to print after a match.
        /// </summary>
        /// <value>The number of lines to print after a match.</value>
        public int AfterContext { get; set; }
    }
}
