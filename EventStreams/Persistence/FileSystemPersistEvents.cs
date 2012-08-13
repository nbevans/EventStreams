using System;
using System.Collections.Generic;
using System.IO;

using EventStreams.Persistence.Serialization.Events;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    internal class FileSystemPersistEvents : IPersistEvents {

        public void Persist(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend) {
            using (var fs = new FileStream(aggregateRoot.Identity + ".events", FileMode.Append, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan))
            using (var esw = new EventStreamWriter(fs, new JsonEventWriter()))
                esw.Write(eventsToAppend);
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            throw new NotSupportedException();
        }
    }
}
