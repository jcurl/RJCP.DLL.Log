namespace RJCP.App.DltDump.Application
{
    using System.Diagnostics.CodeAnalysis;
    using Infrastructure;

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Design calls for an object, no performance impact")]
    public class VersionApp
    {
        public void ShowVersion()
        {
            ShowSimpleVersion();

            Global.Instance.Terminal.StdOut.WriteLine("  TraceReader: {0}",
                Version.GetAssemblyVersion(typeof(RJCP.Diagnostics.Log.TraceLine)));
            Global.Instance.Terminal.StdOut.WriteLine("  TraceReader.Dlt: {0}",
                Version.GetAssemblyVersion(typeof(RJCP.Diagnostics.Log.DltFileTraceReaderFactory)));
        }

        public void ShowSimpleVersion()
        {
            string copyright = Version.GetAssemblyCopyright(typeof(Program));
            if (string.IsNullOrWhiteSpace(copyright)) {
                Global.Instance.Terminal.StdOut.WriteLine("DltDump Version: {0}",
                    Version.GetAssemblyVersion(typeof(Program)));
            } else {
                Global.Instance.Terminal.StdOut.WriteLine("DltDump Version: {0}, {1}",
                    Version.GetAssemblyVersion(typeof(Program)), copyright);
            }
        }
    }
}
