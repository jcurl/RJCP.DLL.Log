namespace RJCP.App.DltDump.Application
{
    using Infrastructure.Text;
    using Resources;
    using RJCP.Core.Environment;
    using Services;

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
            Write(4, 6, HelpResources.Help325_InputFormatPcap);
            Write(2, 4, HelpResources.Help330_Retries);
            Write(2, 4, HelpResources.Help335_StringSearch);
            Write(2, 4, HelpResources.Help340_RegexSearch);
            Write(2, 4, HelpResources.Help345_IgnoreCase);
            Write(2, 4, HelpResources.Help350_EcuId);
            Write(2, 4, HelpResources.Help355_AppId);
            Write(2, 4, HelpResources.Help360_CtxId);
            Write(2, 4, HelpResources.Help365_SessionId);
            Write(2, 4, HelpResources.Help370_DltTypes);
            Write(2, 4, HelpResources.Help375_Verbose);
            Write(2, 4, HelpResources.Help380_NonVerbose);
            Write(2, 4, HelpResources.Help385_Control);
            Write(2, 4, HelpResources.Help390_BeforeContext);
            Write(2, 4, HelpResources.Help395_AfterContext);
            Write(2, 4, HelpResources.Help400_OutputFileName);
            Write(4, 6, HelpResources.Help401_OutputFile);
            Write(4, 6, HelpResources.Help402_OutputDateTime);
            Write(4, 6, HelpResources.Help403_OuputDate);
            Write(4, 6, HelpResources.Help404_OutputTime);
            Write(4, 6, HelpResources.Help405_OutputSplit);
            Write(2, 4, HelpResources.Help410_Force);
            Write(2, 4, HelpResources.Help415_Split);
            Write(2, 4, HelpResources.Help420_Log);
            Write();
            Write(HelpResources.Help500_Input);
            Write();
            Write(2, 4, HelpResources.Help505_InputFile);
            Write(2, 4, HelpResources.Help506_InputFilePcap);
            Write(2, 4, HelpResources.Help507_InputFilePcapNg);
            Write(2, 4, HelpResources.Help510_InputTcp);
            Write(2, 4, HelpResources.Help515_InputSerial);
            Write();
            Write(HelpResources.Help800_ExitCodes);
            Write();
            Write(2, 4, HelpResources.Help805_Success);
            Write(2, 4, HelpResources.Help810_OptionError);
            Write(2, 4, HelpResources.Help811_InputUnknown);
            Write(2, 4, HelpResources.Help812_NoFilesProcessed);
            Write(2, 4, HelpResources.Help813_PartialFilesProcessed);
            Write(2, 4, HelpResources.Help814_OutputError);
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
            Terminal.WriteLine(indent, hangingIndent, message, ShortOptionSymbol, LongOptionSymbol, AssignmentSymbol);
        }
    }
}
