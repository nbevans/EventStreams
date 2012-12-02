using System;
using System.Collections.Generic;
using System.Threading;

namespace EventStreams.Core.Domain {

    /// <summary>
    /// Serves as a boilerplate helper component that can, optionally, be used by aggregate roots to simplify their event dispatching.
    /// This type manages the list of subscribed observers and allows events to be published to those observers.
    /// 
    /// Note that this implementation treats the aggregate root instance itself as an observer, so as to ensure that the event handler is called
    /// on the AR first.
    /// </summary>
    /// <remarks>
    /// An aggregate root can implement this type by forwarding its <code>IObservable.Subscribe()</code> invocations to a privately held instance of this type.
    /// </remarks>
    /// <typeparam name="TAggregateRoot">The type of aggregate root for which observation services will be performed.</typeparam>
    public class EventDispatcher<TAggregateRoot> : IObservable<EventArgs>, IObserver<EventArgs>, IDisposable
        where TAggregateRoot : class, IObservable<EventArgs>, new() {

        private readonly ReaderWriterLockSlim _rwLock =
            new ReaderWriterLockSlim();

        private readonly IList<IObserver<EventArgs>> _observers =
            new List<IObserver<EventArgs>>(1);

        public TAggregateRoot Owner { get; set; }
        public bool IsCompleted { get; set; }

        public EventDispatcher(TAggregateRoot owner) {
            if (owner == null) throw new ArgumentNullException("owner");
            Owner = owner;
        }

        public void Dispose() {
            ((IObserver<EventArgs>)this).OnCompleted();
        }

        public virtual void Dispatch(EventArgs args) {
            ((IObserver<EventArgs>)this).OnNext(args);

            DispatchToSelf(args);
        }

        protected virtual void DispatchToSelf(EventArgs args) {
            new ConventionEventHandler<TAggregateRoot>(Owner).OnNext(args);
        }

        private void RemoveObserver(IObserver<EventArgs> observer) {
            try {
                _rwLock.EnterWriteLock();

                _observers.Remove(observer);

            } finally {
                _rwLock.ExitWriteLock();
            }
        }

        private void NotifyObservers(Action<IObserver<EventArgs>> action) {
            try {
                _rwLock.EnterReadLock();

                foreach (var observer in _observers)
                    action(observer);

            } finally {
                _rwLock.ExitReadLock();
            }
        }

        public virtual IDisposable Subscribe(IObserver<EventArgs> observer) {
            if (observer == null) throw new ArgumentNullException("observer");
            
            try {
                _rwLock.EnterWriteLock();

                _observers.Add(observer);

            } finally {
                _rwLock.ExitWriteLock();
            }

            return Disposable.Create(() => RemoveObserver(observer));
        }

        void IObserver<EventArgs>.OnNext(EventArgs value) {
            ThrowIfCompleted();
            NotifyObservers(observer => observer.OnNext(value));
        }

        void IObserver<EventArgs>.OnError(Exception error) {
            ThrowIfCompleted();
            NotifyObservers(observer => observer.OnError(error));
        }

        void IObserver<EventArgs>.OnCompleted() {
            ThrowIfCompleted();
            NotifyObservers(observer => observer.OnCompleted());
            IsCompleted = true;
        }

        protected void ThrowIfCompleted() {
            if (IsCompleted)
                throw new InvalidOperationException(
                    "The aggregate root previously indicated that it had completed producing commands.");
        }
    }
}
