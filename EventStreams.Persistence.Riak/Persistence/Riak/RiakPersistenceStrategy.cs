using System;
using System.Collections.Generic;

using CorrugatedIron;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;

namespace EventStreams.Persistence.Riak {
    using Core;
    using Serialization.Events;

    public class RiakPersistenceStrategy : IPersistenceStrategy {
        private readonly RiakStorer _storer;
        private readonly RiakLoader _loader;
        private readonly IRiakClusterConfiguration _riakClusterConfiguration;
        private readonly IRiakClient _riakClient;

        public RiakPersistenceStrategy(IRiakClusterConfiguration riakClusterConfiguration, IEventReaderWriterPair eventReaderWriterPair) {
            if (riakClusterConfiguration == null) throw new ArgumentNullException("riakClusterConfiguration");
            if (eventReaderWriterPair == null) throw new ArgumentNullException("eventReaderWriterPair");

            _riakClient = new RiakCluster(riakClusterConfiguration, new RiakConnectionFactory()).CreateClient();

            _storer = new RiakStorer(_riakClient, eventReaderWriterPair.Writer);
            _loader = new RiakLoader(_riakClient, eventReaderWriterPair.Reader);
        }

        public void Store(Guid identity, IEnumerable<IStreamedEvent> eventsToAppend) {
            _storer.Store(identity, eventsToAppend);
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return _loader.Load(identity);
        }
    }
}
