using System;

namespace EventStreams.Core.Domain {
    public interface IAggregateRoot : IObservable<EventArgs>, IDisposable {
        Guid Identity { get; }
        object Memento { get; }
    }
}