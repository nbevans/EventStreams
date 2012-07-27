using System;

namespace EventStreams.Core
{
    public interface IEventTiming
    {
        DateTime Timestamp { get; }
    }
}
