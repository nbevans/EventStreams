using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace EventStreams.Core
{
    [DataContract]
    [DebuggerDisplay("{Name}")]
    public abstract class EventBase<TAggregateRoot> : IEvent<TAggregateRoot> where TAggregateRoot : class, new()
    {
        [DataMember(Order = 0)]
        public Guid Id { get; private set; }

        [DataMember(Order = 1)]
        public DateTime Timestamp { get; private set; }

        protected EventBase()
        {
            Id = Guid.NewGuid();
            Timestamp = TimeSource.UtcNow;
            Aggregator = currentState => currentState;
        }

        public Func<TAggregateRoot, TAggregateRoot> Aggregator { get; protected set; }

        public string Name
        {
            get { return string.Format("#{0:x} @ {1:o}", Id.GetHashCode(), Timestamp); }
        }
    }
}
