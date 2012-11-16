namespace EventStreams {
    using System;
    using Core.Domain;

    public interface IEventSource {
        TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new();
        TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new();
        TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, IEventSourced, new();
        TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new();
        TAggregateRoot OpenOrCreate<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new();
    }
}