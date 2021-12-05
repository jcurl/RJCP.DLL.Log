namespace RJCP.Diagnostics.Log.Dlt
{
    /// <summary>
    /// Provides extension methods for the <see cref="DltType"/> enumeration.
    /// </summary>
    public static class DltTypeExtension
    {
        /// <summary>
        /// Gets a descriptive string of the type/subtype combination.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A descriptive string of the type/subtype combination</returns>
        /// <remarks>
        /// The text being displayed is similar to the one shown by the DLT Viewer 2.18.0 STABLE for messages of type
        /// log, network trace and control.
        /// </remarks>
        public static string GetDescription(this DltType type)
        {
            switch (type) {
            case DltType.LOG_FATAL:
                return "log fatal";
            case DltType.LOG_ERROR:
                return "log error";
            case DltType.LOG_WARN:
                return "log warn";
            case DltType.LOG_INFO:
                return "log info";
            case DltType.LOG_DEBUG:
                return "log debug";
            case DltType.LOG_VERBOSE:
                return "log verbose";
            case DltType.NW_TRACE_IPC:
                return "nw_trace ipc";
            case DltType.NW_TRACE_CAN:
                return "nw_trace can";
            case DltType.NW_TRACE_FLEXRAY:
                return "nw_trace flexray";
            case DltType.NW_TRACE_MOST:
                return "nw_trace most";
            case DltType.NW_TRACE_ETHERNET:
                return "nw_trace vfb";
            case DltType.NW_TRACE_SOMEIP:
            case DltType.NW_TRACE_USER_DEFINED_0:
            case DltType.NW_TRACE_USER_DEFINED_1:
            case DltType.NW_TRACE_USER_DEFINED_2:
            case DltType.NW_TRACE_USER_DEFINED_3:
            case DltType.NW_TRACE_USER_DEFINED_4:
            case DltType.NW_TRACE_USER_DEFINED_5:
            case DltType.NW_TRACE_USER_DEFINED_6:
            case DltType.NW_TRACE_USER_DEFINED_7:
            case DltType.NW_TRACE_USER_DEFINED_8:
                return "nw_trace ";
            case DltType.CONTROL_REQUEST:
                return "control request";
            case DltType.CONTROL_RESPONSE:
                return "control response";
            case DltType.CONTROL_TIME:
                return "control time";
            case DltType.APP_TRACE_VARIABLE:
                return "app_trace variable";
            case DltType.APP_TRACE_FUNCTION_IN:
                return "app_trace func_in";
            case DltType.APP_TRACE_FUNCTION_OUT:
                return "app_trace func_out";
            case DltType.APP_TRACE_STATE:
                return "app_trace state";
            case DltType.APP_TRACE_VFB:
                return "app_trace vfb";
            default:
                return TypeDescription((int)type);
            }
        }

        private static string TypeDescription(int type)
        {
            switch (type & DltConstants.MessageType.DltTypeMask) {
            case DltConstants.MessageType.DltTypeLog:
                return "log ";
            case DltConstants.MessageType.DltTypeAppTrace:
                return "app_trace ";
            case DltConstants.MessageType.DltTypeNwTrace:
                return "nw_trace ";
            case DltConstants.MessageType.DltTypeControl:
                return "control ";
            default:
                return string.Empty;
            }
        }
    }
}
