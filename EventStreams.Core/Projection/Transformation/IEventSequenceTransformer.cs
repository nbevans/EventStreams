using System;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;

    public interface IEventSequenceTransformer {
        IEventSequenceTransformer Bind<TEventTransformer>() where TEventTransformer : class, IEventTransformer, new();
        IEnumerable<IStreamedEvent> Transform<TModel>(IEnumerable<IStreamedEvent> events);
    }
}
