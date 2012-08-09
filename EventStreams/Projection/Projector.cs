using System;
using System.Linq;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Transformation;

    public class Projector : IProjector {
        private readonly EventSequenceTransformer _eventSequenceTransformer = new EventSequenceTransformer();

        public IEventSequenceTransformer Transformations { get { return _eventSequenceTransformer; } }

        public TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events) where TAggregateRoot : class, IObserver<EventArgs>, new() {
            var aggregateRoot =
                _eventSequenceTransformer
                    .Transform<TAggregateRoot>(events)
                    .Aggregate(
                        new TAggregateRoot(),
                        (currentState, currentEvent) => {
                            currentState.OnNext(currentEvent.Arguments);
                            return currentState;
                        }
                    );

            aggregateRoot.OnCompleted();

            return aggregateRoot;
        }
    }
}