using System;
using System.Collections.Generic;

namespace EventStreams.Persistence {
    using Core;

    public interface IStorer {
        void Store(Guid identity, IEnumerable<IStreamedEvent> eventsToAppend);
    }
}
