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
        private static readonly DateTime Expire = new DateTime(2022, 7, 1);

        public static int Main(string[] args)
        {
            if (DateTime.Now > Expire) {
                Console.WriteLine("This software is experimental, and expired on {0}\n\n", Expire.ToShortDateString());
                return -1;
            } else {
                Console.WriteLine("This software is experimental, and will expire on {0}\n\n", Expire.ToShortDateString());
            }

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
