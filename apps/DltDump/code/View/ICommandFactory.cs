namespace RJCP.App.DltDump.View
{
    using RJCP.Core.CommandLine;

    /// <summary>
    /// Interface for defining an application command factory.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Creates the command using the given options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns><see cref="ICommand"/> instance.</returns>
        ICommand Create(Options cmdLine, CmdOptions options);
    }
}
