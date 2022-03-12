namespace RJCP.App.DltDump.Domain.InputStream
{
    using System;

    public class ConnectSuccessEventArgs : EventArgs
    {
        public bool Succeed { get; set; } = true;
    }
}
