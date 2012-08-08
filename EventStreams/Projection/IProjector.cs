using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Transformation;

    public interface IProjector {
        IEventSequenceTransformer Transformations { get; }
        IProjector Cache<TAggregateRoot>() where TAggregateRoot : class, new();
        TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events)
            where TAggregateRoot : class, IObserver<IStreamedEvent>, new();
    }
}
