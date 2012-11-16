using System;

namespace EventStreams.Core.Domain {
    public interface IEventSourced {
        Guid Identity { get; }
        object Memento { get; } 
    }
}