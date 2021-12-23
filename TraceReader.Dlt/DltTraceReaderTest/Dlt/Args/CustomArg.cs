namespace RJCP.Diagnostics.Log.Dlt.Args
{
    using System.Text;

    public class CustomArg : IDltArg
    {
        private readonly string m_Arg;

        public CustomArg()
        {
            m_Arg = "custom";
        }

        public CustomArg(string arg)
        {
            m_Arg = arg ?? string.Empty;
        }

        public StringBuilder Append(StringBuilder strBuilder)
        {
            return strBuilder.Append(ToString());
        }

        public override string ToString()
        {
            return m_Arg;
        }
    }
}
