using System;
using System.Collections.Generic;

namespace EventStreams {
    using Core.Domain;

    /// <summary>
    /// A composition of multiple <see cref="EventSource"/> instances, each representing a logically separate source.
    /// This type acts as a facade interface by selecting the appropriate <see cref="EventSource"/> for a particular requested type of aggregate root or read model.
    /// </summary>
    public class CompositeEventSource : IEventSource {
        private readonly Dictionary<Type, object> _sources =
            new Dictionary<Type, object>();

        public CompositeEventSource Add<TEventSourced>(EventSource eventSource) where TEventSourced : class, IEventSourced, new() {
            _sources.Add(typeof(TEventSourced), eventSource);
            return this;
        }

        /// <summary>
        /// Creates a new aggregate root with a unique identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be created.</typeparam>
        /// <returns>The newly created aggregate root.</returns>
        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Create<TAggregateRoot>();
        }

        /// <summary>
        /// Creates a new aggregate root with a specific unique identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be created.</typeparam>
        /// <param name="identity">The identity to be used for the new aggregate root.</param>
        /// <returns>The newly created aggregate root.</returns>
        public TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Create<TAggregateRoot>(identity);
        }

        /// <summary>
        /// Reads a specific event stream and projects it onto a read model implementation.
        /// </summary>
        /// <typeparam name="TReadModel">The of read model to be used for the projection.</typeparam>
        /// <param name="identity">The identity of the event stream to be read.</param>
        /// <returns>The projected read model.</returns>
        public TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, IEventSourced, new() {
            return GetSource<TReadModel>().Read<TReadModel>(identity);
        }

        /// <summary>
        /// Opens an existing aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be opened.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened.</param>
        /// <returns>The opened aggregate root.</returns>
        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Open<TAggregateRoot>(identity);
        }

        /// <summary>
        /// Opens or creates an aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be opened or created.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened or created.</param>
        /// <returns>The aggregate root that was either opened or created.</returns>
        public TAggregateRoot OpenOrCreate<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().OpenOrCreate<TAggregateRoot>(identity);
        }

        private EventSource GetSource<TEventSourced>() where TEventSourced : class, IEventSourced, new() {
            object source;
            if (_sources.TryGetValue(typeof(TEventSourced), out source))
                return (EventSource)source;

            throw new InvalidOperationException(
                string.Format("An event source for the type '{0}' is not available.",
                              typeof (TEventSourced).FullName));
        }
    }
}
