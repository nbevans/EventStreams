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
        private readonly ConditionalWeakTable<IAggregateRoot, AggregateRootObserver> _objects =
            new ConditionalWeakTable<IAggregateRoot, AggregateRootObserver>();

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
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be created.</typeparam>
        /// <returns>The newly created aggregate root.</returns>
        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            var ar = new TAggregateRoot();
            Observe(ar);
            return ar;
        }

        /// <summary>
        /// Creates a new aggregate root with a specific unique identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be created.</typeparam>
        /// <param name="identity">The identity to be used for the new aggregate root.</param>
        /// <returns>The newly created aggregate root.</returns>
        public TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return OpenCore<TAggregateRoot>(identity, false);
        }

        /// <summary>
        /// Reads a specific event stream and projects it onto a read model implementation.
        /// </summary>
        /// <typeparam name="TReadModel">The of read model to be used for the projection.</typeparam>
        /// <param name="identity">The identity of the event stream to be read.</param>
        /// <returns>The projected read model.</returns>
        public TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, IEventSourced, new() {
            return OpenCore<TReadModel>(identity, true, EventHandlerBehavior.Lossy);
        }

        /// <summary>
        /// Opens an existing aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be opened.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened.</param>
        /// <returns>The opened aggregate root.</returns>
        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return OpenCore<TAggregateRoot>(identity);
        }

        /// <summary>
        /// Opens or creates an aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be opened or created.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened or created.</param>
        /// <returns>The aggregate root that was either opened or created.</returns>
        public TAggregateRoot OpenOrCreate<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            try {
                return Open<TAggregateRoot>(identity);

            } catch (StreamNotFoundPersistenceException) {
                return Create<TAggregateRoot>(identity);
            }
        }

        private TEventSourced OpenCore<TEventSourced>(Guid identity, bool loadEvents = true, EventHandlerBehavior eventHandlerBehavior = EventHandlerBehavior.Lossless) where TEventSourced : class, IEventSourced, new() {
            var events = loadEvents ? _persistenceStrategy.Load(identity) : Enumerable.Empty<IStreamedEvent>();
            var ar = _projector.Project<TEventSourced>(identity, events, x => new ConventionEventHandler<TEventSourced>(x, eventHandlerBehavior));
            
            if (typeof(IAggregateRoot).IsAssignableFrom(typeof(TEventSourced)))
                Observe((IAggregateRoot)ar);

            return ar;
        }

        private void Close(IAggregateRoot aggregateRoot) {
            _objects.Remove(aggregateRoot);
        }

        private void Commit(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> uncommittedEvents) {
            AggregateRootObserver observer;
            if (!_objects.TryGetValue(aggregateRoot, out observer))
                throw new InvalidOperationException("Commit cannot be performed because the aggregate root did not originate from this event source.");

            _persistenceStrategy.Store(aggregateRoot, uncommittedEvents);
        }

        private void Observe(IAggregateRoot aggregateRoot) {
            var observer = new AggregateRootObserver(this, aggregateRoot);
            _objects.Add(aggregateRoot, observer);
            aggregateRoot.Subscribe(observer);
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
            private readonly IAggregateRoot _aggregateRoot;
            private readonly LinkedList<IStreamedEvent> _uncommitted = new LinkedList<IStreamedEvent>();

            public AggregateRootObserver(EventSource parentSource, IAggregateRoot aggregateRoot) {
                if (parentSource == null) throw new ArgumentNullException("parentSource");
                if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
                _parentSource = parentSource;
                _aggregateRoot = aggregateRoot;
            }

            public void OnNext(EventArgs value) {
                _uncommitted.AddLast(new StreamedEvent(value));
            }

            public void OnError(Exception error) {
                throw new NotSupportedException();
            }

            public void OnCompleted() {
                if (_uncommitted.Count > 0) {
                    _parentSource.Commit(_aggregateRoot, _uncommitted);
                    _uncommitted.Clear();

                } else {
                    _parentSource.Close(_aggregateRoot);
                }
            }
        }
    }
}