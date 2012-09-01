using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    internal sealed class NullPersistEvents : IPersistenceStrategy {
        public void Store(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend) {
            
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return Enumerable.Empty<IStreamedEvent>();
        }
    }
}
