namespace RJCP.Diagnostics.Log.Decoder
{
    using System;
    using System.Linq.Expressions;
    using RJCP.CodeQuality;

    public class LineCacheAccessor : AccessorBase
    {
        private const string AssemblyName = "RJCP.Diagnostics.Log.Dlt";
        private const string TypeName = "RJCP.Diagnostics.Log.Decoder.DltTraceDecoderBase";

        private static PrivateType GetLineCacheType()
        {
            PrivateType parent = new PrivateType(AssemblyName, TypeName);
            return parent.GetNestedType("LineCache");
        }

        private static readonly PrivateType AccType = GetLineCacheType();

        public LineCacheAccessor() : base(AccType) { }

        public int CacheLength
        {
            get { return (int)GetFieldOrProperty(nameof(CacheLength)); }
        }

        // As ReadOnlySpan<T> is a `ref struct` type, we can't box it, so more code is needed to compile it first.
        private delegate ReadOnlySpan<byte> GetCacheDelegate();

        public ReadOnlySpan<byte> GetCache()
        {
            var instance = Expression.Constant(PrivateTargetObject);
            var method = AccType.ReferencedType.GetMethod(nameof(GetCache), Type.EmptyTypes);
            var call = Expression.Call(instance, method, Array.Empty<Expression>());
            var expression = Expression.Lambda<GetCacheDelegate>(call, Array.Empty<ParameterExpression>());
            var func = expression.Compile();
            return func();
        }

        public int Consume(int bytes)
        {
            return (int)Invoke(nameof(Consume), bytes);
        }

        // As ReadOnlySpan<T> is a `ref struct` type, we can't box it, so more code is needed to compile it first.
        private delegate void AppendDelegate(ReadOnlySpan<byte> buffer);

        public void Append(ReadOnlySpan<byte> buffer)
        {
            var instance = Expression.Constant(PrivateTargetObject);
            var method = AccType.ReferencedType.GetMethod(nameof(Append), new Type[] { typeof(ReadOnlySpan<byte>) });
            var parameter = Expression.Parameter(typeof(ReadOnlySpan<byte>), nameof(buffer));
            var call = Expression.Call(instance, method, parameter);
            var expression = Expression.Lambda<AppendDelegate>(call, parameter);
            var func = expression.Compile();
            func(buffer);
        }
    }
}
