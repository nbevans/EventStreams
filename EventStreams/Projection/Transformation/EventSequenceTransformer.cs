using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Projection.Transformation {
    using Core;

    public class EventSequenceTransformer : IEventSequenceTransformer {
        private readonly List<IEventTransformer> _eventTransformers = new List<IEventTransformer>();
        private bool _isChronologic;

        public IEventSequenceTransformer Bind<TEventTransformer>() where TEventTransformer : IEventTransformer, new() {
            _eventTransformers.Add(new TEventTransformer());
            _isChronologic = false;
            return this;
        }

        public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IEnumerable<IStreamedEvent> events) where TAggregateRoot : class, new() {
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

        private IEnumerable<IStreamedEvent> TransformCore<TAggregateRoot>(IEnumerable<IStreamedEvent> events, IEventTransformer transformer) where TAggregateRoot : class, new() {
            foreach (var e in events) {
                var transformedEvents = transformer.Transform<TAggregateRoot>(e);
                if (events != null)
                    foreach (var transformedEvent in transformedEvents)
                        yield return transformedEvent;
            }
        }

        private void EnsureChronology() {
            if (!_isChronologic) {
                _eventTransformers.Sort((a, b) => a.Chronology.CompareTo(b.Chronology));
                _isChronologic = true;
            }
        }
    }
}
