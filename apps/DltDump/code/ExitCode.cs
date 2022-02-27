﻿namespace RJCP.App.DltDump
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
        /// None of the input files could be processed.
        /// </summary>
        NoFilesProcessed = 2,

        /// <summary>
        /// Not all of the input files could be processed.
        /// </summary>
        PartialFilesProcessed = 3,

        /// <summary>
        /// An unknown error occurred (an unhandled exception).
        /// </summary>
        UnknownError = 255
    }
}