using System;
using System.Collections.Generic;

namespace EventStreams {
    using Core.Domain;

    public class CompositeEventSource : IEventSource {
        private readonly Dictionary<Type, object> _sources =
            new Dictionary<Type, object>();

        public CompositeEventSource Add<TEventSourced>(EventSource eventSource) where TEventSourced : class, IEventSourced, new() {
            _sources.Add(typeof(TEventSourced), eventSource);
            return this;
        }

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Create<TAggregateRoot>();
        }

        public TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Create<TAggregateRoot>(identity);
        }

        public TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, IEventSourced, new() {
            return GetSource<TReadModel>().Read<TReadModel>(identity);
        }

        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Open<TAggregateRoot>(identity);
        }

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
