using System;
using System.Collections.Generic;
using System.IO;

namespace EventStreams.Persistence.Streams {
    using Core;
    using Serialization.Events;

    public interface IEventStreamWriter : IDisposable {
        Stream InnerStream { get; }
        IEventWriter EventWriter { get; }

        void Write(IEnumerable<IStreamedEvent> streamedEvents);
    }
}