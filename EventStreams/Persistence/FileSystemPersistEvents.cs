using System;
using System.Collections.Generic;
using System.IO;

using EventStreams.Persistence.Serialization.Events;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    public class FileSystemPersistEvents : IPersistEvents {

        public string RootPath { get; set; }

        public FileSystemPersistEvents() {
            RootPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Persist(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend) {
            using (var fs = new FileStream(GetFileName(aggregateRoot), FileMode.Append, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan))
            using (var esw = new EventStreamWriter(fs, new JsonEventWriter()))
                esw.Write(eventsToAppend);
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            throw new NotSupportedException();
        }

        public virtual string GetFileName(IAggregateRoot aggregateRoot) {
            return Path.Combine(RootPath, aggregateRoot.Identity + ".events");
        }
    }
}
