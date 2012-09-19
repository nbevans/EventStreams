using System;

namespace EventStreams.Core.Domain {
    public sealed class Memento<TAggregateRootState> where TAggregateRootState : new() {

        private readonly Guid _identity;
        private readonly TAggregateRootState _state;

        public Guid Identity { get { return _identity; } }

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
