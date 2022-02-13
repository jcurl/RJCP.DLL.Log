namespace RJCP.App.DltDump.View
{
    public class CommandFactory : ICommandFactory
    {
        /// <summary>
        /// Creates the command using the given options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="ICommand" /> instance from the options.</returns>
        public ICommand Create(CmdOptions options)
        {
#if DEBUG
            if (options.CrashTest) {
                return new CrashCommand();
            }
#endif

            if (options.Help) {
                return new HelpCommand(HelpCommand.Mode.ShowHelp);
            } else if (options.Version) {
                return new HelpCommand(HelpCommand.Mode.ShowVersion);
            }

            return null;
        }
    }
}
