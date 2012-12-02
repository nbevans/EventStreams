using System;

namespace EventStreams.Core {
    public interface IStreamedEventIdentity {
        Guid Id { get; }
    }
}
