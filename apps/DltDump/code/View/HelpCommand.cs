namespace RJCP.App.DltDump.View
{
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
            return ExitCode.Success;
        }
    }
}
