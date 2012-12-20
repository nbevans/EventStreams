using System;
using System.Collections.Generic;

namespace EventStreams.Persistence.Riak {
    using Core;
    using Serialization.Events;

    public class RiakStorer : IStorer {
        private readonly IEventWriter _eventWriter;

        public RiakStorer(IEventWriter eventWriter) {
            if (eventWriter == null) throw new ArgumentNullException("eventWriter");
            _eventWriter = eventWriter;
        }

        public void Store(Guid identity, IEnumerable<IStreamedEvent> eventsToAppend) {
        }
    }
}
