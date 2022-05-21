namespace RJCP.App.DltDump
{
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using RJCP.Diagnostics.Logging;
    using RJCP.Diagnostics.Trace;

    public static class Log
    {
        public static LogSource App { get; }

        public static LogSource Pcap { get; }

        static Log()
        {
            LogSource.SetLoggerFactory(GetLoggerFactory());
            App = new LogSource("DltDump");
            Pcap = new LogSource("RJCP.Diagnostics.Log.Dlt.Pcap");
        }

        private static ILoggerFactory GetLoggerFactory()
        {
            // Should be something similar to dltdump.dll.json
            string file = typeof(Program).Assembly.Location;
            string app = Path.GetFileName(file);

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile($"{app}.json", true, false)
                .Build();

            return LoggerFactory.Create(builder => {
                builder
                    .AddConfiguration(config.GetSection("Logging"))
                    .AddConsole()
                    .AddSimplePrioMemoryLogger();
            });
        }
    }
}
