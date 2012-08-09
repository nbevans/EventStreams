using System;

namespace EventStreams.Core.Domain {
    public interface IAggregateRootRepository<TAggregateRoot> where TAggregateRoot : class, IObserver<EventArgs>, new() {
        TAggregateRoot Create();
        TAggregateRoot Get(Guid identifier);
        bool TryGet(Guid identifier, out TAggregateRoot aggregateRoot);
    }
}
