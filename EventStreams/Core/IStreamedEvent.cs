using System;

namespace EventStreams.Core {
    public interface IStreamedEvent : IStreamedEventIdentity, IStreamedEventTiming, IStreamedEventDiagnostics {
        EventArgs Arguments { get; }
    }
}