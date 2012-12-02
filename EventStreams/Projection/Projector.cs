using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Projection {
    using Core;
    using Core.Domain;
    using Transformation;

    internal sealed class Projector : IProjector {
        private readonly IEventSequenceTransformer _eventSequenceTransformer =
            new EventSequenceTransformer();

        public IEventSequenceTransformer Transformations {
            get { return _eventSequenceTransformer; }
        }

        public TModel Project<TModel>(IEnumerable<IStreamedEvent> events, Func<TModel, EventHandler<TModel>> eventHandlerFactory)
            where TModel : class, new() {

            return Project(Guid.NewGuid(), events, eventHandlerFactory);
        }

        public TModel Project<TModel>(Guid identity, IEnumerable<IStreamedEvent> events, Func<TModel, EventHandler<TModel>> eventHandlerFactory)
            where TModel : class, new() {

            // Initialize a suitable activator for the model type.
            var activator =
                new ObjectActivatorCache<TModel>()
                    .Activator();

            // New up an instance of TModel.
            var model = activator(identity);

            // ReSharper disable PossibleMultipleEnumeration
            // Short-circuit (for performance) if there is nothing to actually project.
            if (events == null || !events.Any())
                return model;

            // Ensure all the events are transformed (if needed) before projecting them.
            // This can for instance ensure that any "old" events are upgraded to newer replacements.
            var transformedEvents = Transformations.Transform<TModel>(events);
            // ReSharper restore PossibleMultipleEnumeration

            // Project the events by injecting them into the model object.
            //
            // We would normally use LINQ's Aggregate() operator here.
            // Unfortunately it is quite slow (relatively speaking) and with
            // 1mil iterations versus the code below, it's ~3 seconds slower.
            var applier = eventHandlerFactory(model);
            foreach (var e in transformedEvents)
                applier.OnNext(e.Arguments);

            // TODO: This OnCompletion notification isn't wanted or needed for read-models.
            //
            // Send a signal to notify that event projection has finished.
            // Typically a model would then not allow itself to receive
            // any further IObserver<EventArgs> notifications.
            applier.OnCompleted();

            return model;
        }
    }
}