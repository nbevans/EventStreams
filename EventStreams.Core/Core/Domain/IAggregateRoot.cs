using System;

namespace EventStreams.Core.Domain {
    public interface IAggregateRoot : IObserver<EventArgs>, IObservable<EventArgs>, IDisposable {
        Guid Identity { get; }
        object Memento { get; }
    }
}