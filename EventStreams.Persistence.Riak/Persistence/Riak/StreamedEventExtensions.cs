using System;

namespace EventStreams.Persistence.Riak {
    using Core;

    internal static class StreamedEventExtensions {
        public static string ToRiakIdentity(this IStreamedEvent streamedEvent) {
            return streamedEvent != null ? streamedEvent.Id.ToRiakIdentity() : string.Empty;
        }
    }
}
