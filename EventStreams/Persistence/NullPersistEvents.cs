using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    internal sealed class NullPersistEvents : IPersistEvents {
        public void Persist(IAggregateRoot aggregateRoot, IEnumerable<EventArgs> eventsToAppend) {
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return Enumerable.Empty<IStreamedEvent>();
        }
    }
}
