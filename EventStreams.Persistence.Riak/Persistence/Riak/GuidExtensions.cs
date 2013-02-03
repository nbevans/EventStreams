using System;

namespace EventStreams.Persistence.Riak {
    public static class GuidExtensions {
        public static string ToRiakIdentity(this Guid value) {
            return value.ToString("N");
        }
    }
}
