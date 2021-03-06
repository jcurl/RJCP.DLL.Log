namespace RJCP.App.DltDump.Domain
{
    using OutputStream;

    public class TestOutputStreamFactory : IOutputStreamFactory
    {
        public bool Force { get; set; }

        public long Split { get; set; }

        public IOutputStream Create(OutputFormat outFormat, string outFileName)
        {
            return new ConsoleOutput();
        }
    }
}
