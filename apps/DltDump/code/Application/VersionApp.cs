namespace RJCP.App.DltDump.Application
{
    using Infrastructure;
    using Resources;
    using Services;

    public static class VersionApp
    {
        private static readonly object s_Lock = new object();
        private static string s_Version;

        public static string GetVersion()
        {
            if (s_Version == null) {
                lock (s_Lock) {
                    if (s_Version == null) {
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
            Terminal.WriteDirect(AppResources.VersionNetRuntime, System.Environment.Version);
            Terminal.WriteDirect(AppResources.VersionTraceReader, Version.GetAssemblyVersion(typeof(Diagnostics.Log.TraceLine)));
            Terminal.WriteDirect(AppResources.VersionTraceReaderDlt, Version.GetAssemblyVersion(typeof(Diagnostics.Log.DltFileTraceReaderFactory)));
        }

        public static void ShowSimpleVersion()
        {
            Terminal.WriteLine(GetVersion());
        }
    }
}
