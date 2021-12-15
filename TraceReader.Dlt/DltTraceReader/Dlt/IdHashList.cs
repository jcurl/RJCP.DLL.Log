namespace RJCP.Diagnostics.Log.Dlt
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// DLT 4-byte ID hashing mechanism for efficiently parsing a large number of IDs.
    /// </summary>
    /// <remarks>
    /// DLT IDs are used very often. Reduce the overall GC load by only creating a string if it is unique, else return
    /// an already prepared string. Each DLT line can generate 3 IDs, which can translate to (26 + length*2) [1] per
    /// string (120 bytes on a 64-bit machine). Do this for a million lines, and that's 114MB of wasted space just
    /// there. Most identifiers repeat a lot.
    /// <list type="bullet">
    /// <item>[1] https://codeblog.jonskeet.uk/2011/04/05/of-memory-and-strings/</item>
    /// </list>
    /// </remarks>
    internal class IdHashList
    {
        public static IdHashList Instance { get; } = new IdHashList();

        private struct IdEntry
        {
            public int Identifier { get; private set; }

            public string Value { get; private set; }

            public IdEntry(int identifier, string value)
            {
                Identifier = identifier;
                Value = value;
            }
        }

        private readonly List<IdEntry>[] m_Entries = new List<IdEntry>[65536];

        /// <summary>
        /// Initializes a new instance of the <see cref="IdHashList"/> class.
        /// </summary>
        public IdHashList()
        {
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < m_Entries.Length; i++) {
                m_Entries[i] = new List<IdEntry>(3);
            }
        }

        /// <summary>
        /// Parses the 4-byte identifier.
        /// </summary>
        /// <param name="id">The integer representing the 4-byte ASCII ID.</param>
        /// <returns>The string representing the ID.</returns>
        public string ParseId(int id)
        {
            int index = GetIndex(id);
            List<IdEntry> bucket = m_Entries[index];

            for (int i = 0; i < bucket.Count; i++) {
                if (bucket[i].Identifier == id) return bucket[i].Value;
            }

            string value = GetValue(id);
            bucket.Add(new IdEntry(id, value));
            return value;
        }

        private static ushort GetIndex(int index)
        {
            return unchecked((ushort)(((index & 0x7F7F) ^ (index >> 16)) & 0x7F7F));
        }

        private static string GetValue(int id)
        {
            // If something other than ASCII is wanted, implement a table that converts via a look up to a char.
            // Change the GetIndex function to mask 0xFFFF instead of 0x7F7F to use the full character range and
            // avoid collisions.

            Span<char> value = stackalloc char[4];

            value[0] = unchecked((char)((id >> 24) & 0x7F));
            if (value[0] == 0x00) return string.Empty;

            value[1] = unchecked((char)((id >> 16) & 0x7F));
            if (value[1] == 0x00) return value[0..1].ToString();

            value[2] = unchecked((char)((id >> 8) & 0x7F));
            if (value[2] == 0x00) return value[0..2].ToString();

            value[3] = unchecked((char)(id & 0x7F));
            if (value[3] == 0x00) return value[0..3].ToString();

            return value.ToString();
        }
    }
}
