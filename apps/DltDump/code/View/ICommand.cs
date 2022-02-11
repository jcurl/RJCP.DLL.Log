namespace RJCP.App.DltDump.View
{
    /// <summary>
    /// An interface for a top level command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>The result of the execution.</returns>
        ExitCode Run();
    }
}
