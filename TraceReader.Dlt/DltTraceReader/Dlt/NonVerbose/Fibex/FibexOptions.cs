namespace RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex
{
    using System;

    /// <summary>
    /// Describes a set of options that can be used when loading a FIBEX file.
    /// </summary>
    [Flags]
    public enum FibexOptions
    {
        /// <summary>
        /// Use the default options, of no differentiation between ECU ID (assume everything is from the same ECU, so
        /// the message identifiers cannot be mixed), and that an extended header is present (message identifiers must
        /// be unique for only the Application and Context Identifier).
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Load the FIBEX file, assuming that the input source has a standard header which contains the ECU Identifier.
        /// The default behaviour is without the standard header. Using this option can help to differentiate message
        /// identifiers between different ECUs.
        /// </summary>
        WithEcuId = 0x01,

        /// <summary>
        /// Load the FIBEX file, assuming that the input source does not have an extended header. The default behaviour
        /// is to assume that an extended header exists, so that message identifiers can be duplicated, so long as they
        /// have unique combination of application and context identifiers.
        /// </summary>
        WithoutExtHeader = 0x02
    }
}
