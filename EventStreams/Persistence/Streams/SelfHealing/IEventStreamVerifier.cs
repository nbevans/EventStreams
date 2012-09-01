using System;

namespace EventStreams.Persistence.Streams.SelfHealing {
    public interface IEventStreamVerifier {
        void Verify();
    }
}