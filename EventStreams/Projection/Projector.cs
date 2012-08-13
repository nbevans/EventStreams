using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Projection {
    using Core;
    using Transformation;

    // ReSharper disable PossibleMultipleEnumeration
    public class Projector : IProjector {

        private readonly IEventSequenceTransformer _eventSequenceTransformer = new EventSequenceTransformer();

        public IEventSequenceTransformer Transformations { get { return _eventSequenceTransformer; } }

        public TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events) where TAggregateRoot : class, IObserver<EventArgs>, new() {
            // Short-circuit (for performance) if there is nothing to actually project.
            if (events == null || !events.Any())
                return new TAggregateRoot();

            // Ensure all the events are transformed (if needed) before projecting them.
            // This can for instance ensure that any "old" events are upgraded to newer replacements.
            var transformedEvents =
                _eventSequenceTransformer
                    .Transform<TAggregateRoot>(events);

            // Project the events by injecting them into the aggregate root.
            //
            // We would normally use LINQ's Aggregate() operator here.
            // Unfortunately it is quite slow (relatively speaking) and with
            // 1mil iterations versus the code below, it's ~3 seconds slower.
            var aggregateRoot = new TAggregateRoot();
            foreach (var e in transformedEvents)
                aggregateRoot.OnNext(e.Arguments);

            // Send a signal to notify that event projection has finished.
            // Typically an aggregate root would then not allow itself to receive
            // any further IObserver<EventArgs> notifications.
            aggregateRoot.OnCompleted();

            return aggregateRoot;
        }
    }
    // ReSharper restore PossibleMultipleEnumeration
}