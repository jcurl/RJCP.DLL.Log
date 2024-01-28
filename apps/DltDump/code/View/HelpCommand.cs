namespace RJCP.App.DltDump.View
{
    using Application;
    using RJCP.Core.CommandLine;

    /// <summary>
    /// Application to show the version or help.
    /// </summary>
    public class HelpCommand : ICommand
    {
        private readonly Options m_CmdLine;

        public enum Mode
        {
            ShowVersion,
            ShowHelp
        }

        public HelpCommand(Options cmdLine, Mode mode)
        {
            m_CmdLine = cmdLine;
            HelpMode = mode;
        }

        public Mode HelpMode { get; }

        public ExitCode Run()
        {
            switch (HelpMode) {
            case Mode.ShowVersion:
                VersionApp.ShowVersion();
                return ExitCode.Success;
            case Mode.ShowHelp:
                HelpApp.ShowHelp(m_CmdLine);
                return ExitCode.Success;
            default:
                return ExitCode.OptionsError;
            }
        }
    }
}
