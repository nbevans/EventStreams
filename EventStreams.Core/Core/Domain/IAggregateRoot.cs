using System;

namespace EventStreams.Core.Domain {
    /// <summary>
    /// The core interface that all event sourced aggregate roots must implement.
    /// This interface aims to ensure that every AR has both an unique identity and a memento container for its current state. As well as ensuring that every AR is observable for events.
    /// </summary>
    public interface IAggregateRoot : IEventSourced, IObservable<EventArgs>, IDisposable { }
}