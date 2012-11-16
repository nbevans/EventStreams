using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Core.Domain;
    using Transformation;

    public interface IProjector {
        IEventSequenceTransformer Transformations { get; }

        TEventSourced Project<TEventSourced>(IEnumerable<IStreamedEvent> events, Func<TEventSourced, EventHandler<TEventSourced>> eventHandlerFactory)
            where TEventSourced : class, IEventSourced, new();

        TEventSourced Project<TEventSourced>(Guid identity, IEnumerable<IStreamedEvent> events, Func<TEventSourced, EventHandler<TEventSourced>> eventHandlerFactory)
            where TEventSourced : class, IEventSourced, new();
    }
}
