using System;

namespace EventStreams.Core.Domain {
    /// <summary>
    /// The core interface that all event sourced types must implement, including aggregate roots and read models.
    /// This interface aims to ensure that every event sourced type has both an unique identity and a memento container.
    /// </summary>
    public interface IEventSourced {
        /// <summary>
        /// Gets the unique identity of the event sourced domain object.
        /// </summary>
        Guid Identity { get; }

        /// <summary>
        /// Gets the memento representing the current state of the event sourced domain object.
        /// </summary>
        object Memento { get; } 
    }
}