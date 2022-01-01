namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    public class CustomControlRequest : IControlArg
    {
        public int ServiceId
        {
            get { return 0x1000; }
        }

        public DltType DefaultType
        {
            get { return DltType.CONTROL_REQUEST; }
        }

        public override string ToString()
        {
            return "[custom_control_request]";
        }
    }
}
