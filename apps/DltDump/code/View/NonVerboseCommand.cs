namespace RJCP.App.DltDump.View
{
    using System;
    using System.Collections.Generic;
    using Application;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;

    public class NonVerboseCommand : ICommand
    {
        private readonly FibexOptions m_Options = FibexOptions.None;
        private readonly IEnumerable<string> m_FibexPaths;

        public NonVerboseCommand(CmdOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.NonVerboseMultiEcu) m_Options |= FibexOptions.WithEcuId;
            if (options.NonVerboseNoExtHeader) m_Options |= FibexOptions.WithoutExtHeader;

            m_FibexPaths = options.Fibex;
        }

        public ExitCode Run()
        {
            NonVerboseApp app = new NonVerboseApp(m_Options);
            return app.Run(m_FibexPaths);
        }
    }
}
