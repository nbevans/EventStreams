using System;
using System.Collections.Generic;

namespace EventStreams {
    using Core.Domain;

    public class CompositeEventSource {
        private readonly Dictionary<Type, object> _sources =
            new Dictionary<Type, object>();

        public CompositeEventSource Add<TAggregateRoot>(EventSource<TAggregateRoot> eventSource) where TAggregateRoot : class, IAggregateRoot, new() {
            _sources.Add(typeof(TAggregateRoot), eventSource);
            return this;
        }

        public TAggregateRoot Create<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Create();
        }

        public TAggregateRoot Create<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Create(identity);
        }

        public TAggregateRoot Open<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().Open(identity);
        }

        public TAggregateRoot OpenOrCreate<TAggregateRoot>(Guid identity) where TAggregateRoot : class, IAggregateRoot, new() {
            return GetSource<TAggregateRoot>().OpenOrCreate(identity);
        }

        private EventSource<TAggregateRoot> GetSource<TAggregateRoot>() where TAggregateRoot : class, IAggregateRoot, new() {
            object source;
            if (_sources.TryGetValue(typeof(TAggregateRoot), out source))
                return (EventSource<TAggregateRoot>) source;

            throw new InvalidOperationException(
                string.Format("An event source for an aggregate root of type '{0}' is not available.",
                              typeof (TAggregateRoot).FullName));
        }
    }
}
