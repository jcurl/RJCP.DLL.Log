namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    /// <summary>
    /// Provides extension methods for the <see cref="LogLevel"/> enumeration.
    /// </summary>
    public static class LogLevelExtension
    {
        /// <summary>
        /// Gets a descriptive string of the log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>A descriptive string of the log level</returns>
        public static string GetDescription(this LogLevel logLevel)
        {
            switch (logLevel) {
            case LogLevel.Default:
                return "default";
            case LogLevel.Block:
                return "block_all";
            case LogLevel.Fatal:
                return "fatal";
            case LogLevel.Error:
                return "error";
            case LogLevel.Warn:
                return "warning";
            case LogLevel.Info:
                return "info";
            case LogLevel.Debug:
                return "debug";
            case LogLevel.Verbose:
                return "verbose";
            default:
                return string.Format("log_level={0}", (int)logLevel);
            }
        }
    }
}
