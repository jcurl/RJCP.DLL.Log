namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A list of IPv4 fragments that are reassembled as the fragments arrive.
    /// </summary>
    /// <remarks>
    /// Each fragment for a particular fragment identification, source address, destination address and protocol number
    /// that arrives, should be added to a collection of <see cref="IpFragments"/> for reassembly and error checking. If
    /// an error occurs, this collection should be discarded as incomplete, and a new collection should be created with
    /// the fragment added there.
    /// </remarks>
    public class IpFragments
    {
        private const int TimeOut = 15000;  // No more than 15 seconds should pass, else we assume that packets were lost.

        private readonly List<IpFragment> m_Fragments = new List<IpFragment>();
        private bool m_HasLastFragment;
        private DateTime m_LastTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpFragments"/> class for the IPv4 packet identifier.
        /// </summary>
        /// <param name="fragmentId">The fragment identifier.</param>
        public IpFragments(int fragmentId)
        {
            FragmentId = fragmentId;
        }

        /// <summary>
        /// Gets the fragment identifier.
        /// </summary>
        /// <value>The fragment identifier.</value>
        public int FragmentId { get; }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>The time stamp. This is the default value if not yet defined.</value>
        public DateTime TimeStamp { get; private set; }

        private bool IsExpired(DateTime timeStamp)
        {
            if (m_Fragments.Count != 0) {
                TimeSpan difference = timeStamp - m_LastTime;
                if (Math.Abs(difference.TotalMilliseconds) > TimeOut) return true;
            }

            m_LastTime = timeStamp;
            return false;
        }

        private IpFragmentResult IsReassembled()
        {
            if (!m_HasLastFragment) return IpFragmentResult.Incomplete;

            int expectedOffset = 0;
            foreach (IpFragment fragment in m_Fragments) {
                if (fragment.FragmentOffset != expectedOffset)
                    return IpFragmentResult.Incomplete;
                expectedOffset += fragment.Buffer.Length;
            }

            return IpFragmentResult.Reassembled;
        }

        /// <summary>
        /// Adds the fragment, for the non-zero offset.
        /// </summary>
        /// <param name="fragOffset">The fragment offset.</param>
        /// <param name="mf">
        /// Indicates if more fragments are expected and is <see langword="true"/>, else <see langword="false"/>
        /// indicates this is the last fragment
        /// </param>
        /// <param name="buffer">The buffer containing the fragment, in the payload itself.</param>
        /// <param name="timeStamp">The time stamp of the packet containing this payload.</param>
        /// <param name="position">The position in the stream for this payload.</param>
        /// <returns>
        /// An indication of success.
        /// <list type="bullet">
        /// <item>If the result is <see cref="IpFragmentResult.Incomplete"/>, more fragments are still expected.</item>
        /// <item>
        /// If the result is <see cref="IpFragmentResult.Reassembled"/>, get the reassembled fragments with
        /// <see cref="GetFragments()"/>.
        /// </item>
        /// <item>All other results indicate an error. A new <see cref="IpFragments"/> object should be created.</item>
        /// </list>
        /// </returns>
        public IpFragmentResult AddFragment(int fragOffset, bool mf, ReadOnlySpan<byte> buffer, DateTime timeStamp, long position)
        {
            if (IsExpired(timeStamp)) return IpFragmentResult.InvalidTimeOut;

            IpFragment fragment = new IpFragment(fragOffset, buffer, position);
            if (m_Fragments.Count == 0) {
                m_Fragments.Add(fragment);
                m_HasLastFragment = !mf;
            } else if (!mf) {
                // There is only one last fragment.
                if (m_HasLastFragment) return IpFragmentResult.InvalidDuplicateLastPacket;

                // We expect that if this is the last fragment, it must be at the end of the list.
                if (fragOffset < m_Fragments[^1].FragmentOffset + m_Fragments[^1].Buffer.Length)
                    return IpFragmentResult.InvalidOffset;

                m_Fragments.Add(fragment);
                m_HasLastFragment = true;
            } else {
                bool inserted = false;
                for (int i = 0; i < m_Fragments.Count; i++) {
                    int sOffset = m_Fragments[i].FragmentOffset;
                    int sLength = m_Fragments[i].Buffer.Length;

                    if (fragOffset + buffer.Length <= sOffset) {
                        m_Fragments.Insert(i, fragment);
                        inserted = true;
                        break;
                    }

                    if (fragOffset < sOffset + sLength) return IpFragmentResult.InvalidOverlap;
                }
                if (!inserted) {
                    // If we have the last fragment, adding it to the end would mean that we didn't have the last
                    // fragment. Thus the fragment offset is bad.
                    if (m_HasLastFragment) return IpFragmentResult.InvalidOffset;

                    m_Fragments.Add(fragment);
                }
            }

            if (fragOffset == 0) TimeStamp = timeStamp;
            return IsReassembled();
        }

        /// <summary>
        /// Gets the fragments.
        /// </summary>
        /// <returns>An enumerable providing the fragments in the order they occur.</returns>
        public IEnumerable<IpFragment> GetFragments()
        {
            return m_Fragments;
        }
    }
}
