using System;

namespace EventStreams.Persistence {
    public enum EventStreamReaderState {
        HeadIndicator,
        HeadRecordLength,
        Id,
        Timestamp,
        ArgumentsType,
        Body,
        Hash,
        TailRecordLength,
        TailIndicator
    }
}
