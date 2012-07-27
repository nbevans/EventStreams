namespace EventStreams.Core
{
    public interface IEvent<TAggregateRoot> : IEventIdentification, IEventTiming, IEventAggregation<TAggregateRoot>, IEventCore
        where TAggregateRoot : class, new()
    {
        //
        //
        //
    }
}