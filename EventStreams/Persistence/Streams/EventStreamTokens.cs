using System;

namespace EventStreams.Persistence.Streams {
    internal static class EventStreamTokens {
        public static readonly byte HeadIndicator = 0x02;
        public static readonly byte TailIndicator = 0x03;
    }
}
