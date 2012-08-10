using System;
using System.Collections.Generic;

namespace EventStreams {
    using Core.Domain;

    public class EventSource {
        private readonly IDictionary<Guid, AggregateRootObserver> _objects =
            new Dictionary<Guid, AggregateRootObserver>();

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            var ar = new TAggregateRoot();
            var observer = new AggregateRootObserver(this, ar);
            _objects.Add(ar.Identity, observer);
            ar.Subscribe(observer);
            return ar;
        }

        internal void Commit(IAggregateRoot aggregateRoot, IEnumerable<EventArgs> uncommitedEvents) {
            
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
        }
    }
}
