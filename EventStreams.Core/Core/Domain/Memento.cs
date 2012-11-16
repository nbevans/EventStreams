using System;

namespace EventStreams.Core.Domain {
    /// <summary>
    /// Serves as a container for the current state of an event sourced domain object, including aggregate roots and read models.
    /// </summary>
    /// <typeparam name="TAggregateRootState">The type of state object to be held by the container.</typeparam>
    public sealed class Memento<TAggregateRootState> where TAggregateRootState : new() {

        private readonly Guid _identity;
        private readonly TAggregateRootState _state;

        /// <summary>
        /// Gets the unique identity of the event sourced domain object.
        /// </summary>
        public Guid Identity { get { return _identity; } }

        /// <summary>
        /// Gets the current state of the event sourced domain object.
        /// </summary>
        public TAggregateRootState State { get { return _state; } }

        public Memento() {
            _identity = Guid.NewGuid();
            _state = new TAggregateRootState();
        }

        public Memento(Guid identity) {
            _identity = identity;
            _state = new TAggregateRootState();
        } 
    }
}
