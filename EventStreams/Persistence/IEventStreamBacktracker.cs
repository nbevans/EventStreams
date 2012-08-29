using System;

namespace EventStreams.Persistence {
    public interface IEventStreamBacktracker {
        byte[] HashOrNull();
        byte[] HashOrNull(out long hashPosition);
    }
}