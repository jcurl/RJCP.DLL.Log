namespace RJCP.App.DltDump.View
{
    using System;
    using System.IO;
    using Application;
    using Resources;
    using RJCP.Core.CommandLine;
    using RJCP.Diagnostics.Dump;

    public static class CommandLine
    {
        public static ExitCode Run(string[] arguments)
        {
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
                string path = Path.Combine(Environment.CurrentDirectory, Crash.Data.CrashDumpFactory.FileName);
                Terminal.WriteLine(AppResources.ErrorDumpBeingGenerated, path);
                Crash.Data.Dump(path);
            }

            return result;
        }
    }
}
