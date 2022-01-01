namespace RJCP.Diagnostics.Log.Dlt.ControlArgs
{
    public class CustomControlResponse : IControlArg
    {
        public int ServiceId
        {
            get { return 0x1000; }
        }

        public DltType DefaultType
        {
            get { return DltType.CONTROL_RESPONSE; }
        }

        public override string ToString()
        {
            return "[custom_control_response]";
        }
    }
}
