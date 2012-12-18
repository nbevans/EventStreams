using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using EventHandling;
    using Transformation;

    public interface IProjector {
        IEventSequenceTransformer Transformations { get; }

        TModel Project<TModel>(IEnumerable<IStreamedEvent> events, Func<TModel, EventHandler> eventHandlerFactory)
            where TModel : class;

        TModel Project<TModel>(Guid identity, IEnumerable<IStreamedEvent> events, Func<TModel, EventHandler> eventHandlerFactory)
            where TModel : class;
    }
}
