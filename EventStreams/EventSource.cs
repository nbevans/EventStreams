using System;
using System.Collections.Generic;

namespace EventStreams {
    using Core.Domain;

    public class EventSource {
        private readonly IDictionary<Guid, AggregateRootObserver> _objects =
            new Dictionary<Guid, AggregateRootObserver>();

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            var ar = new TAggregateRoot();
            var observer = new AggregateRootObserver(ar);
            _objects.Add(ar.Identity, observer);
            ar.Subscribe(observer);
            return ar;
        }
    }

    internal sealed class AggregateRootObserver : IObserver<EventArgs> {
        public IAggregateRoot AggregateRoot { get; private set; }

        public AggregateRootObserver(IAggregateRoot aggregateRoot) {
            AggregateRoot = aggregateRoot;
        }

        public void OnNext(EventArgs value) {
            throw new NotImplementedException();
        }

        public void OnError(Exception error) {
            throw new NotImplementedException();
        }

        public void OnCompleted() {
            throw new NotImplementedException();
        }
    }
}
