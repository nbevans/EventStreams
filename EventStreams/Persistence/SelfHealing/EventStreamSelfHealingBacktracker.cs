using System;

namespace EventStreams.Persistence.SelfHealing {
    public class EventStreamSelfHealingBacktracker : IEventStreamBacktracker {
        public byte[] HashOrNull() {
            throw new NotImplementedException();
        }

        public byte[] HashOrNull(out long hashPosition) {
            throw new NotImplementedException();
        }
    }
}
