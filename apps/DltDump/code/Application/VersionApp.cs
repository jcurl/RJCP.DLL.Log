namespace RJCP.App.DltDump.Application
{
    using Infrastructure;
    using Resources;

    public static class VersionApp
    {
        public static void ShowVersion()
        {
            ShowSimpleVersion();

            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionTraceReader,
                Version.GetAssemblyVersion(typeof(Diagnostics.Log.TraceLine)));
            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionTraceReaderDlt,
                Version.GetAssemblyVersion(typeof(Diagnostics.Log.DltFileTraceReaderFactory)));
        }

        public static void ShowSimpleVersion()
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
