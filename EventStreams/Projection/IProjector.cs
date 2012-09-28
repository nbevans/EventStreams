using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Transformation;

    public interface IProjector {
        IEventSequenceTransformer Transformations { get; }

        TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events)
            where TAggregateRoot : class, IObserver<EventArgs>, new();

        TAggregateRoot Project<TAggregateRoot>(Guid identity, IEnumerable<IStreamedEvent> events)
            where TAggregateRoot : class, IObserver<EventArgs>, new();
    }
}
