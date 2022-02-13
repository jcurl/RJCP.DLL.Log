namespace RJCP.App.DltDump
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Application;
    using Resources;
    using RJCP.Diagnostics;
    using RJCP.Diagnostics.Dump;
    using View;

    public static class Program
    {
        public static int Main(string[] args)
        {
            InitCrashReporter();

            Log.App.TraceEvent(TraceEventType.Information, VersionApp.GetVersion());

            try {
                return (int)CommandLine.Run(args);
            } catch (Exception ex) {
                Terminal.WriteLine(AppResources.ErrorAppUnhandledException);
                Log.App.TraceException(ex, nameof(Program), "Unhandled exception");
                string path = Path.Combine(Environment.CurrentDirectory, Crash.Data.CrashDumpFactory.FileName);
                Terminal.WriteLine(AppResources.ErrorDumpBeingGenerated, path);
                Crash.Data.Dump(path);

                return (int)ExitCode.UnknownError;
            }
        }

        private static void InitCrashReporter()
        {
            CrashReporter.SetExceptionHandlers();
        }
    }
}
