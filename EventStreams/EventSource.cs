using System;
using System.Collections.Generic;

namespace EventStreams {
    using Projection;
    using Persistence;
    using Persistence.Serialization.Events;
    using Core;
    using Core.Domain;

    public class EventSource {
        private readonly IDictionary<Guid, AggregateRootObserver> _objects =
            new Dictionary<Guid, AggregateRootObserver>();

        private readonly IProjector _projector =
            new Projector();

        private readonly IPersistEvents _persistEvents =
            new FileSystemPersistEvents(new JsonEventWriter());

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            return Observe(new TAggregateRoot());
        }

        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            var events = _persistEvents.Load(identity);
            var ar = _projector.Project<TAggregateRoot>(events);
            return Observe(ar);
        }

        private void Close(IAggregateRoot aggregateRoot) {
            _objects.Remove(aggregateRoot.Identity);
        }

        private void Commit(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> uncommittedEvents) {
            AggregateRootObserver observer;
            if (!_objects.TryGetValue(aggregateRoot.Identity, out observer))
                throw new InvalidOperationException("Commit cannot be performed because the aggregate root did not originate from this event source.");

            _persistEvents.Persist(aggregateRoot, uncommittedEvents);
        }

        private TAggregateRoot Observe<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : class, IAggregateRoot, new() {
            var observer = new AggregateRootObserver(this, aggregateRoot);
            _objects.Add(aggregateRoot.Identity, observer);
            aggregateRoot.Subscribe(observer);
            return aggregateRoot;
        }

        private sealed class AggregateRootObserver : IObserver<EventArgs> {
            private readonly EventSource _parentSource;
            private readonly IAggregateRoot _aggregateRoot;
            private readonly LinkedList<IStreamedEvent> _uncommitted = new LinkedList<IStreamedEvent>();

            public AggregateRootObserver(EventSource parentSource, IAggregateRoot aggregateRoot) {
                if (parentSource == null) throw new ArgumentNullException("parentSource");
                if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
                _parentSource = parentSource;
                _aggregateRoot = aggregateRoot;
            }

            public void OnNext(EventArgs value) {
                _uncommitted.AddLast(new StreamedEvent(value));
            }

            public void OnError(Exception error) {
                throw new NotSupportedException();
            }

            public void OnCompleted() {
                if (_uncommitted.Count > 0) {
                    _parentSource.Commit(_aggregateRoot, _uncommitted);
                    _uncommitted.Clear();

                } else {
                    _parentSource.Close(_aggregateRoot);
                }
            }
        }
    }
}