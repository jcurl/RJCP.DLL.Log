namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// Describes the features available on the DLT Trace Line.
    /// </summary>
    /// <remarks>
    /// This is a value type, which is a wrapper around an integer. The enablement/disablement of features is done by
    /// bit toggling (the values of the bits are not related to any particular values defined in AutoSAR). THe bits are
    /// set or cleared depending on the features.
    /// <para>To combine features, use the predefined values and add them together with the <c>+</c> operator.</para>
    /// </remarks>
    public readonly struct DltLineFeatures
    {
        /// <summary>
        /// The set of no features.
        /// </summary>
        public static readonly DltLineFeatures NoFeatures = new DltLineFeatures(0);

        /// <summary>
        /// The ECU Identifier is present, as defined by the WEID bit (not, the storage header does not set this
        /// feature).
        /// </summary>
        public static readonly DltLineFeatures EcuIdFeature = new DltLineFeatures(1);

        /// <summary>
        /// The Application Identifier is present, as defined by the UEH bit.
        /// </summary>
        public static readonly DltLineFeatures AppIdFeature = new DltLineFeatures(2);

        /// <summary>
        /// The Context Identifier is present, as defined by the UEH bit.
        /// </summary>
        public static readonly DltLineFeatures CtxIdFeature = new DltLineFeatures(4);

        /// <summary>
        /// The log time stamp is present, as defined if there is a storage header present.
        /// </summary>
        public static readonly DltLineFeatures LogTimeStampFeature = new DltLineFeatures(8);

        /// <summary>
        /// The device time stamp is present, as defined by the WTMS bit.
        /// </summary>
        public static readonly DltLineFeatures DevTimeStampFeature = new DltLineFeatures(16);

        /// <summary>
        /// The Message Type is present, as defined by the UEH bit.
        /// </summary>
        public static readonly DltLineFeatures MessageTypeFeature = new DltLineFeatures(32);

        /// <summary>
        /// The Session Identifier is present, as defined by the SEID bit.
        /// </summary>
        public static readonly DltLineFeatures SessionIdFeature = new DltLineFeatures(64);

        /// <summary>
        /// The Verbose bit is set, as defined by the VERB bit.
        /// </summary>
        public static readonly DltLineFeatures VerboseFeature = new DltLineFeatures(128);

        /// <summary>
        /// The Big Endian encoding is enabled, as defined by the MSBF bit.
        /// </summary>
        public static readonly DltLineFeatures BigEndianFeature = new DltLineFeatures(256);

        private static readonly int FeatureMask = 0x1FF;

        private readonly int m_Feature;

        private DltLineFeatures(int features)
        {
            m_Feature = features;
        }

        /// <summary>
        /// Implements the + operator, combining two features together.
        /// </summary>
        /// <param name="feature">The feature to add from.</param>
        /// <param name="newFeature">The new feature to add to.</param>
        /// <returns>The result of the operator with the two features combined.</returns>
        public static DltLineFeatures operator +(DltLineFeatures feature, DltLineFeatures newFeature)
        {
            return new DltLineFeatures(feature.m_Feature | newFeature.m_Feature);
        }

        /// <summary>
        /// Implements the - operator, removing a set of features.
        /// </summary>
        /// <param name="feature">The feature to remove from.</param>
        /// <param name="delFeature">The features to remove.</param>
        /// <returns>The result of the operator, with the features on the right removed.</returns>
        public static DltLineFeatures operator -(DltLineFeatures feature, DltLineFeatures delFeature)
        {
            return new DltLineFeatures(feature.m_Feature & (~delFeature.m_Feature));
        }

        /// <summary>
        /// Implements the ~ operator, which inverts all the features
        /// </summary>
        /// <param name="feature">The feature set that should be inverted.</param>
        /// <returns>The result of the invert operator.</returns>
        /// <remarks>
        /// This feature is useful in removing all features but a single element.
        /// </remarks>
        public static DltLineFeatures operator ~(DltLineFeatures feature)
        {
            return new DltLineFeatures(FeatureMask & (~feature.m_Feature));
        }

        /// <summary>
        /// Sets or clears the specified features.
        /// </summary>
        /// <param name="feature">The feature set to change.</param>
        /// <param name="value">
        /// Set the features if <see langword="true"/>, otherwise if <see langword="false"/> then clear the features.
        /// </param>
        /// <returns>The result of the set operation.</returns>
        public DltLineFeatures Update(DltLineFeatures feature, bool value)
        {
            if (value) {
                return new DltLineFeatures(m_Feature | feature.m_Feature);
            } else {
                return new DltLineFeatures(m_Feature & ~feature.m_Feature);
            }
        }

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
        }

        private bool GetBit(DltLineFeatures bit) { return (m_Feature & bit.m_Feature) != 0; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return m_Feature.ToString("X");
        }
    }
}
