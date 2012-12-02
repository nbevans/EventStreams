using System;

namespace EventStreams.Core {
    public interface IStreamedEventTiming {
        DateTime Timestamp { get; }
    }
}
