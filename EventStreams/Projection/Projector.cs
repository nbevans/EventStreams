using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Projection
{
    using Core;

    public class Projector {
        private readonly EventInvocationCache _invocationCache = new EventInvocationCache();

        public void Cache<TAggregateRoot>() where TAggregateRoot : class, new() {
            _invocationCache.Cache<TAggregateRoot>();
        }

        public TAggregateRoot Project<TAggregateRoot>(IEnumerable<IStreamedEvent> events) where TAggregateRoot : class, new() {
            Func<TAggregateRoot, IStreamedEvent, TAggregateRoot> foo =
                (currentState, currentEvent) => {
                    _invocationCache
                        .Get<TAggregateRoot>(currentEvent)
                        .Invoke(currentState, currentEvent, true);

                    return currentState;
                };

            return events.Aggregate(new TAggregateRoot(), foo);
        }
    }
}
