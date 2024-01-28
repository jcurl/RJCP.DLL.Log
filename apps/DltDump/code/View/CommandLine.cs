namespace RJCP.App.DltDump.View
{
    using System.Diagnostics;
    using System.Text;
    using Application;
    using Resources;
    using RJCP.Core.CommandLine;
    using RJCP.Diagnostics;

    public static class CommandLine
    {
        public static ExitCode Run(string[] arguments)
        {
            if (Log.App.ShouldTrace(TraceEventType.Information)) {
                StringBuilder args = new StringBuilder();
                foreach (string arg in arguments) {
                    args.AppendFormat("Arg: '{0}'; ", arg);
                }
                Log.App.TraceEvent(TraceEventType.Information, args.ToString());
            }

            CmdOptions cmdOptions = new CmdOptions();

            Options cmdLine;
            try {
                cmdLine = Options.Parse(cmdOptions, arguments);
            } catch (OptionException ex) {
                Global.Instance.Terminal.StdOut.WrapLine(AppResources.OptionsError);
                Global.Instance.Terminal.StdOut.WriteLine(ex.Message);

                return ExitCode.OptionsError;
            }

            ICommand command = Global.Instance.CommandFactory.Create(cmdLine, cmdOptions);
            if (command is null) {
                HelpApp.ShowSimpleHelp(cmdLine);
                return ExitCode.OptionsError;
            }

            ExitCode result = command.Run();
            if (cmdOptions.Log) {
                Log.App.TraceEvent(TraceEventType.Information, "Result: {0} ({1})", result, (int)result);
                string path = CrashReporter.CreateDump(Diagnostics.Crash.CoreType.None);
                Global.Instance.Terminal.StdOut.WrapLine(AppResources.ErrorDumpBeingGenerated, path);
            }

            return result;
        }
    }
}
