using System;

namespace EventStreams.Persistence.SelfHealing {
    public interface IEventStreamVerifier {
        void Verify();
    }
}