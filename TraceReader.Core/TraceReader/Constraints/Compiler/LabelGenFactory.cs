namespace RJCP.Diagnostics.Log.Constraints.Compiler
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

#if NET10_0_OR_GREATER
    using Lock = System.Threading.Lock;
#else
    using Lock = System.Object;
#endif

    internal static class LabelGenFactory
    {
        private static readonly Lock m_LabelLock = new();
        private static readonly Dictionary<ILGenerator, int> m_LabelNum = new();

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
