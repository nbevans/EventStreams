using System;
using System.Linq;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;
    using ReadModelling;

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
                foreach (var transformer in GetAllEventTransformers()) {
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

        private IEnumerable<IEventTransformer> GetAllEventTransformers() {
            var readModels = ReadModelContext.Current;
            if (readModels != null) {
                var readModelEventTransformers = readModels.Reverse().Select(rm => new ReadModelEventTransformer(rm));
                return _eventTransformers.Concat(readModelEventTransformers);
            }

            return Enumerable.Empty<ReadModelEventTransformer>();
        }

        private void EnsureChronology() {
            if (!_modified) {
                _eventTransformers.Sort((a, b) => a.Chronology.CompareTo(b.Chronology));
                _modified = true;
            }
        }
    }
}
