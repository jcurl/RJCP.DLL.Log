namespace RJCP.Diagnostics.Log
{
    using System;
    using Dlt;
    using Dlt.ControlArgs;

    /// <summary>
    /// Representation of a single control message of the DLT protocol.
    /// </summary>
    public class DltControlTraceLine : DltTraceLineBase
    {
        private static readonly DltLineFeatures ControlLineFeatures =
            DltLineFeatures.MessageTypeFeature +
            DltLineFeatures.AppIdFeature +
            DltLineFeatures.CtxIdFeature;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltControlTraceLine"/> class.
        /// </summary>
        /// <param name="service">The service object describing the control message request or response.</param>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        public DltControlTraceLine(IControlArg service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Service = service;
            Type = service.DefaultType;
            Features = ControlLineFeatures;
        }

        /// <summary>
        /// Gets the service object for this control message.
        /// </summary>
        /// <value>The service object for this control message.</value>
        public IControlArg Service { get; }

        /// <summary>
        /// This is the decoded line of text with time stamp and other metadata stripped.
        /// </summary>
        /// <value>This is a string that represents a printable version of the control message.</value>
        public override string Text
        {
            get
            {
                base.Text ??= Service.ToString() ?? string.Empty;
                return base.Text;
            }
            set { base.Text = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        /// <remarks>
        /// Shall return a string containing the values of the DLT line fields, similar to a DLT trace line, exported to
        /// a text file using DLT Viewer 2.18.0. The date of the log message is converted to local time.
        /// </remarks>
        public override string ToString()
        {
            return string.Format("{0} non-verbose {1}", base.ToString(), Text);
        }
    }
}
