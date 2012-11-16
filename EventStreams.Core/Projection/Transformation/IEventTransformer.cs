using System;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;
    using Core.Domain;

    public interface IEventTransformer {
        DateTime Chronology { get; }
        IEnumerable<IStreamedEvent> Transform<TEventSourced>(IStreamedEvent candidateEvent) where TEventSourced : IEventSourced;
    }
}