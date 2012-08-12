using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    public interface IPersistEvents {
        void Persist(IAggregateRoot aggregateRoot, IEnumerable<EventArgs> eventsToAppend);
        IEnumerable<IStreamedEvent> Load(Guid identity);
    }
}
