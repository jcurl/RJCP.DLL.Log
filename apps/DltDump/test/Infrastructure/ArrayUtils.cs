namespace RJCP.App.DltDump.Infrastructure
{
    using System;

    public static class ArrayUtils
    {
        public static T[] CopyArray<T>(this T[] sourceArray)
        {
            T[] buffer = new T[sourceArray.Length];
            Array.Copy(sourceArray, buffer, sourceArray.Length);
            return buffer;
        }
    }
}
