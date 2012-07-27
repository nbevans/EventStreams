using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStreams.Projection
{
    using Core;

    public class Projector
    {
        public TAggregateRoot Project<TAggregateRoot>(IEnumerable<IEvent<TAggregateRoot>> events) where TAggregateRoot : class, new()
        {
            Func<TAggregateRoot, IEvent<TAggregateRoot>, TAggregateRoot> foo =
                (currentState, currentEvent) =>
                    {


                        return currentEvent.Aggregator(currentState);
                    };

            return events.Aggregate(new TAggregateRoot(), foo);
        }
    }
}
