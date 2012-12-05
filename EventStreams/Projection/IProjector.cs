using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Core.Domain;
    using Transformation;

    public interface IProjector {
        IEventSequenceTransformer Transformations { get; }

        TModel Project<TModel>(IEnumerable<IStreamedEvent> events, Func<TModel, EventHandler<TModel>> eventHandlerFactory)
            where TModel : class;

        TModel Project<TModel>(Guid identity, IEnumerable<IStreamedEvent> events, Func<TModel, EventHandler<TModel>> eventHandlerFactory)
            where TModel : class;
    }
}
