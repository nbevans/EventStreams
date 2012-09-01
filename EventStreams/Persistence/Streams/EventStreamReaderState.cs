using System;

namespace EventStreams.Persistence.Streams {
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
