namespace RJCP.Diagnostics.Log.Dlt.ArgEncoder
{
    using System;

    public abstract class ArgEncoderTestBase<TArgEncoder> where TArgEncoder : IArgEncoder
    {
        protected IArgEncoder GetEncoder()
        {
            return Activator.CreateInstance<TArgEncoder>();
        }
    }
}
