using System;

namespace EventStreams.Core {
    public interface IStreamedEventDiagnostics {
        string DebugName { get; }
    }
}
