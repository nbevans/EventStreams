using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EventStreams {
    using Core;
    using Core.Domain;
    using Persistence;
    using Projection;

    public class EventSource {
        private readonly ConditionalWeakTable<IAggregateRoot, AggregateRootObserver> _objects =
            new ConditionalWeakTable<IAggregateRoot, AggregateRootObserver>();

        private readonly IProjector _projector;
        private readonly IPersistenceStrategy _persistenceStrategy;

        public EventSource(IPersistenceStrategy persistenceStrategy)
            : this(persistenceStrategy, null) { }

        public EventSource(IPersistenceStrategy persistenceStrategy, IProjector projector) {
            if (persistenceStrategy == null) throw new ArgumentNullException("persistenceStrategy");
            _persistenceStrategy = persistenceStrategy;
            _projector = projector ?? new Projector();
        }

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            return Observe(new TAggregateRoot());
        }

        public TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            var ar = _projector.Project<TAggregateRoot>(identity, null);
            return Observe(ar);
        }

        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            var events = _persistenceStrategy.Load(identity);
            var ar = _projector.Project<TAggregateRoot>(identity, events);
            return Observe(ar);
        }

        public TAggregateRoot OpenOrCreate<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            try {
                return Open<TAggregateRoot>(identity);

            } catch (StreamNotFoundPersistenceException) {
                return Create<TAggregateRoot>(identity);
            }
        }

        private void Close(IAggregateRoot aggregateRoot) {
            _objects.Remove(aggregateRoot);
        }

        private void Commit(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> uncommittedEvents) {
            AggregateRootObserver observer;
            if (!_objects.TryGetValue(aggregateRoot, out observer))
                throw new InvalidOperationException("Commit cannot be performed because the aggregate root did not originate from this event source.");

            _persistenceStrategy.Store(aggregateRoot, uncommittedEvents);
        }

        private TAggregateRoot Observe<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : class, IAggregateRoot, new() {
            var observer = new AggregateRootObserver(this, aggregateRoot);
            _objects.Add(aggregateRoot, observer);
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