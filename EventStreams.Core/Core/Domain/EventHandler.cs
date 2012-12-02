using System;

namespace EventStreams.Core.Domain {
    public abstract class EventHandler<TModel> : IObserver<EventArgs> where TModel : class, new() {
        public TModel Owner { get; private set; }
        public EventHandlerBehavior Behavior { get; private set; }
        public bool IsCompleted { get; private set; }

        protected EventHandler(TModel owner, EventHandlerBehavior behavior) {
            if (owner == null) throw new ArgumentNullException("owner");
            Owner = owner;
            Behavior = behavior;
        }

        public virtual void OnNext(EventArgs value) {
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
                throw new InvalidOperationException("The event sourced object has already completed handling events.");
        }

        protected void HandleEventHandlerNotFound(EventArgs args) {
            if (Behavior == EventHandlerBehavior.Lossless) {
                throw new InvalidOperationException(
                    string.Format(
                        "The event handler for '{0}' does not exist on the '{1}' type.",
                        args.GetType().Name, typeof(TModel)));
            }
        }
    }
}
