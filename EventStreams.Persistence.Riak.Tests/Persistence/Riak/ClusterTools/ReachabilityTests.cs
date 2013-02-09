using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EventStreams.Persistence.Riak.ClusterTools {

    [TestFixture]
    public class ReachabilityTests {

        [Test]
        [Ignore]
        public void Test_cluster_is_healthy_and_all_nodes_are_up() {
            IEnumerable<string> unreachableNodes;
            var result = Reachability.Test("riak03", out unreachableNodes);
            Assert.IsTrue(result == Health.Okay);
        }

        [Test]
        [Ignore]
        public void Test_cluster_is_degraded_and_that_there_are_some_unreachable_nodes() {
            IEnumerable<string> unreachableNodes;
            var result = Reachability.Test("riak03", out unreachableNodes);
            Assert.IsTrue(result == Health.Degraded);
        }
    }
}
