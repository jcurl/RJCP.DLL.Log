namespace RJCP.App.DltDump.Application
{
    using System.Diagnostics.CodeAnalysis;
    using Infrastructure;
    using Resources;

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Design calls for an object, no performance impact")]
    public class VersionApp
    {
        public void ShowVersion()
        {
            ShowSimpleVersion();

            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionTraceReader,
                Version.GetAssemblyVersion(typeof(Diagnostics.Log.TraceLine)));
            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionTraceReaderDlt,
                Version.GetAssemblyVersion(typeof(Diagnostics.Log.DltFileTraceReaderFactory)));
        }

        public void ShowSimpleVersion()
        {
            string copyright = Version.GetAssemblyCopyright(typeof(Program));
            if (string.IsNullOrWhiteSpace(copyright)) {
                Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionDltDump,
                    Version.GetAssemblyVersion(typeof(Program)));
            } else {
                Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionDltDumpCopyright,
                    Version.GetAssemblyVersion(typeof(Program)), copyright);
            }
        }
    }
}
