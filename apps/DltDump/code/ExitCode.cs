namespace RJCP.App.DltDump
{
    /// <summary>
    /// The application exit code.
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        /// There was no error, the operation finished successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// There was an error parsing the command line options.
        /// </summary>
        OptionsError = 1,

        /// <summary>
        /// An input URI couldn't be parsed.
        /// </summary>
        InputError = 2,

        /// <summary>
        /// None of the input files could be processed.
        /// </summary>
        NoFilesProcessed = 3,

        /// <summary>
        /// Not all of the input files could be processed.
        /// </summary>
        PartialFilesProcessed = 4,

        /// <summary>
        /// An unknown error occurred (an unhandled exception).
        /// </summary>
        UnknownError = 255
    }
}
