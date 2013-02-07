using System;

using CorrugatedIron;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;

namespace EventStreams.Persistence.Riak {

    internal static class TestingRiakClient {
        public static IRiakClient Get() {
            var riakClusterConfiguration = RiakClusterConfiguration.LoadFromConfig("riakConfig");
            var riakClient = new RiakCluster(riakClusterConfiguration, new RiakConnectionFactory()).CreateClient();

            return riakClient;
        }
    }
}
