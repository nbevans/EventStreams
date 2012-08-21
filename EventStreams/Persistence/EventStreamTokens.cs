using System;

namespace EventStreams.Persistence {
    internal static class EventStreamTokens {
        public static readonly byte RecordStartIndicator = 0x02;
        public static readonly byte RecordEndIndicator = 0x03;
    }
}
