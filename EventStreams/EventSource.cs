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
    /// The ultimate entry point into the event store. All aggregate roots and read models are "sourced" via this provider.
    /// The event source will perform event observation and lifetime scope management of all objects that are sourced from it.
    /// </summary>
    public class EventSource : IEventSource {

        // We use the "little known" ConditionalWeakTable hidden in the depths of .NET 4 because it means we don't need to hold
        // strong references to our aggregate roots which would prevent them being garbage collected. This is important to prevent
        // serious memory leaks.
        private readonly ConditionalWeakTable<IObservable<EventArgs>, AggregateRootObserver> _objects =
            new ConditionalWeakTable<IObservable<EventArgs>, AggregateRootObserver>();

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
        /// Creates a new aggregate root with a unique identity.
        /// </summary>
        /// <typeparam name="TModel">The type of aggregate root to be created.</typeparam>
        /// <returns>The newly created aggregate root.</returns>
        public TModel Create<TModel>() where TModel : class, IObservable<EventArgs>, new() {
            return Create<TModel>(Guid.NewGuid());
        }

        /// <summary>
        /// Creates a new aggregate root with a specific unique identity.
        /// </summary>
        /// <typeparam name="TModel">The type of aggregate root to be created.</typeparam>
        /// <param name="identity">The identity to be used for the new aggregate root.</param>
        /// <returns>The newly created aggregate root.</returns>
        public TModel Create<TModel>(Guid identity) where TModel : class, IObservable<EventArgs>, new() {
            return OpenCore<TModel>(identity, false);
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
        /// Opens an existing aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TModel">The type of aggregate root to be opened.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened.</param>
        /// <returns>The opened aggregate root.</returns>
        public TModel Open<TModel>(Guid identity) where TModel : class, IObservable<EventArgs>, new() {
            return OpenCore<TModel>(identity);
        }

        /// <summary>
        /// Opens or creates an aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TModel">The type of aggregate root to be opened or created.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened or created.</param>
        /// <returns>The aggregate root that was either opened or created.</returns>
        public TModel OpenOrCreate<TModel>(Guid identity) where TModel : class, IObservable<EventArgs>, new() {
            try {
                return Open<TModel>(identity);

            } catch (StreamNotFoundPersistenceException) {
                return Create<TModel>(identity);
            }
        }

        private TModel OpenCore<TModel>(Guid identity, bool loadEvents = true, EventHandlerBehavior eventHandlerBehavior = EventHandlerBehavior.Lossless) where TModel : class, new() {
            var events = loadEvents ? _persistenceStrategy.Load(identity) : Enumerable.Empty<IStreamedEvent>();
            var ar = _projector.Project<TModel>(identity, events, x => new ConventionEventHandler<TModel>(x, eventHandlerBehavior));

            if (typeof(IObservable<EventArgs>).IsAssignableFrom(typeof(TModel)))
                Observe((IObservable<EventArgs>)ar, identity);

            return ar;
        }

        private void Close(IObservable<EventArgs> aggregateRoot) {
            _objects.Remove(aggregateRoot);
        }

        private void Commit(Guid identity, IEnumerable<IStreamedEvent> uncommittedEvents) {
            _persistenceStrategy.Store(identity, uncommittedEvents);
        }

        private void Observe(IObservable<EventArgs> aggregateRoot, Guid identity) {
            var observer = new AggregateRootObserver(this, aggregateRoot, identity);
            _objects.Add(aggregateRoot, observer);
            observer.Subscription = aggregateRoot.Subscribe(observer);
        }

        /// <summary>
        /// Provides observation services upon an aggregate root.
        /// </summary>
        /// <remarks>
        /// When the <see cref="EventSource"/> creates or opens an aggregate root, it will subscribe for notification of events on that AR.
        /// This type is the observer implementation that will act as the callback function when the AR is publishing an event.
        /// </remarks>
        private sealed class AggregateRootObserver : IObserver<EventArgs> {
            private readonly EventSource _parentSource;
            private readonly IObservable<EventArgs> _aggregateRoot;
            private readonly Guid _identity;

            public IDisposable Subscription { get; set; }

            public AggregateRootObserver(EventSource parentSource, IObservable<EventArgs> aggregateRoot, Guid identity) {
                if (parentSource == null) throw new ArgumentNullException("parentSource");
                if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
                _parentSource = parentSource;
                _aggregateRoot = aggregateRoot;
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
                _parentSource.Close(_aggregateRoot);

                // Unsubscribe from observation.
                Subscription.Dispose();
            }
        }
    }
}