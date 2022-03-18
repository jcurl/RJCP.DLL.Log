namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    internal static class LabelGenFactory
    {
        private static readonly object m_LabelLock = new object();
        private static readonly Dictionary<ILGenerator, int> m_LabelNum = new Dictionary<ILGenerator, int>();

        public static LabelGen Create(ILGenerator ilGen)
        {
            int num;
            lock (m_LabelLock) {
                if (!m_LabelNum.TryGetValue(ilGen, out num)) {
                    m_LabelNum.Add(ilGen, 2);
                    num = 1;
                } else {
                    m_LabelNum[ilGen] = num + 1;
                }
            }
            return new LabelGen(ilGen, num);
        }
    }
}
