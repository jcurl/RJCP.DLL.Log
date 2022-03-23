namespace RJCP.Diagnostics.Log
{
    using System;
    using Collections;
    using Dlt;
    using Dlt.Args;

    /// <summary>
    /// A DLT log line indicating that parsing found data that doesn't conform to the DLT protocol.
    /// </summary>
    public class DltSkippedTraceLine : DltTraceLine
    {
        private static readonly DltLineFeatures SkippedLineFeatures =
            DltLineFeatures.EcuIdFeature + DltLineFeatures.AppIdFeature + DltLineFeatures.CtxIdFeature +
            DltLineFeatures.VerboseFeature + DltLineFeatures.MessageTypeFeature;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltSkippedTraceLine"/> class.
        /// </summary>
        /// <param name="bytes">The number of bytes that were skipped.</param>
        /// <param name="reason">The reason why data was skipped.</param>
        /// <remarks>
        /// The argument list is read-only.
        /// </remarks>
        public DltSkippedTraceLine(long bytes, string reason)
        {
            EcuId = string.Empty;
            ApplicationId = "SKIP";
            ContextId = "SKIP";
            Count = InvalidCounter;
            DeviceTimeStamp = new TimeSpan(0);
            Type = DltType.LOG_WARN;
            Features = SkippedLineFeatures;
            BytesSkipped = bytes;

            if (reason != null) {
                Arguments = new ReadOnlyArrayList<IDltArg>(new IDltArg[] {
                    new StringDltArg("Skipped:"),
                    new SignedIntDltArg(bytes),
                    new StringDltArg("bytes; Reason:"),
                    new StringDltArg(reason)
                });
                Reason = reason;
            } else {
                Arguments = new ReadOnlyArrayList<IDltArg>(new IDltArg[] {
                    new StringDltArg("Skipped:"),
                    new SignedIntDltArg(bytes),
                    new StringDltArg("bytes")
                });
                Reason = string.Empty;
            }
        }

        /// <summary>
        /// Gets the number of bytes that were skipped.
        /// </summary>
        /// <value>The number of bytes that were skipped.</value>
        public long BytesSkipped { get; }

        /// <summary>
        /// Gets the reason why data was skipped.
        /// </summary>
        /// <value>The reason why data was skipped.</value>
        /// <remarks>
        /// The actual reason why data was skipped is optional, and only contains the reason why data was skipped in the
        /// first place. Changes to the implementation may change the reason for bytes being skipped.
        /// </remarks>
        public string Reason { get; }
    }
}
