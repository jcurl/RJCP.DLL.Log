namespace RJCP.App.DltDump.Domain
{
    using OutputStream;

    public class TestOutputStreamFactory : IOutputStreamFactory
    {
        public bool Force { get; set; }

        public long Split { get; set; }

        public bool ConvertNonVerbose { get; set; }

        public InputFiles InputFiles { get; } = new InputFiles();

        public IOutputStream Create(OutputFormat outFormat, string outFileName)
        {
            return new ConsoleOutput();
        }
    }
}
