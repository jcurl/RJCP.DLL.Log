namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Reflection.Emit;

    internal class LabelGen
    {
        private readonly ILGenerator m_IlGen;

        public LabelGen(ILGenerator ilGen, int labelNum)
        {
            m_IlGen = ilGen;
            LabelNum = labelNum;
        }

        private Label m_Label;

        public int LabelNum { get; }

        public bool IsLabelled { get; private set; }

        public Label Label
        {
            get
            {
                if (!IsLabelled) {
                    m_Label = m_IlGen.DefineLabel();
                    IsLabelled = true;
                }
                return m_Label;
            }
        }
    }
}
