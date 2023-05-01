﻿namespace RJCP.App.DltDump.View
{
    using System.Collections.Generic;
    using Application;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;
    using Services;

    public class NonVerboseCommand : ICommand
    {
        private readonly FibexOptions m_Options = FibexOptions.None;
        private readonly IEnumerable<string> m_FibexPaths;

        public NonVerboseCommand(CmdOptions options)
        {
            m_Options = FibexService.GetOptions(options);
            m_FibexPaths = options.Fibex;
        }

        public ExitCode Run()
        {
            NonVerboseApp app = new NonVerboseApp(m_Options);
            return app.Run(m_FibexPaths);
        }
    }
}
