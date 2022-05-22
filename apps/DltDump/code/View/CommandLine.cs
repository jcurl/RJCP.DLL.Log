namespace RJCP.App.DltDump.View
{
    using System.Diagnostics;
    using System.Text;
    using Application;
    using Resources;
    using RJCP.Core.CommandLine;
    using RJCP.Diagnostics;
    using Services;

    public static class CommandLine
    {
        public static ExitCode Run(string[] arguments)
        {
            if (Log.App.ShouldTrace(TraceEventType.Information)) {
                StringBuilder cmdLine = new StringBuilder();
                foreach (string arg in arguments) {
                    cmdLine.AppendFormat("Arg: '{0}'; ", arg);
                }
                Log.App.TraceEvent(TraceEventType.Information, cmdLine.ToString());
            }

            CmdOptions cmdOptions = new CmdOptions();

            try {
                _ = Options.Parse(cmdOptions, arguments);
            } catch (OptionException ex) {
                Terminal.WriteLine(AppResources.OptionsError);
                Terminal.WriteLine(ex.Message);

                return ExitCode.OptionsError;
            }

            ICommand command = Global.Instance.CommandFactory.Create(cmdOptions);
            if (command == null) {
                HelpApp.ShowSimpleHelp();
                return ExitCode.OptionsError;
            }

            ExitCode result = command.Run();
            if (cmdOptions.Log) {
                Log.App.TraceEvent(TraceEventType.Information, "Result: {0} ({1})", result, (int)result);
                string path = CrashReporter.CreateDump(Diagnostics.Dump.CoreType.None);
                Terminal.WriteLine(AppResources.ErrorDumpBeingGenerated, path);
            }

            return result;
        }
    }
}
