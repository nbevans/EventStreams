using System;

using NUnit.Framework;

namespace EventStreams.Persistence.Riak.ClusterTools {

    [TestFixture]
    public class RingReadyTests {

        [Test]
        [Ignore]
        public void Test_ring_is_ready() {
            var r = RingReady.Test("riak03");
            Assert.IsTrue(r);
        }

        [Test]
        [Ignore]
        public void Test_ring_is_not_ready() {
            var r = RingReady.Test("riak03");
            Assert.IsFalse(r);
        }

        [Test]
        [Ignore]
        public void Wait_for_ring_to_become_ready() {
            RingReady.Wait("riak03");
        }
    }
}
