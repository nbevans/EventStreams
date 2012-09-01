using System;

namespace EventStreams.Persistence.Streams {
    using Core;
    using Serialization.Events;

    internal interface IEventStreamReaderContext {
        IStreamedEvent Event { get; }
        byte[] StreamHash { get; }
        byte[] CurrentHash { get; }
        long CurrentHashPosition { get; }

        void HeadIndicator();
        void HeadRecordLength();
        void Id();
        void Timestamp();
        void ArgumentsType();
        void Body(IEventReader eventReader);
        void Hash();
        void TailRecordLength();
        void TailIndicator();
    }
}
