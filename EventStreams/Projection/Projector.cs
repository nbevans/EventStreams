﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Transformation;

    public class Projector : IProjector {
        //private readonly EventInvocationCache _invocationCache = new EventInvocationCache();
        private readonly EventSequenceTransformer _eventSequenceTransformer = new EventSequenceTransformer();

        public IEventSequenceTransformer Transformations { get { return _eventSequenceTransformer; } }

        public IProjector Cache<TAggregateRoot>() where TAggregateRoot : class, new() {
            //_invocationCache.Cache<TAggregateRoot>();
            return this;
        }

        public TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events) where TAggregateRoot : class, IObserver<IStreamedEvent>, new() {
            var aggregateRoot =
                _eventSequenceTransformer
                    .Transform<TAggregateRoot>(events)
                    .Aggregate(
                        new TAggregateRoot(),
                        (currentState, currentEvent) => {
                            currentState.OnNext(currentEvent);
                            return currentState;
                        }
                    );

            aggregateRoot.OnCompleted();

            return aggregateRoot;
        }
    }
}