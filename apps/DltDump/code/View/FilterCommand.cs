namespace RJCP.App.DltDump.View
{
    using System;
    using System.Threading.Tasks;
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
            FilterConfig config = new FilterConfig(m_Options.Arguments) {
                ShowPosition = m_Options.Position,
                InputFormat = m_Options.InputFormat
            };
            FilterApp app = new FilterApp(config);

            Task<ExitCode> filterTask = app.Run();
            filterTask.ConfigureAwait(false);
            return filterTask.GetAwaiter().GetResult();
        }
    }
}
