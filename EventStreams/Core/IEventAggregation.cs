using System;

namespace EventStreams.Core
{
    public interface IEventAggregation<TAggregateRoot>
        where TAggregateRoot : class, new()
    {
        Func<TAggregateRoot, TAggregateRoot> Aggregator { get; }
    }
}
