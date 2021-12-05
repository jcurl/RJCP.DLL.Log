namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// Describes the features available on the DLT Trace Line.
    /// </summary>
    /// <remarks>
    /// This is a value type, which is a wrapper around an integer. The enablement/disablement of features is done by
    /// bit toggling (the values of the bits are not related to any particular values defined in AutoSAR). THe bits are
    /// set or cleared depending on the features.
    /// </remarks>
    internal class DltLineFeatures : IDltLineFeatures
    {
        internal const int NoFeatures = 0;
        internal const int EcuIdFeature = 1;          // As defined by WEID
        internal const int AppIdFeature = 2;          // As defined by UEH
        internal const int CtxIdFeature = 4;          // As defined by UEH
        internal const int LogTimeStampFeature = 8;   // As defined by WTMS
        internal const int DevTimeStampFeature = 16;  // If a storage header is present
        internal const int MessageTypeFeature = 32;   // As defined by UEH
        internal const int SessionIdFeature = 64;     // As defined by SEID
        internal const int VerboseFeature = 128;      // As defined by VERB
        internal const int BigEndianFeature = 256;    // As defined by MSBF

        // The ECU ID is presented twice in the protocol, but the feature is only set based on the WEID bit, not on
        // the presence of the storage header, to allow control when writing a packet.
        internal const int HasStorageHeader = DevTimeStampFeature;

        // The presence of the extended header UEH, implies these features are also present.
        internal const int HasExtendedHeader = AppIdFeature + CtxIdFeature + MessageTypeFeature;

        /// <summary>
        /// Initializes a new instance of the <see cref="DltLineFeatures"/> class with no features set.
        /// </summary>
        protected internal DltLineFeatures() : this(NoFeatures) { }

        internal DltLineFeatures(int features)
        {
            Features = features;
        }

        internal int Features { get; set; }

        /// <summary>
        /// Get or sets a value if the AutoSAR WEID bit is set for a DLT message.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if the ECU ID is part of the DLT message (WEID bit); otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool EcuId
        {
            get { return GetBit(EcuIdFeature); }
            set { SetBit(EcuIdFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is a logger time stamp (storage header) with this message.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if there is a logger time stamp associated with this message; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool TimeStamp
        {
            get { return GetBit(LogTimeStampFeature); }
            set { SetBit(LogTimeStampFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is an extended header (UEH) defining an Application ID.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if there is an application identifier associated with this message; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool ApplicationId
        {
            get { return GetBit(AppIdFeature); }
            set { SetBit(AppIdFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is an extended header (UEH) defining a Context ID.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if there is an context identifier associated with this message; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool ContextId
        {
            get { return GetBit(CtxIdFeature); }
            set { SetBit(CtxIdFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is an extended header (UEH) defining the message type.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if there is a message type field (MSTP, MTIN) associated with this message;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public bool MessageType
        {
            get { return GetBit(MessageTypeFeature); }
            set { SetBit(MessageTypeFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is a session identifier (WSID) as defined in the standard
        /// header.
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if there is a session identifier (SEID) associated with this message;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public bool SessionId
        {
            get { return GetBit(SessionIdFeature); }
            set { SetBit(SessionIdFeature, value); }
        }

        /// <summary>
        /// Get or sets a value if there is a device time stamp associated with this message (the WTMS bit).
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if there is a device time stamp associated with this message; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool DeviceTimeStamp
        {
            get { return GetBit(DevTimeStampFeature); }
            set { SetBit(DevTimeStampFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the message is in verbose format or not (VERB).
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if message is encoded in verbose format; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsVerbose
        {
            get { return GetBit(VerboseFeature); }
            set { SetBit(VerboseFeature, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this message is encoded in big endian (MSBF).
        /// </summary>
        /// <value>
        /// Is <see langword="true"/> if message is encoded in big endian; otherwise if <see langword="false"/>, the
        /// message is encoded in little endian.
        /// </value>
        public bool BigEndian
        {
            get { return GetBit(BigEndianFeature); }
            set { SetBit(BigEndianFeature, value); }
        }

        internal void Reset()
        {
            Features = 0;
        }

        internal void Set(int features)
        {
            Features = features;
        }

        internal void Set(DltLineFeatures features)
        {
            Features = features.Features;
        }

        private void SetBit(int bitValue, bool value)
        {
            if (value) {
                SetBit(bitValue);
            } else {
                ClrBit(bitValue);
            }
        }

        private void SetBit(int bitValue) { Features |= bitValue; }

        private void ClrBit(int bitValue) { Features &= ~bitValue; }

        private bool GetBit(int bitValue) { return (Features & bitValue) != 0; }
    }
}
