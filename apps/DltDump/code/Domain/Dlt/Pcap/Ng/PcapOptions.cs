namespace RJCP.App.DltDump.Domain.Dlt.Pcap.Ng
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Options;
    using Resources;
    using RJCP.Core;

    /// <summary>
    /// A collection of options as decoded from a PCAP block.
    /// </summary>
    public class PcapOptions : IEnumerable<IPcapOption>
    {
        private readonly bool m_LittleEndian;
        private readonly List<IPcapOption> m_Options = new();
        private readonly PcapOptionFactory m_Factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapOptions"/> class.
        /// </summary>
        /// <param name="littleEndian">
        /// If <see langword="true"/>, then set binary decoding to be little endian, else binary decoding is big endian.
        /// </param>
        public PcapOptions(bool littleEndian)
        {
            m_LittleEndian = littleEndian;
            m_Factory = new PcapOptionFactory(littleEndian);
        }

        /// <summary>
        /// Decodes the option block TLV (Type, Length, Value) and adds to the collection.
        /// </summary>
        /// <param name="block">The PCAP-NG block number.</param>
        /// <param name="buffer">The buffer to decode.</param>
        /// <returns>
        /// Decodes and returns the decoded block, added to the end of this options collection. If the option is "end of
        /// options", then it is returned, but not added to the collection. If there was an error parsing the buffer,
        /// then <see langword="null"/> is returned (an exception is not raised for performance).
        /// </returns>
        /// <exception cref="InvalidOperationException">This collection is read only.</exception>
        public IPcapOption Add(int block, ReadOnlySpan<byte> buffer)
        {
            if (m_IsReadOnly) throw new InvalidOperationException(AppResources.InfraCollectionReadOnly);
            if (buffer.Length < 4) return null;

            IPcapOption option = DecodeOption(block, buffer);
            if (option is not null && option.OptionCode != 0) m_Options.Add(option);
            return option;
        }

        private IPcapOption DecodeOption(int block, ReadOnlySpan<byte> buffer)
        {
            int option = unchecked((ushort)BitOperations.To16Shift(buffer, m_LittleEndian));
            int length = unchecked((ushort)BitOperations.To16Shift(buffer[2..], m_LittleEndian));
            IPcapOption pcapOption = m_Factory.Create(block, option, length, buffer[4..]);
            return pcapOption;
        }

        /// <summary>
        /// Decodes a sequence of options and adds them to this collection. After decoding, this collection is made read
        /// only.
        /// </summary>
        /// <param name="block">The PCAP-NG block number.</param>
        /// <param name="buffer">The buffer to decode.</param>
        /// <returns>The length of the decoded data. If there was an error, -1 is returned.</returns>
        /// <exception cref="InvalidOperationException">This collection is read only.</exception>
        /// <remarks>
        /// All options in the buffer must be properly decoded, else no options will be added. When decoding the
        /// options, the list is cleared first.
        /// </remarks>
        public int Decode(int block, ReadOnlySpan<byte> buffer)
        {
            if (m_IsReadOnly) throw new InvalidOperationException(AppResources.InfraCollectionReadOnly);

            int length = 0;
            List<IPcapOption> options = new();
            while (buffer.Length >= 4) {
                IPcapOption option = DecodeOption(block, buffer);
                if (option is null) return -1;
                if (option.OptionCode != 0) options.Add(option);

                int optionLen = RoundInt32(option.Length + 4);
                length += optionLen;
                if (option.OptionCode == OptionCodes.EndOfOpt || buffer.Length < optionLen) {
                    SetOptions(options);
                    return length;
                }
                buffer = buffer[optionLen..];
            }

            SetOptions(options);
            return length;
        }

        private void SetOptions(List<IPcapOption> list)
        {
            m_Options.Clear();
            m_Options.AddRange(list);
            m_IsReadOnly = true;
        }

        private static int RoundInt32(int value)
        {
            return (value + 3) & ~0x3;
        }

        /// <summary>
        /// Gets the number of options decoded until now.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return m_Options.Count; } }

        private bool m_IsReadOnly;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance is read only; otherwise, <see langword="false"/>.</value>
        /// <exception cref="InvalidOperationException">
        /// Cannot set this property to <see langword="false"/> if this collection is already read only.
        /// </exception>
        /// <remarks>
        /// The collection can be marked to be read only once it is populated.
        /// </remarks>
        public bool IsReadOnly
        {
            get { return m_IsReadOnly; }
            set
            {
                if (m_IsReadOnly && !value)
                    throw new InvalidOperationException(AppResources.InfraCollectionReadOnly);
                m_IsReadOnly = value;
            }
        }

        /// <summary>
        /// Determines whether this collection contains the option code.
        /// </summary>
        /// <param name="optionCode">The option code to look for.</param>
        /// <returns>
        /// Is <see langword="true"/> if this collection contains the option code; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(int optionCode)
        {
            var query = from option in m_Options select option.OptionCode;
            return query.Contains(optionCode);
        }

        /// <summary>
        /// Gets the first index in this collection for the option code. Note, there may be further options.
        /// </summary>
        /// <param name="optionCode">The option code to get the index for.</param>
        /// <returns>The index in this collection where the option code is. If it is not found, then -1 is returned.</returns>
        public int IndexOf(int optionCode)
        {
            for (int i = 0; i < m_Options.Count; i++) {
                if (m_Options[i].OptionCode == optionCode) return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the index in this collection for the option code, starting from the given index.
        /// </summary>
        /// <param name="optionCode">The option code.</param>
        /// <param name="start">The index offset to start from.</param>
        /// <returns>
        /// The index in this collection where the option code is. If it is not found, then -1 is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="start"/> is out of the range of this collection.
        /// </exception>
        public int IndexOf(int optionCode, int start)
        {
            if (start >= m_Options.Count) return -1;

            for (int i = start; i < m_Options.Count; i++) {
                if (m_Options[i].OptionCode == optionCode) return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the <see cref="IPcapOption"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="IPcapOption"/> at the index given.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index"/> is out of the range of this collection.
        /// </exception>
        public IPcapOption this[int index]
        {
            get { return m_Options[index]; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IPcapOption> GetEnumerator()
        {
            return m_Options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
