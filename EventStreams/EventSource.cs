using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EventStreams {
    using Core;
    using Core.Domain;
    using Persistence;
    using Projection;

    /// <summary>
    /// The ultimate entry point into the event store. All write models (WM) and read models (RM) are "sourced" via this provider.
    /// The event source will perform event observation and lifetime scope management of all objects that are sourced from it.
    /// </summary>
    public class EventSource : IEventSource {

        // We use the "little known" ConditionalWeakTable hidden in the depths of .NET 4 because it means we don't need to hold
        // strong references to our write or read models which would prevent them being garbage collected.
        // This is important to prevent serious memory leaks.
        private readonly ConditionalWeakTable<IObservable<EventArgs>, WriteModelObserver> _objects =
            new ConditionalWeakTable<IObservable<EventArgs>, WriteModelObserver>();

        private readonly IProjector _projector;
        private readonly IPersistenceStrategy _persistenceStrategy;

        public EventSource(IPersistenceStrategy persistenceStrategy)
            : this(persistenceStrategy, null) { }

        public EventSource(IPersistenceStrategy persistenceStrategy, IProjector projector) {
            if (persistenceStrategy == null) throw new ArgumentNullException("persistenceStrategy");
            _persistenceStrategy = persistenceStrategy;
            _projector = projector ?? new Projector();
        }

        /// <summary>
        /// Creates a new write model object with a unique identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be created.</typeparam>
        /// <returns>The newly created write model.</returns>
        public TWriteModel Create<TWriteModel>() where TWriteModel : class, IObservable<EventArgs>, new() {
            return Create<TWriteModel>(Guid.NewGuid());
        }

        /// <summary>
        /// Creates a new write model object with a specific unique identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be created.</typeparam>
        /// <param name="identity">The identity of the event stream to be created.</param>
        /// <returns>The newly created write model.</returns>
        public TWriteModel Create<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new() {
            return OpenCore<TWriteModel>(identity, false);
        }

        /// <summary>
        /// Reads a specific event stream and projects it onto a read model implementation.
        /// </summary>
        /// <typeparam name="TReadModel">The of read model to be used for the projection.</typeparam>
        /// <param name="identity">The identity of the event stream to be read.</param>
        /// <returns>The projected read model.</returns>
        public TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, new() {
            return OpenCore<TReadModel>(identity, true, EventHandlerBehavior.Lossy);
        }

        /// <summary>
        /// Opens an existing write model with a specific identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be opened.</typeparam>
        /// <param name="identity">The identity of the event stream to be opened or created.</param>
        /// <returns>The opened write model.</returns>
        public TWriteModel Open<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new() {
            return OpenCore<TWriteModel>(identity);
        }

        /// <summary>
        /// Opens or creates a write model with a specific identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be opened or created.</typeparam>
        /// <param name="identity">The identity of the event stream to be opened or created.</param>
        /// <returns>The write model that was either opened or created.</returns>
        public TWriteModel OpenOrCreate<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new() {
            try {
                return Open<TWriteModel>(identity);

            } catch (StreamNotFoundPersistenceException) {
                return Create<TWriteModel>(identity);
            }
        }

        private TModel OpenCore<TModel>(Guid identity, bool loadEvents = true, EventHandlerBehavior eventHandlerBehavior = EventHandlerBehavior.Lossless) where TModel : class, new() {
            var events = loadEvents ? _persistenceStrategy.Load(identity) : Enumerable.Empty<IStreamedEvent>();
            var model = _projector.Project<TModel>(identity, events, x => new ConventionEventHandler<TModel>(x, eventHandlerBehavior));

            if (typeof(IObservable<EventArgs>).IsAssignableFrom(typeof(TModel)))
                Observe((IObservable<EventArgs>)model, identity);

            return model;
        }

        private void Close(IObservable<EventArgs> writeModel) {
            _objects.Remove(writeModel);
        }

        private void Commit(Guid identity, IEnumerable<IStreamedEvent> events) {
            _persistenceStrategy.Store(identity, events);
        }

        private void Observe(IObservable<EventArgs> writeModel, Guid identity) {
            var observer = new WriteModelObserver(this, writeModel, identity);
            _objects.Add(writeModel, observer);
            observer.Subscription = writeModel.Subscribe(observer);
        }

        /// <summary>
        /// Provides observation services upon a write model (WM).
        /// </summary>
        /// <remarks>
        /// When the <see cref="EventSource"/> creates or opens a write model (WM), it will subscribe for notification of events on that WM.
        /// This type is the observer implementation that will act as the callback function when the WM is publishing an event.
        /// </remarks>
        private sealed class WriteModelObserver : IObserver<EventArgs> {
            private readonly EventSource _parentSource;
            private readonly IObservable<EventArgs> _writeModel;
            private readonly Guid _identity;

            public IDisposable Subscription { get; set; }

            public WriteModelObserver(EventSource parentSource, IObservable<EventArgs> writeModel, Guid identity) {
                if (parentSource == null) throw new ArgumentNullException("parentSource");
                if (writeModel == null) throw new ArgumentNullException("writeModel");
                _parentSource = parentSource;
                _writeModel = writeModel;
                _identity = identity;
            }

            public void OnNext(EventArgs value) {
                _parentSource.Commit(_identity, new[] { new StreamedEvent(value) });
            }

            public void OnError(Exception error) {
                throw new NotSupportedException();
            }

            public void OnCompleted() {
                // Notify the EventSource to purge its observer.
                _parentSource.Close(_writeModel);

                // Unsubscribe from observation.
                Subscription.Dispose();
            }
        }
    }
}