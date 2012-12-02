using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    public sealed class NullPersistenceStrategy : IPersistenceStrategy {
        public void Store(Guid identity, IEnumerable<IStreamedEvent> eventsToAppend) {
            
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return Enumerable.Empty<IStreamedEvent>();
        }
    }
}
