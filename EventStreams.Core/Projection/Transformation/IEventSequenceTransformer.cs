using System;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;
    using Core.Domain;

    public interface IEventSequenceTransformer {
        IEventSequenceTransformer Bind<TEventTransformer>() where TEventTransformer : class, IEventTransformer, new();
        IEnumerable<IStreamedEvent> Transform<TEventSourced>(IEnumerable<IStreamedEvent> events) where TEventSourced : IEventSourced;
    }
}
