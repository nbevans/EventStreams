using System;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;

    public class EventSequenceTransformer : IEventSequenceTransformer {
        private readonly List<IEventTransformer> _eventTransformers = new List<IEventTransformer>();
        private bool _modified;

        public IEventSequenceTransformer Bind<TEventTransformer>() where TEventTransformer : class, IEventTransformer, new() {
            _eventTransformers.Add(new TEventTransformer());
            _modified = false;
            return this;
        }

        public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IEnumerable<IStreamedEvent> events) {
            EnsureChronology();

            foreach (var e in events) {
                IEnumerable<IStreamedEvent> transformedEvents = new[] { e };
                foreach (var transformer in _eventTransformers) {
                    transformedEvents =
                        TransformCore<TAggregateRoot>(
                            transformedEvents,
                            transformer);
                }

                foreach (var te in transformedEvents)
                    yield return te;
            }
        }

        private IEnumerable<IStreamedEvent> TransformCore<TAggregateRoot>(IEnumerable<IStreamedEvent> events, IEventTransformer transformer) {
            foreach (var e in events) {
                var transformedEvents = transformer.Transform<TAggregateRoot>(e);
                if (transformedEvents != null)
                    foreach (var transformedEvent in transformedEvents)
                        yield return transformedEvent;
            }
        }

        private void EnsureChronology() {
            if (!_modified) {
                _eventTransformers.Sort((a, b) => a.Chronology.CompareTo(b.Chronology));
                _modified = true;
            }
        }
    }
}
