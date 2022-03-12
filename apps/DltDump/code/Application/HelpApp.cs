namespace RJCP.App.DltDump.Application
{
    using Infrastructure.Text;
    using Resources;
    using RJCP.Core.Environment;

    public static class HelpApp
    {
        static HelpApp()
        {
            if (Platform.IsWinNT()) {
                ShortOptionSymbol = "/";
                LongOptionSymbol = "/";
                AssignmentSymbol = ":";
            } else {
                ShortOptionSymbol = "-";
                LongOptionSymbol = "--";
                AssignmentSymbol = "=";
            }
        }

        private static string LongOptionSymbol { get; }

        private static string ShortOptionSymbol { get; }

        private static string AssignmentSymbol { get; }

        public static void ShowSimpleHelp()
        {
            Write(HelpResources.Help200_UsageInfo);
            Write();
            Write(2, 4, HelpResources.Help210_SimpleUsageScheme);
            Write();
            Write(HelpResources.Help300_Options);
            Write();
            Write(2, 4, HelpResources.Help310_HelpOption);
            Write(2, 4, HelpResources.Help315_VersionOption);
        }

        public static void ShowHelp()
        {
            VersionApp.ShowSimpleVersion();

            Write();
            Write(HelpResources.Help100_Description);
            Write();
            Write(HelpResources.Help200_UsageInfo);
            Write();
            Write(2, 4, HelpResources.Help210_UsageScheme);
            Write();
            Write(HelpResources.Help300_Options);
            Write();
            Write(2, 4, HelpResources.Help310_HelpOption);
            Write(2, 4, HelpResources.Help315_VersionOption);
            Write(2, 4, HelpResources.Help320_Position);
            Write(2, 4, HelpResources.Help325_InputFormat);
            Write(4, 6, HelpResources.Help325_InputFormatAuto);
            Write(4, 6, HelpResources.Help325_InputFormatFile);
            Write(4, 6, HelpResources.Help325_InputFormatSerial);
            Write(4, 6, HelpResources.Help325_InputFormatNetwork);
            Write(2, 4, HelpResources.Help330_Retries);
            Write();
            Write(HelpResources.Help400_Input);
            Write();
            Write(2, 4, HelpResources.Help405_InputFile);
            Write(2, 4, HelpResources.Help410_InputTcp);
            Write();
            Write(HelpResources.Help800_ExitCodes);
            Write();
            Write(2, 4, HelpResources.Help805_Success);
            Write(2, 4, HelpResources.Help810_OptionError);
            Write(2, 4, HelpResources.Help811_InputUnknown);
            Write(2, 4, HelpResources.Help812_NoFilesProcessed);
            Write(2, 4, HelpResources.Help813_PartialFilesProcessed);
            Write(2, 4, HelpResources.Help899_UnknownError);
            Write();
        }

        private static void Write()
        {
            Global.Instance.Terminal.StdOut.WriteLine(string.Empty);
        }

        private static void Write(string message)
        {
            Write(0, 0, message);
        }

        private static void Write(int indent, int hangingIndent, string message)
        {
            string expanded = string.Format(message, ShortOptionSymbol, LongOptionSymbol, AssignmentSymbol);
            string[] output = Format.Wrap(Global.Instance.Terminal.TerminalWidth - 1, indent, hangingIndent, expanded);

            if (output != null) {
                foreach (string line in output) {
                    Global.Instance.Terminal.StdOut.WriteLine(line);
                }
            }
        }
    }
}
