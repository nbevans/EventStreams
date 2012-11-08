using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Core.Domain;
    using Transformation;

    public interface IProjector {
        IEventSequenceTransformer Transformations { get; }

        TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events)
            where TAggregateRoot : class, IAggregateRoot, new();

        TAggregateRoot Project<TAggregateRoot>(Guid identity, IEnumerable<IStreamedEvent> events)
            where TAggregateRoot : class, IAggregateRoot, new();
    }
}
