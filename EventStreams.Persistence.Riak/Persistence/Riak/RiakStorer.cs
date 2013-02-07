using System;
using System.Collections.Generic;

using CorrugatedIron;

namespace EventStreams.Persistence.Riak {
    using Committers;
    using Core;
    using Serialization.Events;

    public class RiakStorer : IStorer {
        private readonly IRiakClient _riakClient;
        private readonly IEventWriter _eventWriter;
        
        public RiakStorer(IRiakClient riakClient, IEventWriter eventWriter) {
            if (riakClient == null) throw new ArgumentNullException("riakClient");
            if (eventWriter == null) throw new ArgumentNullException("eventWriter");
            _riakClient = riakClient;
            _eventWriter = eventWriter;
        }

        public void Store(Guid identity, IEnumerable<IStreamedEvent> eventsToAppend) {
            var bucket = identity.ToRiakIdentity();

            new ConservativeCommit<IStreamedEvent>(_riakClient, bucket, eventsToAppend, e => e.ToRiakIdentity())
                .Commit();
        }
    }
}
