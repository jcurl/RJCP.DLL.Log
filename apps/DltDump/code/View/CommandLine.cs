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

            return ExitCode.Success;
        }
    }
}
