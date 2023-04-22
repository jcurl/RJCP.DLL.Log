namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    /// <summary>
    /// The type of FIBEX lint warnings that can be returned.
    /// </summary>
    public enum FibexWarnings
    {
        /// <summary>
        /// No warnings presented.
        /// </summary>
        None,

        /// <summary>
        /// An ECU id doesn't have an identifier.
        /// </summary>
        EcuIdMissing,

        /// <summary>
        /// The FIBEX file contains multiple ECU IDs.
        /// </summary>
        EcusMultipleDefined,

        /// <summary>
        /// The FIBEX should define 'fx:ECUID' near the top, before frames.
        /// </summary>
        EcuIdMustBeBeforeFrames,

        /// <summary>
        /// A duplicate PDU id in the FIBEX file.
        /// </summary>
        /// <remarks>
        /// Caused when reading a FIBEX file, when two 'fx:PDU' elements have the same identifier.
        /// </remarks>
        PduIdDuplicate,

        /// <summary>
        /// A PDU id doesn't have an identifier.
        /// </summary>
        /// <remarks>
        /// Caused when reading a FIBEX file, when a 'fx:PDU' element doesn't have an 'ID' attribute.
        /// </remarks>
        PduIdMissing,

        /// <summary>
        /// The byte length field is invalid.
        /// </summary>
        PduInvalidByteLength,

        /// <summary>
        /// The Frame identifier is listed twice and is thus ambiguous.
        /// </summary>
        FrameIdDuplicate,

        /// <summary>
        /// A Frame id doesn't have an identifier.
        /// </summary>
        FrameIdMissing,

        /// <summary>
        /// The Frame identifier is invalid and can't be parsed.
        /// </summary>
        FrameIdInvalid,

        /// <summary>
        /// The Frame Message DLT type or info is unknown.
        /// </summary>
        FrameMessageTypeInvalid,

        /// <summary>
        /// The PDU-REF is missing the attribute referencing to a PDU.
        /// </summary>
        FramePduRefIdMissing,

        /// <summary>
        /// A PDU-REF in a frame doesn't reference to a known PDU.
        /// </summary>
        FramePduRefIdUnknown,

        /// <summary>
        /// The DLT Application Identifier is missing from the frame.
        /// </summary>
        FrameApplicationIdMissing,

        /// <summary>
        /// The DLT Context Identifier is missing from the frame.
        /// </summary>
        FrameContextIdMissing,

        /// <summary>
        /// The DLT Message Type is missing from the frame.
        /// </summary>
        FrameMessageTypeMissing,

        /// <summary>
        /// The DLT Message Info is missing from the frame.
        /// </summary>
        FrameMessageInfoMissing,

        /// <summary>
        /// A PDU has a signal reference that is empty.
        /// </summary>
        MissingSignalIdRef,

        /// <summary>
        /// Multiple signal instances in a PDU are defined.
        /// </summary>
        /// <remarks>
        /// The FIBEX should not have more than a single 'fx:SIGNAL-REF' for a 'fx:PDU'.
        /// </remarks>
        MultipleSignalsInPduDefined,

        /// <summary>
        /// A duplicate entry was found, indicating this entry was discarded.
        /// </summary>
        DuplicateEntry
    }
}
