namespace RJCP.App.DltDump.View
{
    using System;
    using Application;

    public class FilterCommand : ICommand
    {
        private readonly CmdOptions m_Options;

        public FilterCommand(CmdOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            m_Options = options;
        }

        public ExitCode Run()
        {
            FilterConfig config = new FilterConfig(m_Options.Arguments);
            FilterApp app = new FilterApp(config);

            return app.Run();
        }
    }
}
