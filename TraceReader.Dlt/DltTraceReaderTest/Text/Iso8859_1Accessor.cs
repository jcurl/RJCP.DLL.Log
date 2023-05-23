namespace RJCP.Diagnostics.Log.Text
{
    using System;
    using System.Linq.Expressions;
    using RJCP.CodeQuality;

    public static class Iso8859_1Accessor
    {
        private const string AssemblyName = "RJCP.Diagnostics.Log.Dlt";
        private const string TypeName = "RJCP.Diagnostics.Log.Text.Iso8859_1";

        public static readonly PrivateType AccType =
            new PrivateType(AssemblyName, TypeName);

        private delegate int ConvertFromBytesDelegate(ReadOnlySpan<byte> bytes, char[] chars);

        public static int Convert(ReadOnlySpan<byte> bytes, char[] chars)
        {
            var method = AccType.ReferencedType.GetMethod(nameof(Convert),
                new Type[] { typeof(ReadOnlySpan<byte>), typeof(char[]) });
            var p1 = Expression.Parameter(typeof(ReadOnlySpan<byte>), nameof(bytes));
            var p2 = Expression.Parameter(typeof(char[]), nameof(chars));
            var call = Expression.Call(null, method, p1, p2);
            var expression = Expression.Lambda<ConvertFromBytesDelegate>(call, p1, p2);
            var func = expression.Compile();

            return func(bytes, chars);
        }

        private delegate int ConvertToBytesDelegate(string value, Span<byte> bytes);

        public static int Convert(string value, Span<byte> bytes)
        {
            var method = AccType.ReferencedType.GetMethod(nameof(Convert),
                new Type[] { typeof(string), typeof(Span<byte>) });
            var p1 = Expression.Parameter(typeof(string), nameof(value));
            var p2 = Expression.Parameter(typeof(Span<byte>), nameof(bytes));
            var call = Expression.Call(null, method, p1, p2);
            var expression = Expression.Lambda<ConvertToBytesDelegate>(call, p1, p2);
            var func = expression.Compile();

            return func(value, bytes);
        }
    }
}
