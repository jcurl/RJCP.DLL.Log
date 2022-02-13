namespace RJCP.App.DltDump.View
{
    using Application;

    /// <summary>
    /// Application to show the version or help.
    /// </summary>
    public class HelpCommand : ICommand
    {
        public enum Mode
        {
            ShowVersion,
            ShowHelp
        }

        public HelpCommand(Mode mode)
        {
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
                HelpApp.ShowHelp();
                return ExitCode.Success;
            default:
                return ExitCode.OptionsError;
            }
        }
    }
}
