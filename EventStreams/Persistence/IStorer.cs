using System;
using System.Collections.Generic;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    public interface IStorer {
        void Store(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend);
    }
}
