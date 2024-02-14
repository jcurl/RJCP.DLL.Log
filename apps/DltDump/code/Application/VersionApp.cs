namespace RJCP.App.DltDump.Application
{
    using Infrastructure;
    using Resources;

    public static class VersionApp
    {
        private static readonly object s_Lock = new();
        private static string s_Version;

        public static string GetVersion()
        {
            if (s_Version is null) {
                lock (s_Lock) {
                    if (s_Version is null) {
                        string copyright = Version.GetAssemblyCopyright(typeof(Program));
                        if (string.IsNullOrWhiteSpace(copyright)) {
                            s_Version = string.Format(AppResources.VersionDltDump,
                                Version.GetAssemblyVersion(typeof(Program)));
                        } else {
                            s_Version = string.Format(AppResources.VersionDltDumpCopyright,
                                Version.GetAssemblyVersion(typeof(Program)), copyright);
                        }
                    }
                }
            }
            return s_Version;
        }

        public static void ShowVersion()
        {
            ShowSimpleVersion();
            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionNetRuntime,
                System.Environment.Version);
            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionTraceReader,
                Version.GetAssemblyVersion(typeof(Diagnostics.Log.TraceLine)));
            Global.Instance.Terminal.StdOut.WriteLine(AppResources.VersionTraceReaderDlt,
                Version.GetAssemblyVersion(typeof(Diagnostics.Log.DltFileTraceReaderFactory)));
        }

        public static void ShowSimpleVersion()
        {
            Global.Instance.Terminal.StdOut.WrapLine(GetVersion());
        }
    }
}
