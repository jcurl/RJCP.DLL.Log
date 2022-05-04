namespace RJCP.App.DltDump.Native.Win32
{
    using System.Runtime.InteropServices;

    internal partial class Kernel32
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct LargeInteger
        {
            [FieldOffset(0)]
            public int Low;

            [FieldOffset(4)]
            public int High;

            [FieldOffset(0)]
            public long Quad;
        }
    }
}
