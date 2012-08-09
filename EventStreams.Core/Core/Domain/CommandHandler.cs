using System;
using System.Collections.Generic;

namespace EventStreams.Core.Domain {
    public class CommandHandler<TAggregateRoot> : IObservable<EventArgs>, IObserver<EventArgs> where TAggregateRoot : class, IObservable<EventArgs>, new() {

        private readonly IList<IObserver<EventArgs>> _observers =
            new List<IObserver<EventArgs>>(1);

        public TAggregateRoot Owner { get; set; }
        public bool IsCompleted { get; set; }

        public CommandHandler(TAggregateRoot owner) {
            if (owner == null) throw new ArgumentNullException("owner");
            Owner = owner;
        }

        public virtual IDisposable Subscribe(IObserver<EventArgs> observer) {
            if (observer == null) throw new ArgumentNullException("observer");
            _observers.Add(observer);
            return Disposable.Create(() => _observers.Remove(observer));
        }

        public virtual void OnNext(EventArgs value) {
            ThrowIfCompleted();
            foreach (var observer in _observers)
                observer.OnNext(value);
        }

        public virtual void OnError(Exception error) {
            ThrowIfCompleted();
            foreach (var observer in _observers)
                observer.OnError(error);
        }

        public virtual void OnCompleted() {
            ThrowIfCompleted();

            foreach (var observer in _observers)
                observer.OnCompleted();

            IsCompleted = true;
        }

        protected void ThrowIfCompleted() {
            if (IsCompleted)
                throw new InvalidOperationException("The aggregate root previously indicated that it had completed producing commands.");
        }
    }
}
