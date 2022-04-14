namespace RJCP.App.DltDump.Native.Win32
{
    using System.Runtime.InteropServices;

    internal partial class Kernel32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint DateTimeLow;
            public uint DateTimeHigh;
        }
    }
}
