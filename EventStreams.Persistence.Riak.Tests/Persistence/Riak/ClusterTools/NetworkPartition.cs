using System;

namespace EventStreams.Persistence.Riak.ClusterTools {
    internal class NetworkPartition : IDisposable {
        private readonly string _nodeName;
        private bool _closed;

        private NetworkPartition(string nodeName) {
            _nodeName = nodeName;
        }

        /// <summary>
        /// Creates a temporary network partition by segregating (at the IP firewall level) a particular node from all other nodes in the cluster.
        /// </summary>
        /// <param name="nodeName">The name of the node to be network partitioned. The name must exist as a PuTTY "Saved Session".</param>
        /// <returns>An object that can be disposed when the partition is to be removed.</returns>
        public static IDisposable Create(string nodeName) {
            var np = new NetworkPartition(nodeName);
            Plink.Execute("simulate-network-partition.sh", nodeName);
            return np;
        }

        private void Close() {
            if (_closed)
                return;

            Plink.Execute("restart-iptables.sh", _nodeName);

            _closed = true;
        }

        public void Dispose() {
            Close();
        }
    }
}
