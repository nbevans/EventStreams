using System;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;

    public interface IEventTransformer {
        DateTime Chronology { get; }
        IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IStreamedEvent candidateEvent);
    }
}