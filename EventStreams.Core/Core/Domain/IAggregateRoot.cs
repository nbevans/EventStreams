using System;

namespace EventStreams.Core.Domain {
    public interface IAggregateRoot : IEventSourced, IObservable<EventArgs>, IDisposable { }
}