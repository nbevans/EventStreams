using System;

namespace EventStreams {
    using Core.Domain;

    public interface IEventSource {
        /// <summary>
        /// Creates a new aggregate root with a unique identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be created.</typeparam>
        /// <returns>The newly created aggregate root.</returns>
        TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new();

        /// <summary>
        /// Creates a new aggregate root with a specific unique identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be created.</typeparam>
        /// <param name="identity">The identity to be used for the new aggregate root.</param>
        /// <returns>The newly created aggregate root.</returns>
        TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new();

        /// <summary>
        /// Reads a specific event stream and projects it onto a read model implementation.
        /// </summary>
        /// <typeparam name="TReadModel">The of read model to be used for the projection.</typeparam>
        /// <param name="identity">The identity of the event stream to be read.</param>
        /// <returns>The projected read model.</returns>
        TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, IEventSourced, new();

        /// <summary>
        /// Opens an existing aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be opened.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened.</param>
        /// <returns>The opened aggregate root.</returns>
        TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new();

        /// <summary>
        /// Opens or creates an aggregate root with a specific identity.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of aggregate root to be opened or created.</typeparam>
        /// <param name="identity">The identity of the aggregate root to be opened or created.</param>
        /// <returns>The aggregate root that was either opened or created.</returns>
        TAggregateRoot OpenOrCreate<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new();
    }
}