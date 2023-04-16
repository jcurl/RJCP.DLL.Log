namespace RJCP.Diagnostics.Log.Dlt.NonVerbose
{
    using System.Collections.Generic;
    using RJCP.Diagnostics.Log.Dlt;

    public class TestFrame : IFrame
    {
        public TestFrame() { }

        public TestFrame(int id)
        {
            Id = id;
        }

        public string EcuId { get; set; }

        public int Id { get; set; }

        public string ApplicationId { get; set; }

        public string ContextId { get; set; }

        public DltType MessageType { get; set; }

        public IReadOnlyList<IPdu> Arguments { get { return null; } }
    }
}
