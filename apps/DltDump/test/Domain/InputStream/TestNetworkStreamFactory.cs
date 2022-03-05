namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public class TestNetworkStreamFactory : InputStreamFactoryBase
    {
        /// <summary>
        /// Used to simulate an error on opening a file name.
        /// </summary>
        /// <value>The exception that should be simulated when opening a file.</value>
        public bool ConnectResult { get; set; } = true;

        public override IInputStream Create(Uri uri)
        {
            return new TestNetworkStream(ConnectResult);
        }
    }
}
