using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EventStreams {
    using Core;
    using Core.Domain;
    using Persistence;
    using Projection;

    public class EventSource<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot, new() {
        private readonly ConditionalWeakTable<IAggregateRoot, AggregateRootObserver<TAggregateRoot>> _objects =
            new ConditionalWeakTable<IAggregateRoot, AggregateRootObserver<TAggregateRoot>>();

        private readonly IProjector _projector;
        private readonly IPersistenceStrategy _persistenceStrategy;

        public EventSource(IPersistenceStrategy persistenceStrategy)
            : this(persistenceStrategy, null) { }

        public EventSource(IPersistenceStrategy persistenceStrategy, IProjector projector) {
            if (persistenceStrategy == null) throw new ArgumentNullException("persistenceStrategy");
            _persistenceStrategy = persistenceStrategy;
            _projector = projector ?? new Projector();
        }

        public TAggregateRoot Create() {
            return Observe(new TAggregateRoot());
        }

        public TAggregateRoot Create(Guid identity) {
            var ar = _projector.Project<TAggregateRoot>(identity, null);
            return Observe(ar);
        }

        public TAggregateRoot Open(Guid identity) {
            var events = _persistenceStrategy.Load(identity);
            var ar = _projector.Project<TAggregateRoot>(identity, events);
            return Observe(ar);
        }

        public TAggregateRoot OpenOrCreate(Guid identity) {
            try {
                return Open(identity);

            } catch (StreamNotFoundPersistenceException) {
                return Create(identity);
            }
        }

        private void Close(IAggregateRoot aggregateRoot) {
            _objects.Remove(aggregateRoot);
        }

        private void Commit(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> uncommittedEvents) {
            AggregateRootObserver<TAggregateRoot> observer;
            if (!_objects.TryGetValue(aggregateRoot, out observer))
                throw new InvalidOperationException("Commit cannot be performed because the aggregate root did not originate from this event source.");

            _persistenceStrategy.Store(aggregateRoot, uncommittedEvents);
        }

        private TAggregateRoot Observe(TAggregateRoot aggregateRoot) {
            var observer = new AggregateRootObserver<TAggregateRoot>(this, aggregateRoot);
            _objects.Add(aggregateRoot, observer);
            aggregateRoot.Subscribe(observer);
            return aggregateRoot;
        }

        private sealed class AggregateRootObserver<TObservedAggregateRoot> : IObserver<EventArgs> where TObservedAggregateRoot : class, IAggregateRoot, new() {
            private readonly EventSource<TObservedAggregateRoot> _parentSource;
            private readonly TObservedAggregateRoot _aggregateRoot;
            private readonly LinkedList<IStreamedEvent> _uncommitted = new LinkedList<IStreamedEvent>();

            public AggregateRootObserver(EventSource<TObservedAggregateRoot> parentSource, TObservedAggregateRoot aggregateRoot) {
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