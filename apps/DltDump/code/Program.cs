namespace RJCP.App.DltDump
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Application;
    using Resources;
    using RJCP.Diagnostics;
    using Services;
    using View;

    public static class Program
    {
        public static int Main(string[] args)
        {
            CrashReporter.SetExceptionHandlers();

            Log.App.TraceEvent(TraceEventType.Information, VersionApp.GetVersion());

            try {
                return (int)CommandLine.Run(args);
            } catch (Exception ex) {
                Terminal.WriteLine(AppResources.ErrorAppUnhandledException);
                Log.App.TraceException(ex, nameof(Program), "Unhandled exception");

                // We don't need the path. This will print to the log at level 'warn'.
                CrashReporter.CreateDump();

                // If the user is logging to the console, we need to wait 200ms for it to be printed.
                Thread.Sleep(200);
                return (int)ExitCode.UnknownError;
            }
        }
    }
}
