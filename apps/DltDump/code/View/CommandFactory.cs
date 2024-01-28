namespace RJCP.App.DltDump.View
{
    using RJCP.Core.CommandLine;

    public class CommandFactory : ICommandFactory
    {
        /// <summary>
        /// Creates the command using the given options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="ICommand" /> instance from the options.</returns>
        public ICommand Create(Options cmdLine, CmdOptions options)
        {
#if DEBUG
            if (options.CrashTest) {
                return new CrashCommand();
            }
#endif

            if (options.Help) {
                return new HelpCommand(cmdLine, HelpCommand.Mode.ShowHelp);
            } else if (options.Version) {
                return new HelpCommand(cmdLine, HelpCommand.Mode.ShowVersion);
            }

            if (options.Arguments.Count > 0) {
                return new FilterCommand(options);
            } else {
                if (options.Fibex.Count > 0) {
                    return new NonVerboseCommand(options);
                }
            }

            return null;
        }
    }
}
