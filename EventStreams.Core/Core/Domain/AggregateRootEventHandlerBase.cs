using System;

namespace EventStreams.Core.Domain {
    public abstract class AggregateRootEventHandlerBase<TAggregateRoot> : IObserver<IStreamedEvent> where TAggregateRoot : class, new() {

        protected TAggregateRoot Owner { get; private set; }
        protected bool IsCompleted { get; private set; }

        public AggregateRootEventHandlerBase(TAggregateRoot owner) {
            if (owner == null) throw new ArgumentNullException("owner");
            Owner = owner;
        }

        public virtual void OnNext(IStreamedEvent value) {
            ThrowIfCompleted();
        }

        public virtual void OnError(Exception error) {
            ThrowIfCompleted();            
        }

        public virtual void OnCompleted() {
            ThrowIfCompleted();
            IsCompleted = true;
        }

        protected void ThrowIfCompleted() {
            if (IsCompleted)
                throw new InvalidOperationException("The aggregate root has already completed handling events.");
        }

        protected void ThrowEventHandlerNotFound(IStreamedEvent streamedEvent) {
            throw new InvalidOperationException(
                string.Format(
                    "The event handler for '{0}' does not exist on the '{1}' entity type.",
                    streamedEvent.Arguments.GetType().Name, typeof(TAggregateRoot)));
        }
    }
}
