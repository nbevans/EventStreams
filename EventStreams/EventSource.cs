using System;
using System.Collections.Generic;

namespace EventStreams {
    using Projection;
    using Persistence;
    using Core.Domain;

    public class EventSource {
        private readonly IDictionary<Guid, AggregateRootObserver> _objects =
            new Dictionary<Guid, AggregateRootObserver>();

        private readonly IProjector _projector =
            new Projector();

        private readonly IPersistEvents _persistEvents =
            new NullPersistEvents();

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            return Observe(new TAggregateRoot());
        }

        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            var events = _persistEvents.Load(identity);
            var ar = _projector.Project<TAggregateRoot>(events);
            return Observe(ar);
        }

        internal void Commit(IAggregateRoot aggregateRoot, IEnumerable<EventArgs> uncommittedEvents) {
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
    }

    internal sealed class AggregateRootObserver : IObserver<EventArgs> {
        private readonly EventSource _parentSource;
        private readonly IAggregateRoot _aggregateRoot;
        private readonly List<EventArgs> _uncommitted = new List<EventArgs>(4);

        public AggregateRootObserver(EventSource parentSource, IAggregateRoot aggregateRoot) {
            if (parentSource == null) throw new ArgumentNullException("parentSource");
            if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
            _parentSource = parentSource;
            _aggregateRoot = aggregateRoot;
        }

        public void OnNext(EventArgs value) {
            _uncommitted.Add(value);
        }

        public void OnError(Exception error) {
            
        }

        public void OnCompleted() {
            _parentSource.Commit(_aggregateRoot, _uncommitted);
            _uncommitted.Clear();
            _uncommitted.TrimExcess();
        }
    }
}
