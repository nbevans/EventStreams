using System;

using NUnit.Framework;

namespace EventStreams.Persistence.Riak.ClusterTools {

    [TestFixture]
    public class NetworkPartitionTests {

        [Test]
        [Ignore]
        public void Create() {
            Reachability.AssertOkay("riak03");
            
            using (NetworkPartition.Create("riak03")) {
                Reachability.AssertDegraded("riak03");
            }

            Reachability.AssertOkay("riak03");
        }
    }
}
