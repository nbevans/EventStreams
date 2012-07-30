using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Projection {
    using Core;
    using Transformation;

    public class Projector : IProjector {
        private readonly EventInvocationCache _invocationCache = new EventInvocationCache();
        private readonly EventSequenceTransformer _eventSequenceTransformer = new EventSequenceTransformer();

        public IEventSequenceTransformer Transformations { get { return _eventSequenceTransformer; } }

        public IProjector Cache<TAggregateRoot>() where TAggregateRoot : class, new() {
            _invocationCache.Cache<TAggregateRoot>();
            return this;
        }

        public TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events) where TAggregateRoot : class, new() {
            return events
                .Aggregate(
                    new TAggregateRoot(),
                    (currentState, currentEvent) => {
                        _invocationCache
                            .Get<TAggregateRoot>(currentEvent)
                            .Invoke(currentState, currentEvent, new StreamingContext { Projecting = true });

                        return currentState;
                    });
        }
    }
}
