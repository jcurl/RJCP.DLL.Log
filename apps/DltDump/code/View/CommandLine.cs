namespace RJCP.App.DltDump.View
{
    using RJCP.Core.CommandLine;
    using Resources;
    using Application;

    public static class CommandLine
    {
        public static ExitCode Run(string[] arguments)
        {
            CmdOptions cmdOptions = new CmdOptions();

            try {
                _ = Options.Parse(cmdOptions, arguments);
            } catch (OptionException ex) {
                Global.Instance.Terminal.StdOut.WriteLine(AppResources.OptionsError);
                Global.Instance.Terminal.StdOut.WriteLine(ex.Message);

                return ExitCode.OptionsError;
            }

            ICommand command = Global.Instance.CommandFactory.Create(cmdOptions);
            if (command == null) {
                HelpApp.ShowSimpleHelp();
                return ExitCode.OptionsError;
            }

            return command.Run();
        }
    }
}
