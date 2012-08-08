using System;

namespace EventStreams.Core {
    public static class EventArgsExtensions {
        public static StreamedEvent ToStreamedEvent(this EventArgs args) {
            return new StreamedEvent(args);
        }
    }
}
