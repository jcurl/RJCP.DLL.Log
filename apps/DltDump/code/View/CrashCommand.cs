namespace RJCP.App.DltDump.View
{
    using System;

    /// <summary>
    /// Implements a command for a test case that crash notification works.
    /// </summary>
    public class CrashCommand : ICommand
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>The result of the execution.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public ExitCode Run()
        {
            throw new NotImplementedException();
        }
    }
}
