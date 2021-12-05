namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// All possible DLT type/subtype combinations following the Specification of Diagnostic Log and Trace AUTOSAR
    /// Release 4.2.2.
    /// </summary>
    /// <remarks>
    /// Each value represents the type/subtype combination as it is encoded in the Message Info (MSIN) single byte
    /// field, following the Specification of Diagnostic Log and Trace AutoSAR Release 4.2.2 specification.
    /// <para>The Verbose bit is 0 for all values.</para>
    /// </remarks>
    public enum DltType
    {
        /// <summary>
        /// Message Log Info, subtype Fatal.
        /// </summary>
        LOG_FATAL = 0x10,

        /// <summary>
        /// Message Log Info, subtype Error.
        /// </summary>
        LOG_ERROR = 0x20,

        /// <summary>
        /// Message Log Info, subtype Warning.
        /// </summary>
        LOG_WARN = 0x30,

        /// <summary>
        /// Message Log Info, subtype Information.
        /// </summary>
        LOG_INFO = 0x40,

        /// <summary>
        /// Message Log Info, subtype Debug.
        /// </summary>
        LOG_DEBUG = 0x50,

        /// <summary>
        /// Message Log Info, subtype Verbose.
        /// </summary>
        LOG_VERBOSE = 0x60,

        /// <summary>
        /// Message Trace Info, subtype Variable.
        /// </summary>
        APP_TRACE_VARIABLE = 0x12,

        /// <summary>
        /// Message Trace Info, subtype Function in.
        /// </summary>
        APP_TRACE_FUNCTION_IN = 0x22,

        /// <summary>
        /// Message Trace Info, subtype Function out.
        /// </summary>
        APP_TRACE_FUNCTION_OUT = 0x32,

        /// <summary>
        /// Message Trace Info, subtype State.
        /// </summary>
        APP_TRACE_STATE = 0x42,

        /// <summary>
        /// Message Trace Info, subtype VFB.
        /// </summary>
        APP_TRACE_VFB = 0x52,

        /// <summary>
        /// DLT Network Message, subtype Inter-Process-Communication.
        /// </summary>
        NW_TRACE_IPC = 0x14,

        /// <summary>
        /// DLT Network Message, subtype CAN Communications bus.
        /// </summary>
        NW_TRACE_CAN = 0x24,

        /// <summary>
        /// DLT Network Message, subtype FlexRay Communications bus.
        /// </summary>
        NW_TRACE_FLEXRAY = 0x34,

        /// <summary>
        /// DLT Network Message, subtype Most Communications bus.
        /// </summary>
        NW_TRACE_MOST = 0x44,

        /// <summary>
        /// DLT Network Message, subtype Ethernet Communications bus.
        /// </summary>
        NW_TRACE_ETHERNET = 0x54,

        /// <summary>
        /// DLT Network Message, subtype SOME/IP Communication.
        /// </summary>
        NW_TRACE_SOMEIP = 0x64,

        /// <summary>
        /// DLT Network Message, subtype user defined 0.
        /// </summary>
        NW_TRACE_USER_DEFINED_0 = 0x74,

        /// <summary>
        /// DLT Network Message, subtype user defined 1.
        /// </summary>
        NW_TRACE_USER_DEFINED_1 = 0x84,

        /// <summary>
        /// DLT Network Message, subtype user defined 2.
        /// </summary>
        NW_TRACE_USER_DEFINED_2 = 0x94,

        /// <summary>
        /// DLT Network Message, subtype user defined 3.
        /// </summary>
        NW_TRACE_USER_DEFINED_3 = 0xA4,

        /// <summary>
        /// DLT Network Message, subtype user defined 4.
        /// </summary>
        NW_TRACE_USER_DEFINED_4 = 0xB4,

        /// <summary>
        /// DLT Network Message, subtype user defined 5.
        /// </summary>
        NW_TRACE_USER_DEFINED_5 = 0xC4,

        /// <summary>
        /// DLT Network Message, subtype user defined 6.
        /// </summary>
        NW_TRACE_USER_DEFINED_6 = 0xD4,

        /// <summary>
        /// DLT Network Message, subtype user defined 7.
        /// </summary>
        NW_TRACE_USER_DEFINED_7 = 0xE4,

        /// <summary>
        /// DLT Network Message, subtype user defined 8.
        /// </summary>
        NW_TRACE_USER_DEFINED_8 = 0xF4,

        /// <summary>
        /// Message Control Info, subtype Request.
        /// </summary>
        CONTROL_REQUEST = 0x16,

        /// <summary>
        /// Message Control Info, subtype Response.
        /// </summary>
        CONTROL_RESPONSE = 0x26,

        /// <summary>
        /// Message Control Info, subtype Time.
        /// </summary>
        CONTROL_TIME = 0x36,

        /// <summary>
        /// Unknown message type/subtype.
        /// </summary>
        UNKNOWN = 0xFF
    }
}
