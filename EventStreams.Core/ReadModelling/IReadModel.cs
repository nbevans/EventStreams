using System;
using System.Collections.Generic;

namespace EventStreams.ReadModelling {
    using Core;

    public interface IReadModel {
        IEnumerable<IStreamedEvent> Read<TAggregateRoot>(IStreamedEvent candidateEvent);
    }
}