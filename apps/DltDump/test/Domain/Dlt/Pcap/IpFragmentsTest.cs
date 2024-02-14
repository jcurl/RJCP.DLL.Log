namespace RJCP.App.DltDump.Domain.Dlt.Pcap
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class IpFragmentsTest
    {
        [Test]
        public void DefaultIpFragments()
        {
            IpFragments fragments = new(10);
            Assert.That(fragments.FragmentId, Is.EqualTo(10));
            Assert.That(fragments.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragments.GetFragments(), Is.Empty);
            Assert.That(fragments.Length, Is.EqualTo(0));
        }

        private static readonly byte[] Fragment1 = new byte[] {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07
        };

        private static readonly byte[] Fragment2 = new byte[] {
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        private static readonly byte[] Fragment3 = new byte[] {
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17
        };

        private static int GetChecksum(ReadOnlySpan<byte> buffer)
        {
            unchecked {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < buffer.Length; i++) {
                    hash = (hash ^ buffer[i]) * p;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        private static readonly DateTime Time1 = new(2022, 6, 11, 20, 36, 10);
        private static readonly DateTime Time2 = new(2022, 6, 11, 20, 36, 11);
        private static readonly DateTime Time3 = new(2022, 6, 11, 20, 36, 12);
        private static readonly DateTime TimeOld = new(2022, 6, 11, 20, 36, 30);

        [Test]
        public void IpFragments2PiecesForward()
        {
            IpFragments fragments = new(20);
            Assert.That(fragments.AddFragment(0, true, 0x00, Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, false, 0x01, Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Reassembled));
            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(20));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(16));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void IpFragments2PiecesReverse()
        {
            IpFragments fragments = new(20);
            Assert.That(fragments.AddFragment(8, false, 0x01, Fragment2, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, 0x00, Fragment1, Time2, 16), Is.EqualTo(IpFragmentResult.Reassembled));
            Assert.That(fragments.TimeStamp, Is.EqualTo(Time2));
            Assert.That(fragments.FragmentId, Is.EqualTo(20));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(16));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(0));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [TestCase(0, TestName = "IpFragments3Pieces123")]
        [TestCase(1, TestName = "IpFragments3Pieces132")]
        [TestCase(2, TestName = "IpFragments3Pieces213")]
        [TestCase(3, TestName = "IpFragments3Pieces231")]
        [TestCase(4, TestName = "IpFragments3Pieces312")]
        [TestCase(5, TestName = "IpFragments3Pieces321")]
        public void IpFragments3Pieces(int order)
        {
            int pos1, pos2, pos3;
            DateTime t1, t2, t3;
            IpFragments fragments = new(20);
            switch (order) {
            case 0:
                pos1 = 0; pos2 = 16; pos3 = 32;
                t1 = Time1; t2 = Time2; t3 = Time3;
                Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, t1, pos1), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, t2, pos2), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, t3, pos3), Is.EqualTo(IpFragmentResult.Reassembled));
                break;
            case 1:
                pos1 = 0; pos2 = 32; pos3 = 16;
                t1 = Time1; t2 = Time3; t3 = Time2;
                Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, t1, pos1), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, t3, pos3), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, t2, pos2), Is.EqualTo(IpFragmentResult.Reassembled));
                break;
            case 2:
                pos1 = 16; pos2 = 0; pos3 = 32;
                t1 = Time2; t2 = Time1; t3 = Time3;
                Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, t2, pos2), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, t1, pos1), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, t3, pos3), Is.EqualTo(IpFragmentResult.Reassembled));
                break;
            case 3:
                pos1 = 32; pos2 = 0; pos3 = 16;
                t1 = Time3; t2 = Time1; t3 = Time2;
                Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, t2, pos2), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, t3, pos3), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, t1, pos1), Is.EqualTo(IpFragmentResult.Reassembled));
                break;
            case 4:
                pos1 = 16; pos2 = 32; pos3 = 0;
                t1 = Time2; t2 = Time3; t3 = Time1;
                Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, t3, pos3), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, t1, pos1), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, t2, pos2), Is.EqualTo(IpFragmentResult.Reassembled));
                break;
            case 5:
                pos1 = 32; pos2 = 16; pos3 = 0;
                t1 = Time3; t2 = Time2; t3 = Time1;
                Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, t3, pos3), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, t2, pos2), Is.EqualTo(IpFragmentResult.Incomplete));
                Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, t1, pos1), Is.EqualTo(IpFragmentResult.Reassembled));
                break;
            default:
                throw new InvalidOperationException("Undefined ordering for test case");
            }
            Assert.That(fragments.TimeStamp, Is.EqualTo(t1));
            Assert.That(fragments.FragmentId, Is.EqualTo(20));
            Assert.That(fragments.Length, Is.EqualTo(24));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(3));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(pos1));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(pos2));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
            Assert.That(list[2].FragmentOffset, Is.EqualTo(16));
            Assert.That(list[2].Position, Is.EqualTo(pos3));
            Assert.That(list[2].GetArray(), Is.EqualTo(Fragment3).AsCollection);
        }

        [Test]
        public void ExpiredFragments()
        {
            IpFragments fragments = new(30);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, TimeOld, 128), Is.EqualTo(IpFragmentResult.InvalidTimeOut));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(30));
            Assert.That(fragments.Length, Is.EqualTo(16));

            // The last fragment wasn't added, as it was much newer (15 seconds or more) than the last fragment. The
            // existing collection should remain intact, so that it can be logged what is being dropped.
            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(16));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);

            // The user would now be expected to create a new IpFragments object and add the fragment again. This is
            // tested properly in another test case.
            IpFragments fragmentsNew = new(30);
            Assert.That(fragmentsNew.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, TimeOld, 128), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragmentsNew.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragmentsNew.FragmentId, Is.EqualTo(30));
            Assert.That(fragmentsNew.Length, Is.EqualTo(8));
        }

        [Test]
        public void ExpiredFragmentsFirst()
        {
            IpFragments fragments = new(30);
            Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, TimeOld, 32), Is.EqualTo(IpFragmentResult.InvalidTimeOut));

            Assert.That(fragments.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragments.FragmentId, Is.EqualTo(30));
            Assert.That(fragments.Length, Is.EqualTo(16));

            // The last fragment wasn't added, as it was much newer (15 seconds or more) than the last fragment. The
            // existing collection should remain intact, so that it can be logged what is being dropped.
            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[0].Position, Is.EqualTo(16));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment2).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(16));
            Assert.That(list[1].Position, Is.EqualTo(0));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment3).AsCollection);
        }

        [Test]
        public void DuplicateFragmentFirst()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidDuplicate));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
        }

        [Test]
        public void OverlapFragmentFirst()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidOverlap));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
        }

        [Test]
        public void DuplicateFragmentFirst_2()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time3, 32), Is.EqualTo(IpFragmentResult.InvalidDuplicate));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(16));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void OverlapFragmentFirst_2()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment3), Fragment3, Time3, 32), Is.EqualTo(IpFragmentResult.InvalidOverlap));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(16));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void DuplicateFragmentSecond()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time3, 32), Is.EqualTo(IpFragmentResult.InvalidDuplicate));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time2));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(16));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(0));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void OverlapFragmentSecond()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment3), Fragment3, Time3, 32), Is.EqualTo(IpFragmentResult.InvalidOverlap));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time2));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(16));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(0));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void OverlapFragmentPartially()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(12, true, GetChecksum(Fragment2), Fragment2, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time2, 20), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, Time3, 36), Is.EqualTo(IpFragmentResult.InvalidOffset));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time2));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(20));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(12));
            Assert.That(list[1].Position, Is.EqualTo(0));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void OverlapFragmentPartiallyFirst()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(4, true, GetChecksum(Fragment2), Fragment2, Time2, 12), Is.EqualTo(IpFragmentResult.InvalidOverlap));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
        }

        [Test]
        public void OverlapFragmentPartiallySecond()
        {
            IpFragments fragments = new(40);
            Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(12, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidOverlap));

            Assert.That(fragments.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragments.FragmentId, Is.EqualTo(40));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(16));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment3).AsCollection);
        }

        [Test]
        public void AddAfterLastFragment()
        {
            IpFragments fragments = new(50);
            Assert.That(fragments.AddFragment(8, false, GetChecksum(Fragment2), Fragment2, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(16, true, GetChecksum(Fragment3), Fragment3, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidOffset));

            Assert.That(fragments.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragments.FragmentId, Is.EqualTo(50));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void AddLastFragmentNotAtEnd()
        {
            IpFragments fragments = new(50);
            Assert.That(fragments.AddFragment(16, true, GetChecksum(Fragment3), Fragment3, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, false, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidOffset));

            Assert.That(fragments.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragments.FragmentId, Is.EqualTo(50));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(16));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment3).AsCollection);
        }

        [Test]
        public void AddLastFragmentTwice()
        {
            IpFragments fragments = new(60);
            Assert.That(fragments.AddFragment(8, false, GetChecksum(Fragment2), Fragment2, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, false, GetChecksum(Fragment3), Fragment3, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidOffset));

            Assert.That(fragments.TimeStamp.Ticks, Is.EqualTo(0));
            Assert.That(fragments.FragmentId, Is.EqualTo(60));
            Assert.That(fragments.Length, Is.EqualTo(8));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void OverlapExistingPacket()
        {
            IpFragments fragments = new(60);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(1, true, GetChecksum(Fragment3), Fragment3.AsSpan(0, 4), Time3, 40), Is.EqualTo(IpFragmentResult.InvalidOverlap));

            Assert.That(fragments.TimeStamp, Is.EqualTo(Time1));
            Assert.That(fragments.FragmentId, Is.EqualTo(60));
            Assert.That(fragments.Length, Is.EqualTo(16));

            List<IpFragment> list = new(fragments.GetFragments());
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].FragmentOffset, Is.EqualTo(0));
            Assert.That(list[0].Position, Is.EqualTo(0));
            Assert.That(list[0].GetArray(), Is.EqualTo(Fragment1).AsCollection);
            Assert.That(list[1].FragmentOffset, Is.EqualTo(8));
            Assert.That(list[1].Position, Is.EqualTo(16));
            Assert.That(list[1].GetArray(), Is.EqualTo(Fragment2).AsCollection);
        }

        [Test]
        public void IpFragmentsDuplicated()
        {
            IpFragments fragments = new(20);
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(0, true, GetChecksum(Fragment1), Fragment1, Time1, 0), Is.EqualTo(IpFragmentResult.InvalidDuplicate));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.Incomplete));
            Assert.That(fragments.AddFragment(8, true, GetChecksum(Fragment2), Fragment2, Time2, 16), Is.EqualTo(IpFragmentResult.InvalidDuplicate));
            Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, Time3, 32), Is.EqualTo(IpFragmentResult.Reassembled));
            Assert.That(fragments.AddFragment(16, false, GetChecksum(Fragment3), Fragment3, Time3, 32), Is.EqualTo(IpFragmentResult.InvalidDuplicate));
        }
    }
}
