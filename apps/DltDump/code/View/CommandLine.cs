namespace RJCP.App.DltDump.View
{
    using RJCP.Core.CommandLine;

    public static class CommandLine
    {
        public static ExitCode Run(string[] arguments)
        {
            CmdOptions cmdOptions = new CmdOptions();

            try {
                _ = Options.Parse(cmdOptions, arguments);
            } catch (OptionException) {
                /* TODO: Should print the error and then the usage */
                return ExitCode.OptionsError;
            }

            ICommand command = Application.Instance.CommandFactory.Create(cmdOptions);
            if (command == null) {
                /* TODO: Should print that no options were given */
                return ExitCode.OptionsError;
            }

            return command.Run();
        }
    }
}
