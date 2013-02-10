using System;

using NUnit.Framework;

namespace EventStreams.Persistence.Riak.ClusterTools {

    [TestFixture]
    public class NetworkPartitionTests {

        [Test]
        [Ignore]
        public void Given_a_healthy_test_cluster_when_node_three_is_network_partitioned_then_expect_ring_status_to_become_degraded_and_when_unpartitioned_then_expect_cluster_to_return_to_full_health() {
            RingStatus.AssertOkay("riak03");
            
            using (NetworkPartition.Create("riak03")) {
                RingStatus.AssertDegraded("riak03");
            }

            RingReady.Wait("riak03");
            RingStatus.AssertOkay("riak03");
        }
    }
}
